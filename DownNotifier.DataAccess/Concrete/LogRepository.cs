using DownNotifier.DataAccess;
public class LogRepository : ILogRepository
{
    private readonly DownNotifierDbContext _context;

    public LogRepository(DownNotifierDbContext context)
    {
        _context = context;
    }
    public List<LogEntry> GetAllLogs()
    {
        return _context.Set<LogEntry>().OrderByDescending(x => x.TimeStamp).ToList();
    }
}