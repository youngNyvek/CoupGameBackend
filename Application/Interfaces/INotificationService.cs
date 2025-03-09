namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyGroupAsync(string sessionCode, string eventName, object data);
    }
}
