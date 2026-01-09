namespace Shared.Keycloak;

public static class PermissionCacheKeys
{
  public static class GroupMemberShip
  {
    public const string Base = "GroupMemberShip:";
    public static string GetBySubjectId(Guid subjectId) => Base + $"GetBySubjectId:{subjectId}";
  }
}
