using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotificationsCanvasController : MonoBehaviour, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private NotificationMessage _sideNotificationPrefab;
    [SerializeField] private NotificationMessage _successNotificationPrefab;
    [SerializeField] private NotificationMessage _dialogueNotificationPrefab;
    [SerializeField] private Transform _sideNotificationParent;
    [SerializeField] private Transform _successNotificationParent;
    [SerializeField] private Transform _dialogueNotificationParent;

    [Space(10)]
    [SerializeField] private NotificationColor[] _notificationsColors;

    [Space(10)]
    [SerializeField] private LayoutGroup[] _handledLayoutGroups;

    [Space(10)]
    [SerializeField] private float _scrollingSpeed = 400f;

    [ServiceLocatorComponent] private NotificationsSystem _notificationsSystem;
    
    private List<KeyValuePair<NotificationInfo, NotificationMessage>> notificationsBeingShown = new();
    private float scrollingCumulation;

    public void CustomStart()
    { 
        _notificationsSystem.SideNotificationHandler.OnShowNotification += info => OnShowNotification(info, _sideNotificationParent);
        _notificationsSystem.SuccessNotificationHandler.OnShowNotification += info => OnShowNotification(info, _successNotificationParent);
        _notificationsSystem.DialoguesNotificationHandler.OnShowNotification += info => OnShowNotification(info, _dialogueNotificationParent);

        _notificationsSystem.SideNotificationHandler.OnNotificationDoubled += OnDuplicateNotification;
        _notificationsSystem.SuccessNotificationHandler.OnNotificationDoubled += OnDuplicateNotification;
        _notificationsSystem.DialoguesNotificationHandler.OnNotificationDoubled += OnDuplicateNotification;
        
        _notificationsSystem.SideNotificationHandler.OnHideNotification += info => OnHideNotification(info, _sideNotificationParent);
        _notificationsSystem.SuccessNotificationHandler.OnHideNotification += info => OnHideNotification(info, _successNotificationParent);
        _notificationsSystem.DialoguesNotificationHandler.OnHideNotification += info => OnHideNotification(info, _dialogueNotificationParent);
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
    
    private void OnHideNotification(NotificationInfo notificationInfo, Transform notificationParent)
    {
        NotificationMessage notificationObject = FindObjectByNotificationInfo(notificationInfo);
        for (int i = 0; i < notificationsBeingShown.Count; i++)
        {
            if (notificationsBeingShown[i].Key.CompareNotification(notificationInfo))
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
            var parentLayout = notificationParent.GetComponent<VerticalLayoutGroup>();
            int notificationHeight = (int)notificationObject.GetComponent<RectTransform>().rect.height;
            Destroy(notificationObject.gameObject);

            if (notificationsBeingShown.Count == 0) parentLayout.padding.top = 0;

            parentLayout.padding.top += (int)(notificationHeight + parentLayout.spacing);
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
                if (group.padding.top > 0)
                {
                    group.padding.top-=cumulatedInts;
                    if(group.padding.top < 0)
                        group.padding.top = 0;
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
        else if (transform == _dialogueNotificationParent)
            return _dialogueNotificationPrefab;

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