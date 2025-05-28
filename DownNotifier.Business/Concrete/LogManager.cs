using DownNotifier.ViewModels;

public class LogManager : ILogService
{
    private readonly ILogRepository _logRepository;

    public LogManager(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public List<LogViewModel> GetLogs()
    {
        var logs = _logRepository.GetAllLogs();

        return logs.Select(x => new LogViewModel
        {
            Id = x.Id,
            TimeStamp = x.TimeStamp,
            Level = x.Level,
            Message = x.Message,
            Exception = x.Exception
        }).ToList();
    }
}
