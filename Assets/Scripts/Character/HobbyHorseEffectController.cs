using Sirenix.OdinInspector;
using UnityEngine;

public class HobbyHorseEffectController : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] private NotificationsSystem _notificationSystem;

    [SerializeField, FoldoutGroup("References")] private HobbyHorseMovement _movement;
    [SerializeField, FoldoutGroup("References")] private PlayerCameraRotator _cameraRotator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SuccessEffect();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            FailEffect();
        }
    }

    public void SuccessEffect()
    {
        string succes = GetSucces();

        _notificationSystem.SendSuccessNotification(succes, NotificationType.Information);
    }

    public void FailEffect()
    {
        string fail = GetFail();

        _notificationSystem.SendFailNotification(fail, NotificationType.Information);
        _cameraRotator.ShakeCamera(10);
    }

    private string GetFail()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                return "WTF!?";
            case 1:
                return "LoL";
            case 2:
                return "Bad!";
        }

        return "";
    }

    private string GetSucces()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                return "OMG!";
            case 1:
                return "Super!";
            case 2:
                return "Essa!";
        }

        return "";
    }
}
