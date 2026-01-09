namespace Shared.Caching;

public static class CacheTimes
{
  public static TimeSpan Short = TimeSpan.FromMinutes(5);
  public static TimeSpan Medium = TimeSpan.FromMinutes(30);
  public static TimeSpan Long = TimeSpan.FromHours(1);
  public static TimeSpan Day = TimeSpan.FromDays(1);
  public static TimeSpan Month = TimeSpan.FromDays(30);
}
