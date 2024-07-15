using System.Linq;
using UnityEngine;

public class InteractableDetector : MonoBehaviour, IServiceLocatorComponent, IAwake
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private float _maxDist = 4;
    [SerializeField] private LayerMask _mask;

    private Transform _camera;
    private RaycastHit[] _hits = new RaycastHit[30];
    public void CustomAwake()
    {
        if(MyServiceLocator.TryGetServiceLocatorComponent(out PlayerStateMachine player))
        {
            player.FirstPersonCamera.IsNotNull(this, nameof(player));
            _camera = player.FirstPersonCamera;
        }
    }

    public RaycastHit[] GetDetectedItemsSorted()
    {
        RaycastHit[] hits = DetectItems();
        if (hits == null)return new RaycastHit[0];

        return DetectItems().Where(hit => !hit.Equals(null) && hit.collider != null && hit.collider.gameObject != null).OrderBy(hit => hit.distance).ToArray();
    }

    private RaycastHit[] DetectItems()
    {
        _hits = new RaycastHit[30];
        Ray ray = new Ray(_camera.transform.position, _camera.forward);
        if(Physics.RaycastNonAlloc(ray, _hits, _maxDist, _mask) == 0)return null;

        return _hits;
    }
}
