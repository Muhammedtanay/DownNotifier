namespace DownNotifier.Business.Services
{
    public interface INotificationService
    {
        Task NotifyAsync(string to, string subject, string message);
    }
}