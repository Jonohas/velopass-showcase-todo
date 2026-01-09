# Shared.Permissions

A small, flexible EF Core model for permission-based access control. It provides:
- Explicitly defined entity types and relationships
- EntityTypeConfigurations with indexes and delete behaviors
- A ModelBuilder extension to plug into any existing DbContext

## Concepts
- Permission
  - Represents a capability in the system.
  - Identified by a stable Code (e.g., "CONTENT_READ").
- PermissionGroup
  - A named grouping of permissions (e.g., "Editors", "Admins").
- GroupPermission
  - Join entity assigning a Permission to a PermissionGroup.
- GroupMembership
  - Assigns a subject (by its Guid id) to a PermissionGroup.
  - Intentionally generic: the model does not prescribe what a "subject" is; you decide (e.g., user, role, team).

All entities use GUID v7 for Id generation at construction time.

## Installation/Reference
1) Add a project reference to Shared.Permissions from the project that owns your DbContext:
   - `<ProjectReference Include="..Shared\Shared.Permissions\Shared.Permissions.csproj" />`
2) Ensure EF Core is available in the project where the DbContext lives (this solution already does).

## Integrating with your DbContext
Call the extension during OnModelCreating to apply all entity configurations from this package:

```csharp
using Shared.Permissions;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
  // Optional: your default schema
  modelBuilder.HasDefaultSchema("tenant");

  // Apply your own assembly configurations first (optional)
  modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);

  // Apply Shared.Permissions configurations
  modelBuilder.ApplyPermissionsConfigurations();
}
```

This is demonstrated in Database.Tenant/TenantDbContext.cs in this solution.

## Tables and naming
- Table prefix is defined by Constants.TablePrefix = "permissions".
- Tables created (snake_case due to naming convention in this solution):
  - permissions_permission
  - permissions_group
  - permissions_group_permission
  - permissions_subject_group_membership

Key indices and constraints (enforced by configurations):
- Permission.Code unique index; Name indexed
- GroupPermission unique composite index on (GroupId, PermissionId)
- GroupMembership unique composite index on (SubjectId, GroupId)
- Delete behaviors:
  - Deleting a PermissionGroup cascades to its GroupPermissions and Memberships
  - Deleting a Permission cascades to its GroupPermissions

Adjust delete behaviors in the configurations if your domain needs different semantics (e.g., Restrict instead of Cascade).

## Modeling subjects (users/roles/…)
- SubjectId: Guid — the identifier of the subject in your system (e.g., your User.Id or Role.Id).
- The model is agnostic of subject type. If you support multiple subject kinds sharing the same Guid space, ensure you avoid collisions, for example by:
  - Using distinct id ranges/namespaces per subject kind, or
  - Keeping separate membership records per subject store if necessary.

## Seeding examples
You can seed permissions and groups during startup/migration steps. The following example uses a DbContext directly, but you can wrap these patterns in your own repositories/services.

```csharp
using Microsoft.EntityFrameworkCore;
using Shared.Permissions.Domain;

public static class PermissionsSeed
{
  public static async Task SeedAsync(DbContext db, CancellationToken ct = default)
  {
    var permissions = new []
    {
      new Permission("CONTENT_READ",  "Read content"),
      new Permission("CONTENT_WRITE", "Write content"),
      new Permission("USER_MANAGE",   "Manage users"),
    };

    foreach (var p in permissions)
    {
      var exists = await db.Set<Permission>().AnyAsync(x => x.Code == p.Code, ct);
      if (!exists) await db.Set<Permission>().AddAsync(p, ct);
    }

    var admins = await db.Set<PermissionGroup>().FirstOrDefaultAsync(g => g.Name == "Admins", ct)
                 ?? (await db.Set<PermissionGroup>().AddAsync(new PermissionGroup("Admins", "Administrators"), ct)).Entity;

    await db.SaveChangesAsync(ct);

    // Ensure group-permission links
    async Task EnsureGroupPermission(Guid groupId, Guid permissionId)
    {
      var exists = await db.Set<GroupPermission>().AnyAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId, ct);
      if (!exists) await db.Set<GroupPermission>().AddAsync(new GroupPermission(groupId, permissionId), ct);
    }

    var permIds = await db.Set<Permission>()
      .Where(p => new []{"CONTENT_READ","CONTENT_WRITE","USER_MANAGE"}.Contains(p.Code))
      .Select(p => p.Id)
      .ToListAsync(ct);

    foreach (var pid in permIds)
      await EnsureGroupPermission(admins.Id, pid);

    await db.SaveChangesAsync(ct);

    // Optionally assign a subject (e.g., user) to Admins group
    var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    var membershipExists = await db.Set<GroupMembership>()
      .AnyAsync(m => m.SubjectId == userId && m.GroupId == admins.Id, ct);

    if (!membershipExists)
      await db.Set<GroupMembership>().AddAsync(new GroupMembership(userId, admins.Id), ct);

    await db.SaveChangesAsync(ct);
  }
}
```

## Querying effective permissions for a subject
Get a list of permission codes granted to a subject via group memberships:

```csharp
using Microsoft.EntityFrameworkCore;
using Shared.Permissions.Domain;

public static Task<List<string>> GetPermissionCodesForSubject(DbContext db, Guid subjectId, CancellationToken ct)
{
  return db.Set<GroupMembership>()
    .Where(m => m.SubjectId == subjectId)
    .Join(db.Set<GroupPermission>(), m => m.GroupId, gp => gp.GroupId, (m, gp) => gp)
    .Join(db.Set<Permission>(), gp => gp.PermissionId, p => p.Id, (gp, p) => p.Code)
    .Distinct()
    .ToListAsync(ct);
}
```

Check whether a subject has a specific permission code:

```csharp
public static Task<bool> SubjectHasPermission(DbContext db, Guid subjectId, string permissionCode, CancellationToken ct)
{
  return db.Set<GroupMembership>()
    .Where(m => m.SubjectId == subjectId)
    .Join(db.Set<GroupPermission>(), m => m.GroupId, gp => gp.GroupId, (m, gp) => gp)
    .Join(db.Set<Permission>(), gp => gp.PermissionId, p => p.Id, (gp, p) => p.Code)
    .AnyAsync(code => code == permissionCode, ct);
}
```

Tip: In higher-traffic paths, consider caching flattened permissions per subject to avoid repeated joins.

## Migrations
Run EF Core migrations in the project that owns your DbContext. For example, if your DbContext is in Database.Tenant and your startup is the Api project:

```bash
# From solution root
# Add a migration
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet ef migrations add AddPermissions \
  -p Database.Tenant/Database.Tenant.csproj \
  -s Api/Api.csproj

# Apply migration
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet ef database update \
  -p Database.Tenant/Database.Tenant.csproj \
  -s Api/Api.csproj
```

This solution sets Npgsql and snake_case naming in TenantDbContextFactory, and calls ApplyPermissionsConfigurations() in TenantDbContext.

## Multi-tenant notes
- The model is storage-agnostic: it works in single-tenant or multi-tenant contexts.

## Extending
- Add domain services tailored to your application.
- Introduce additional aggregate roots (e.g., Resource-level grants) by adding your own entities/configurations.
- Consider adding centralized constants for permission codes in your application to avoid typos and enable compile-time references.

## Notes
- All entity Ids are GUID v7 generated in constructors; EF configurations set ValueGeneratedNever().
- Delete behaviors are explicitly defined; review them for your data lifecycle and auditing requirements.
