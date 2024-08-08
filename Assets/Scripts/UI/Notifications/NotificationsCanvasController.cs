using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static NotificationHandler;

public class NotificationsCanvasController : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private NotificationMessage _sideNotificationPrefab;
    [SerializeField] private NotificationMessage _successNotificationPrefab;
    [SerializeField] private NotificationMessage _failNotificationPrefab;
    [SerializeField] private Transform _sideNotificationParent;
    [SerializeField] private Transform _successNotificationParent;
    [SerializeField] private Transform _failNotificationParent;

    [Space(10)]
    [SerializeField] private NotificationColor[] _notificationsColors;

    [Space(10)]
    [SerializeField] private VerticalLayoutGroup[] _handledLayoutGroups;

    [Space(10)]
    [SerializeField] private float _scrollingSpeed = 400f;

    [ServiceLocatorComponent] private NotificationsSystem _notificationsSystem;
    
    private List<KeyValuePair<NotificationInfo, NotificationMessage>> notificationsBeingShown = new();
    private float scrollingCumulation;

    public void CustomStart()
    { 
        _notificationsSystem.SideNotificationHandler.OnShowNotification += info => OnShowNotification(info, _sideNotificationParent);
        _notificationsSystem.SuccessNotificationHandler.OnShowNotification += info => OnShowNotification(info, _successNotificationParent);
        _notificationsSystem.FailNotificationHandler.OnShowNotification += info => OnShowNotification(info, _failNotificationParent);

        _notificationsSystem.SideNotificationHandler.OnNotificationDoubled += OnDuplicateNotification;
        _notificationsSystem.SuccessNotificationHandler.OnNotificationDoubled += OnDuplicateNotification;
        _notificationsSystem.FailNotificationHandler.OnNotificationDoubled += OnDuplicateNotification;
        
        _notificationsSystem.SideNotificationHandler.OnHideNotification += info => OnHideNotification(info, _sideNotificationParent, _notificationsSystem.SideNotificationHandler);
        _notificationsSystem.SuccessNotificationHandler.OnHideNotification += info => OnHideNotification(info, _successNotificationParent, _notificationsSystem.SuccessNotificationHandler);
        _notificationsSystem.FailNotificationHandler.OnHideNotification += info => OnHideNotification(info, _failNotificationParent, _notificationsSystem.FailNotificationHandler);
    }

    private void Update()
    {
       AnimateNotificationsScrolling();
    }

    private void OnShowNotification(NotificationInfo notificationInfo, Transform notificationParent)
    {
        var prefabToInstantiate = GetSpecifiedPrefab(notificationParent);
        var instantiatedNotification = Instantiate(prefabToInstantiate, notificationParent);
        var notificationColor = GetNotificationColor(notificationInfo.Type);
        instantiatedNotification.SetNotification(notificationInfo, notificationColor);
        notificationsBeingShown.Add(new(notificationInfo, instantiatedNotification));

        if (instantiatedNotification == null)
            return;

        instantiatedNotification.ShowAnimation();
    }

    private void OnDuplicateNotification(NotificationInfo notificationInfo)
    {
        NotificationMessage notificationObject = FindObjectByNotificationInfo(notificationInfo);

        if (notificationObject == null)
            return;

        notificationObject.DuplicateAnimation();
    }
    
    private void OnHideNotification(NotificationData notificationInfo, Transform notificationParent, NotificationHandler handler)
    {
        NotificationMessage notificationObject = FindObjectByNotificationInfo(notificationInfo.Info);
        for (int i = 0; i < notificationsBeingShown.Count; i++)
        {
            if (notificationsBeingShown[i].Key.CompareNotification(notificationInfo.Info))
            {
                notificationsBeingShown.RemoveAt(i);
                break;
            }
        }

        if (notificationObject == null) return;

        UnityEvent onEndAnimation = new UnityEvent();
        onEndAnimation.AddListener(OnEndAnimation);

        notificationObject.HideAnimation(onEndAnimation);

        void OnEndAnimation()
        {
            handler.RemoveNotification(notificationInfo);
            var parentLayout = notificationParent.GetComponent<VerticalLayoutGroup>();
            int notificationHeight = (int)notificationObject.GetComponent<RectTransform>().rect.height;
            Destroy(notificationObject.gameObject);



            if (notificationsBeingShown.Count == 0)
            {
                if (parentLayout.reverseArrangement)
                    parentLayout.padding.top = 0;
                else
                    parentLayout.padding.bottom = 0;
            }

            if (parentLayout.reverseArrangement)
                parentLayout.padding.top += (int)(notificationHeight + parentLayout.spacing);
            else
                parentLayout.padding.bottom += (int)(notificationHeight + parentLayout.spacing);
        }
    }

    private void AnimateNotificationsScrolling()
    {
        
        scrollingCumulation += Time.deltaTime * _scrollingSpeed;
        if (scrollingCumulation >= 1)
        {
            int cumulatedInts = Mathf.FloorToInt(scrollingCumulation);
            _handledLayoutGroups.ForEach(group =>
            {
                if (group.reverseArrangement)
                {
                    if (group.padding.top > 0)
                    {
                        group.padding.top -= cumulatedInts;
                        if (group.padding.top < 0)
                            group.padding.top = 0;
                    }
                }
                else
                {
                    if (group.padding.bottom > 0)
                    {
                        group.padding.bottom -= cumulatedInts;
                        if (group.padding.bottom < 0)
                            group.padding.bottom = 0;
                    }
                }

            });
            scrollingCumulation -= cumulatedInts;
        }
    }

    private NotificationMessage GetSpecifiedPrefab(Transform transform)
    {
        if (transform == _sideNotificationParent)
            return _sideNotificationPrefab;
        else if (transform == _successNotificationParent)
            return _successNotificationPrefab;
        else if (transform == _failNotificationParent)
            return _failNotificationPrefab;

        return null;
    }

    private Color GetNotificationColor(NotificationType notificationType)
    {
        return _notificationsColors.FirstOrDefault(color => color.NotificationType == notificationType).ColorHook.Color;
    }

    private NotificationMessage FindObjectByNotificationInfo(NotificationInfo notificationInfo)
    {
            foreach (var pair in notificationsBeingShown)
        {
            if (pair.Key.CompareNotification(notificationInfo))
            {
                return pair.Value;
            }
        }

        return null;
    }
}