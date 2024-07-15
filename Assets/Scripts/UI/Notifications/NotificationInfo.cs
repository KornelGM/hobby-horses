using I2.Loc;

public class NotificationInfo 
{
    public LocalizedString Description;
    public NotificationType Type;

    public bool CompareNotification(NotificationInfo info)
    {
        if (info.Description.mTerm != Description.mTerm) return false;
        if (info.Type != Type) return false;
        return true;
    }

    public NotificationInfo(LocalizedString description, NotificationType type = NotificationType.Information)
    {
        Description = description;
        Type = type;
    }
}
