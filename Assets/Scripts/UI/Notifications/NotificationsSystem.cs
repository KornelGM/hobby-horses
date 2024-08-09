using I2.Loc;
using System.Collections;
using UnityEngine;

public class NotificationsSystem : MonoBehaviour, IServiceLocatorComponent, IUpdateable
{
    public ServiceLocator MyServiceLocator { get; set; }

    public bool Enabled { get; } = true;
    public float TimeMultiplier;

    [field: SerializeField] public NotificationHandler SuccessNotificationHandler { get; private set; } = new();
    [field: SerializeField] public NotificationHandler SideNotificationHandler { get; private set; } = new();
    [field: SerializeField] public NotificationHandler FailNotificationHandler { get; private set; } = new();

    public void SendSuccessNotification(LocalizedString notificationText, NotificationType notificationType = NotificationType.Information)
    {
        SendNotification(notificationText, notificationType, SuccessNotificationHandler);
    }

    public void SendDelayedSideNotification(LocalizedString notificationText, NotificationType notificationType,
        float delay)
    {
        StartCoroutine(Delay(notificationText, notificationType, delay));
    }

    private IEnumerator Delay(LocalizedString notificationText, NotificationType notificationType,
        float delay)
    {
        yield return new WaitForSeconds(delay);
        SendSideNotification(notificationText, notificationType);
    }

    public void SendSideNotification(LocalizedString notificationText, NotificationType notificationType)
    {

        SendNotification(notificationText, notificationType, SideNotificationHandler);
    }

    public void SendFailNotification(LocalizedString notificationText, NotificationType notificationType)
    {
        SendNotification(notificationText, notificationType, FailNotificationHandler);
    }

    private void SendNotification(LocalizedString notificationText, NotificationType notificationType, NotificationHandler handler)
    {
        handler.AddNotification(new NotificationInfo(notificationText, notificationType), TimeMultiplier);
    }
    
    public void CustomUpdate()
    {
        SuccessNotificationHandler.CustomUpdate();
        SideNotificationHandler.CustomUpdate();
        FailNotificationHandler.CustomUpdate();
    }
}