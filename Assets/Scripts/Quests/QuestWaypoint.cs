using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestWaypoint : MonoBehaviour, IServiceLocatorComponent, IWindow
{
    public Transform Target;
    public ServiceLocator TargetServiceLocator;
    public float OffsetY;

    [SerializeField] private TMP_Text distance;
    [SerializeField] private GameObject _arrowTransform;
    [SerializeField] private Image _arrowImage;

    [ServiceLocatorComponent] private PlayerManager _playerManager;

    private Camera mainCamera;

    public ServiceLocator MyServiceLocator { get; set; }
    public WindowManager Manager { get; set; }
    public GameObject MyObject { get => gameObject; }
    public int Priority { get; set; }
    public bool CanBeClosedByManager { get; set; }
    public bool IsOnTop { get; set; }
    public bool ShouldActivateCursor { get; set; }
    public bool ShouldBeCached { get; set; }
    public bool ShouldDeactivateCrosshair { get; set; } = false;

    private void Start()
    {
        distance.IsNotNull(this, nameof(distance));
    }

    private void Update()
    {
        if (Target == null) return;
        if (_playerManager.LocalPlayer == null) return;

        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }

        float minX = _arrowImage.GetPixelAdjustedRect().width / 2f;
        float maxX = Screen.width - minX;
        float minY = _arrowImage.GetPixelAdjustedRect().height / 2f;
        float maxY = Screen.height - minY;

        Vector3 targetPos = new(Target.position.x, Target.position.y + OffsetY, Target.position.z);

        Vector3 forward = Quaternion.Euler(new Vector3(0f, mainCamera.transform.eulerAngles.y, 0f)) * Vector3.forward;
        float angle = Vector3.SignedAngle(forward, targetPos - mainCamera.transform.position, Vector3.up);

        if (Mathf.Abs(90f - Mathf.Abs(angle)) < 30f)
        {
            targetPos = RotatePointAroundPivot(targetPos, mainCamera.transform.position, new Vector3(0f, -Mathf.Sign(angle) * (Mathf.Abs(angle) - 60f), 0f));
        }

        Vector2 screenPos = mainCamera.WorldToScreenPoint(targetPos) + Vector3.up * (_arrowImage.GetComponent<RectTransform>().sizeDelta.y/2) ;

        if (Vector3.Dot(targetPos - mainCamera.transform.position, mainCamera.transform.forward) < 0f)
        {
            if (screenPos.x < Screen.width / 2f)
            {
                screenPos.x = maxX;
            }
            else
            {
                screenPos.x = minX;
            }
        }

        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        _arrowTransform.transform.position = screenPos;

        distance.text = Vector3.Distance(_playerManager.LocalPlayer.transform.position, Target.position).ToString("0") + "m";
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

}
