using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsManager : MonoBehaviour, IManager
{
    public event Action<Notification> OnNewNotification;

    public List<Notification> Notifications = new();

    public void AddNotification(string title, NotificationPosition position, NotificationsType type)
    {
        Notification notification = new(title, position, type);
        Notifications.Add(notification);

        OnNewNotification?.Invoke(notification);
    }

    public void RemoveNotification(Notification notification)
    {
        Notifications.Remove(notification);
    }

    public void CustomReset()
    {
        Notifications.Clear();
        OnNewNotification = null;
    }
}

public class Notification
{
    public string Title;
    public NotificationPosition Position;
    public NotificationsType Type;

    public Notification(string title, NotificationPosition position, NotificationsType type)
    {
        Title = title;
        Position = position;
        Type = type;
    }
}

public enum NotificationPosition
{
    Top,
    Middle
}

public enum NotificationsType
{
    Positive,
    Negative,
    Neutral
}
