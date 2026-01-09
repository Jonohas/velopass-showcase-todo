using Microsoft.EntityFrameworkCore;
using Shared.S3ObjectStorage.Domain.Folder;
using Shared.S3ObjectStorage.Domain.Image;
using Shared.S3ObjectStorage.EF.EntityTypeConfigurations;
using File = Shared.S3ObjectStorage.Domain.File.File;

namespace Shared.S3ObjectStorage.Tests;

public class TestDbContext : DbContext
{
  public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

  public DbSet<File> Files => Set<File>();
  public DbSet<Folder> Folders => Set<Folder>();
  public DbSet<Image> Images => Set<Image>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileConfiguration).Assembly);
  }
}

public class ObjectStorageModelTests
{
  private static TestDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<TestDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;
    return new TestDbContext(options);
  }

  /// <summary>
  /// Validates that the entity configuration for the <see cref="Folder"/> class is correctly applied
  /// in the database context, including table name, constraints, and relationships.
  /// </summary>
  /// <remarks>
  /// Assertions include:
  /// - The table name for the Folder entity is set to "objectstorage_folder".
  /// - The <see cref="Folder.Name"/> property is required and has a maximum length of 255 characters.
  /// - A self-referencing foreign key based on <see cref="Folder.ParentId"/> exists with a delete behavior of Restrict.
  /// - An index on the <see cref="Folder.ParentId"/> property exists in the database.
  /// </remarks>
  [Fact]
  public void FolderConfiguration_Should_Set_Table_And_Constraints()
  {
    using var ctx = CreateContext();
    var entityType = ctx.Model.FindEntityType(typeof(Folder));
    Assert.NotNull(entityType);

    // Table name
    Assert.Equal("objectstorage_folder", entityType!.GetTableName());

    // Name property required and max length 255
    var nameProp = entityType.FindProperty(nameof(Folder.Name));
    Assert.NotNull(nameProp);
    Assert.False(nameProp!.IsNullable);
    Assert.Equal(255, nameProp.GetMaxLength());

    // Self-referencing FK on ParentId with Restrict delete
    var fk = entityType.GetForeignKeys()
      .Single(f => f.Properties.Count == 1 && f.Properties.First().Name == nameof(Folder.ParentId));
    Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);

    // Index on ParentId exists
    var hasIndex = entityType.GetIndexes()
      .Any(ix => ix.Properties.Count == 1 && ix.Properties.First().Name == nameof(Folder.ParentId));
    Assert.True(hasIndex);
  }

  /// <summary>
  /// Ensures that the entity configuration for the <see cref="File"/> class is correctly applied
  /// in the database context, including table name, relationships, and indexes.
  /// </summary>
  /// <remarks>
  /// Verifications include:
  /// - The table name for the File entity is set to "objectstorage_file".
  /// - A foreign key relationship exists between the File entity and its ParentFolder,
  /// with a delete behavior of SetNull based on the <see cref="File.ParentId"/> property.
  /// - An index is present on the <see cref="File.ParentId"/> property.
  /// </remarks>
  [Fact]
  public void FileConfiguration_Should_Set_Table_Relationship_And_Index()
  {
    using var ctx = CreateContext();
    var entityType = ctx.Model.FindEntityType(typeof(File));
    Assert.NotNull(entityType);

    // Table name
    Assert.Equal("objectstorage_file", entityType!.GetTableName());

    // Relationship to ParentFolder with SetNull delete behavior
    var fk = entityType.GetForeignKeys()
      .Single(f => f.Properties.Count == 1 && f.Properties.First().Name == nameof(File.ParentId));
    Assert.Equal(DeleteBehavior.SetNull, fk.DeleteBehavior);

    // Index on ParentId exists
    var hasIndex = entityType.GetIndexes()
      .Any(ix => ix.Properties.Count == 1 && ix.Properties.First().Name == nameof(File.ParentId));
    Assert.True(hasIndex);
  }

  /// <summary>
  /// Validates that the entity configuration for the <see cref="Image"/> class is correctly applied
  /// in the database context, including table name, relationships, and required/optional constraints.
  /// </summary>
  /// <remarks>
  /// Assertions include:
  /// - The table name for the Image entity is set to "objectstorage_images".
  /// - The <see cref="Image.OriginalFileId"/> property is required and enforces a Restrict delete behavior in its foreign key.
  /// - All optional file reference properties (<see cref="Image.ThumbnailFileId"/>, <see cref="Image.SmallFileId"/>,
  /// <see cref="Image.MediumFileId"/>, <see cref="Image.LargeFileId"/>, <see cref="Image.ExtraLargeFileId"/>,
  /// <see cref="Image.XXLargeFileId"/>, and <see cref="Image.MaxFileId"/>) enforce a Restrict delete behavior
  /// in their foreign keys, but are not required.
  /// </remarks>
  [Fact]
  public void ImageConfiguration_Should_Set_Table_Relationships_And_Requirements()
  {
    using var ctx = CreateContext();
    var entityType = ctx.Model.FindEntityType(typeof(Image));
    Assert.NotNull(entityType);

    // Table name
    Assert.Equal("objectstorage_images", entityType!.GetTableName());

    // Find FKs by property name
    var fks = entityType.GetForeignKeys().ToList();

    var originalFk = fks.Single(f => f.Properties.Single().Name == nameof(Image.OriginalFileId));
    Assert.Equal(DeleteBehavior.Restrict, originalFk.DeleteBehavior);
    Assert.True(originalFk.IsRequired);

    string[] optionalProps = new[]
    {
      nameof(Image.ThumbnailFileId), nameof(Image.SmallFileId), nameof(Image.MediumFileId),
      nameof(Image.LargeFileId), nameof(Image.ExtraLargeFileId), nameof(Image.XXLargeFileId), nameof(Image.MaxFileId)
    };

    foreach (var prop in optionalProps)
    {
      var fk = fks.Single(f => f.Properties.Single().Name == prop);
      Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
      Assert.False(fk.IsRequired);
    }
  }
}