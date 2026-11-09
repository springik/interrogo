namespace Web.Services;

public class LiveSyncService
{
    public event Action<string>? OnEventChanged;
    public event Action? OnNewEventCreated;

    public void NotifyEventChanged(string eventId)
    {
        OnEventChanged?.Invoke(eventId);
    }
    
    public void NotifyNewEventCreated() => OnNewEventCreated?.Invoke();
}