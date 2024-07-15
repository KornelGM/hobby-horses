using UnityEngine;
using Pathfinding;

public class DoorAstarController : MonoBehaviour, IServiceLocatorComponent, IAwake, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }
    [field: SerializeField] public NavmeshCut _navmeshCut { get; private set; }

    [SerializeField] BoxCollider _collider;
    [SerializeField] PathFindingNodeTags _openTag = PathFindingNodeTags.BasicGround;
    [SerializeField] PathFindingNodeTags _closedTag = PathFindingNodeTags.ClosedDoor;

    private Bounds _bounds;

    public void CustomAwake()
    {   
        _collider.IsNotNull(this);
        _bounds = _collider.bounds;
        
        if (_navmeshCut == null) return;
        
        _navmeshCut.enabled = false;
    }

    public void CustomStart()
    {
        OnClosed();
    }

    public void OnOpened()
    {
        SetNodesOpened(true);
        
        if (_navmeshCut == null) return;
        
        _navmeshCut.enabled = false;
    }
    public void OnClosed()
    {
        SetNodesOpened(false);
        
        if (_navmeshCut == null) return;
        
        _navmeshCut.enabled = true;
    }

    private void SetNodesOpened(bool open)
    {
        GraphUpdateObject guo = new GraphUpdateObject(_bounds);

        int tag = open ? (int)_openTag : (int)_closedTag;

        guo.modifyTag = true;
        guo.setTag = tag;
        guo.updatePhysics = false;

        AstarPath.active.UpdateGraphs(guo);
    }

}
