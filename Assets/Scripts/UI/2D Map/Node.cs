using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class Node : MonoBehaviour
{
#if UNITY_EDITOR
    [ListDrawerSettings(
        CustomAddFunction = nameof(AddNodeNeighbour),
        CustomRemoveElementFunction = nameof(OnNeighbourRemoved))]
#endif
    [FormerlySerializedAs("neighbors")] public List<Node> Neighbors;
    [SerializeField] private float size;

    private Image _image;
    private bool _nodesEnabled;

    private void Awake()
    {
        _image = GetComponent<Image>();
        SetTransparency(0f);
    }

    private void Update()
    {
        ToggleNodesVisibility();
    }

    private void ToggleNodesVisibility()
    {

        if (!Input.GetKeyDown(KeyCode.G))
            return;

        if (!_nodesEnabled)
            SetTransparency(1f);
        else
            SetTransparency(0f);

        _nodesEnabled = !_nodesEnabled;

    }
    private void SetTransparency(float alpha)
    {
        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }

    private void OnValidate()
    {
        GetComponent<RectTransform>().localScale = new Vector3(size, size, size);
    }

#if UNITY_EDITOR
    [SerializeField, Range(0, 100)] private float _nodeFindingSphere;
#endif

    private void OnDrawGizmosSelected()
    {

#if UNITY_EDITOR
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _nodeFindingSphere);
#endif

        Gizmos.color = Color.green;
        foreach (Node neighbor in Neighbors)
        {
            if (!neighbor.Neighbors.Contains(this))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
                continue;
            }
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }

#if UNITY_EDITOR
    private void OnNeighbourRemoved(Node n)
    {
        Neighbors.Remove(n);
        if (n != null) n.Neighbors.Remove(this);
    }
    private void AddNodeNeighbour(Node n)
    {
        if (Neighbors.Contains(n)) return;

        Neighbors.Add(n);

        if (n.Neighbors.Contains(this)) return;

        n.Neighbors.Add(this);
    }

    [Button]
    private void GetNodes()
    {
        var hits = Physics.SphereCastAll(transform.position, _nodeFindingSphere, Vector3.up, _nodeFindingSphere);
        Neighbors = new();
        foreach (var hit in hits)
        {
            if (!hit.transform.TryGetComponent<Node>(out Node node)) continue;
            if (node == this) continue;
            AddNodeNeighbour(node);
        }
    }
#endif

}