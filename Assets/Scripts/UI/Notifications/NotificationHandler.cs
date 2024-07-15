using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class NotificationHandler
{
    public class NotificationData
    {
        public float MaxTime;
        public float LeastTime;
        public NotificationInfo Info;

        public void ResetTimer() => LeastTime = MaxTime;

        public  NotificationData(NotificationInfo info, float time, float minTime, float timeMultiplier)
        {
            Info = info;

            string text = "";
            if(info.Description == null)
                text = info.Description.mTerm;
            else 
                text = info.Description;

            float maxTime = time * text.Length * timeMultiplier;
            MaxTime = maxTime > minTime ? maxTime : minTime;
            LeastTime = MaxTime;
        }
    }

    public event Action<NotificationInfo> OnShowNotification;
    public event Action<NotificationInfo> OnNotificationDoubled;
    public event Action<NotificationInfo> OnHideNotification;

    [SerializeField] private float _unitOfTimePerLetter = 0.15f;
    [SerializeField] private float _minimumNotyficationTime = 3f;
    [SerializeField] private int _maxNotificationsSameTime;

    private int _spacesForNotification => _maxNotificationsSameTime - _shownNotifications.Count;
    private Queue<NotificationData> _notificationsInQueue = new();
    private List<NotificationData> _shownNotifications = new();


    public void AddNotification(NotificationInfo info, float timeMultiplier)
    {
        NotificationData alreadyExistingNotification = GetNotificationDataByInfo(info);
        if (alreadyExistingNotification != null)
        {
            OnNotificationDoubled?.Invoke(alreadyExistingNotification.Info);
            alreadyExistingNotification.ResetTimer();
            return;
        }

        _notificationsInQueue.Enqueue(new NotificationData(info, _unitOfTimePerLetter, _minimumNotyficationTime, timeMultiplier));
    }

    public void CustomUpdate()
    {
        Countdown();
        TryShowNotificationsFromQueue();
    }

    private void Countdown()
    {
        for (int i = _shownNotifications.Count - 1; i >=0 ; i--)
        {
            NotificationData notification = _shownNotifications[i];
            notification.LeastTime -= Time.deltaTime;

            if(notification.LeastTime <= 0)
            {
                HideNotification(notification.Info);
                _shownNotifications.Remove(notification);
            }
        }
    }

    private void TryShowNotificationsFromQueue()
    {
        for (int i = 0; i < _spacesForNotification; i++)
        {       
            if (_notificationsInQueue.Count == 0) return;
            
            NotificationData notification = _notificationsInQueue.Dequeue();

            ShowNotification(notification.Info);
            _shownNotifications.Add(notification);
        }
    }

    private void ShowNotification(NotificationInfo notification)
    {
        OnShowNotification?.Invoke(notification);
    }

    private void HideNotification(NotificationInfo notification)
    {
        OnHideNotification?.Invoke(notification);
    }

    private NotificationData GetNotificationDataByInfo(NotificationInfo info)
    {
        var foundNotificationData = _shownNotifications.ToList().Find(notification => notification.Info.CompareNotification(info));

        if (foundNotificationData != null) return foundNotificationData;
        
        return _notificationsInQueue.ToList().Find(notification => notification.Info.CompareNotification(info));
    }
}
