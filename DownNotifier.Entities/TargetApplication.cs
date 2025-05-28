using DownNotifier.Entities;

public class TargetApplication : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int CheckIntervalInMinutes { get; set; }
    public bool IsActive { get; set; }
    public string UserId { get; set; }

    public DateTime LastChecked { get; set; }
    public int LastStatusCode { get; set; }
}
