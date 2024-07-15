using UnityEngine;
using Sirenix.OdinInspector;

public class GraphUpdater : MonoBehaviour, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    [SerializeField] Collider[] _collidersOnLevels;
    [SerializeField] Collider _defaultCollider;
    public void UpdateGraph(int level)
    {
        if (level < 0 || level >= _collidersOnLevels.Length)
        {
            UpdateGraph();
            return;
        }
        AstarPath.active.UpdateGraphs(_collidersOnLevels[level].bounds);
    }

    public void UpdateGraph()
    {
        if (_defaultCollider == null) return;
        AstarPath.active.UpdateGraphs(_defaultCollider.bounds);
    }

    [Button]
    private void CreateUpdateColliders()
    {
        VisualItemService visualItemService = transform.parent.GetComponentInChildren<VisualItemService>();
        if (visualItemService == null)
        {
            Debug.Log("Could not find visual item service");
            return;
        }

        Collider[] colliders = visualItemService.GetComponentsInChildren<Collider>(true);
        Vector3 maxValue = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Vector3 minValue = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        foreach (var collider in colliders)
        {
            Vector3 boundsMax = collider.bounds.max;
            Vector3 boundsMin = collider.bounds.min;
            
            maxValue.x = maxValue.x < boundsMax.x ? boundsMax.x : maxValue.x;
            maxValue.y = maxValue.y < boundsMax.y ? boundsMax.y : maxValue.y;
            maxValue.z = maxValue.z < boundsMax.z ? boundsMax.z : maxValue.z;

            minValue.x = minValue.x > boundsMin.x ? boundsMin.x : minValue.x;
            minValue.y = minValue.y > boundsMin.y ? boundsMin.y : minValue.y;
            minValue.z = minValue.z > boundsMin.z ? boundsMin.z : minValue.z;
        }
        Vector3 center = (maxValue + minValue) * 0.5f;
        Vector3 size = new Vector3(Mathf.Abs(maxValue.x - minValue.x), Mathf.Abs(maxValue.y - minValue.y), Mathf.Abs(maxValue.z - minValue.z));

        DestroyImmediate(GetComponent<BoxCollider>());

        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

        boxCollider.isTrigger = true;
        boxCollider.center = transform.InverseTransformPoint(center);
        boxCollider.size = size;

        _collidersOnLevels = new Collider[3];
        _collidersOnLevels[0] = boxCollider;
        _collidersOnLevels[1] = boxCollider;
        _collidersOnLevels[2] = boxCollider;
        _defaultCollider = boxCollider;
    }

}
