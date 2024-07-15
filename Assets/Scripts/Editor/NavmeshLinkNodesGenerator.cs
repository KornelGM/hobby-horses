#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Pathfinding;

public class NavmeshLinkNodesGenerator : EditorWindow
{
    private int dropIndex = 1;
    private int jumpIndex = 2;
    private float maxDropHeight = 1f;
    private float maxJumpHeight = 1f;
    private float XZMaxNodeDistance = 4f;
    private float XZNodeSpace = 1f;
    private Transform nodesParentObject;
    private Transform dropsParentObject;
    private Transform jumpsParentObject;

    [MenuItem("Tools/A*/Generate navmesh link nodes")]
    private static void Init()
    {
        GetWindow<NavmeshLinkNodesGenerator>("Are you sure?").Show();
    }

    private void OnGUI()
    {
        if (!nodesParentObject)
        {
            nodesParentObject = GameObject.Find("Nodes parent").transform;
        }

        if (!dropsParentObject)
        {
            dropsParentObject = GameObject.Find("Drops position bounds parent").transform;
        }

        if (!jumpsParentObject)
        {
            jumpsParentObject = GameObject.Find("Jumps position bounds parent").transform;
        }

        EditorGUILayout.LabelField("All contents of nodes parent object will be removed");
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Nodes parent object");
        nodesParentObject = EditorGUILayout.ObjectField(nodesParentObject, typeof(Transform), true) as Transform;
        EditorGUILayout.LabelField("Drops position bounds parent object");
        dropsParentObject = EditorGUILayout.ObjectField(dropsParentObject, typeof(Transform), true) as Transform;
        EditorGUILayout.LabelField("Jumps position bounds parent object");
        jumpsParentObject = EditorGUILayout.ObjectField(jumpsParentObject, typeof(Transform), true) as Transform;
        EditorGUILayout.Space();

        dropIndex = EditorGUILayout.IntField("Drop tag index", dropIndex);
        jumpIndex = EditorGUILayout.IntField("Jump tag index", jumpIndex);
        maxDropHeight = EditorGUILayout.FloatField("Max drop height", maxDropHeight);
        maxJumpHeight = EditorGUILayout.FloatField("Max jump height", maxJumpHeight);
        XZMaxNodeDistance = EditorGUILayout.FloatField("Max node distance", XZMaxNodeDistance);
        XZNodeSpace = EditorGUILayout.FloatField("Max node distance", XZNodeSpace);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Proceed"))
        {
            bool error = false;

            if (nodesParentObject == dropsParentObject.transform.parent)
            {
                Debug.LogError("Nodes parent object cannot be a parent to drops position bounds parent object");
                error = true;
            }

            if (nodesParentObject == jumpsParentObject.transform.parent)
            {
                Debug.LogError("Nodes parent object cannot be a parent to jumps position bounds parent object");
                error = true;
            }

            if (XZNodeSpace <= 0f)
            {
                Debug.LogError("XZ Node Space must be bigger than 0");
                error = true;
            }
            
            if (!error)
            {
                Generate();
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            GetWindow<NavmeshLinkNodesGenerator>().Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void Generate()
    {
        while (nodesParentObject.childCount > 0)
        {
            DestroyImmediate(nodesParentObject.GetChild(0).gameObject);
        }

        float dropAddValue = 0f;
        bool dropSwitched = false;
        for (int i = 0; i < dropsParentObject.childCount;)
        {
            NodeLinksBoundingBox dropBox = dropsParentObject.GetChild(i).GetComponent<NodeLinksBoundingBox>();
            dropBox.transform.localScale = Vector3.one;

            float jumpAddValue = 0f;
            bool jumpSwitched = false;
            for (int j = 0; j < jumpsParentObject.childCount;)
            {
                NodeLinksBoundingBox jumpBox = jumpsParentObject.GetChild(j).GetComponent<NodeLinksBoundingBox>();
                jumpBox.transform.localScale = Vector3.one;

                Vector3 dropPos = dropBox.transform.position;
                Vector3 jumpPos = jumpBox.transform.position;

                dropPos.x += dropAddValue;
                jumpPos.x += jumpAddValue;

                dropPos.y = 0f;
                jumpPos.y = 0f;

                if ((dropPos - jumpPos).sqrMagnitude <= XZMaxNodeDistance * XZMaxNodeDistance)
                {
                    dropPos -= dropBox.transform.position;
                    jumpPos -= jumpBox.transform.position;

                    dropPos.y = Mathf.Abs(dropBox.size.y) / 2f;
                    jumpPos.y = Mathf.Abs(jumpBox.size.y) / 2f;
                    Vector3 dropPosWorld = dropBox.transform.TransformPoint(dropPos);
                    Vector3 jumpPosWorld = jumpBox.transform.TransformPoint(jumpPos);
                    Physics.Raycast(dropPosWorld, Vector3.down, out RaycastHit hitInfoDrop, dropBox.size.y);
                    Physics.Raycast(jumpPosWorld, Vector3.down, out RaycastHit hitInfoJump, jumpBox.size.y);

                    // Create drop node
                    if (hitInfoDrop.transform && hitInfoJump.transform)
                    {
                        if (maxDropHeight > 0f && dropPosWorld.y - jumpPosWorld.y <= maxDropHeight)
                        {
                            CreateNodeLink(hitInfoDrop, hitInfoJump, "Drop node", dropIndex);
                        }

                        if (maxJumpHeight > 0f && dropPosWorld.y - jumpPosWorld.y <= maxJumpHeight)
                        {
                            CreateNodeLink(hitInfoJump, hitInfoDrop, "Jump node", jumpIndex);
                        }
                    }
                }

                if (!jumpSwitched && jumpAddValue != 0f)
                {
                    jumpAddValue = -jumpAddValue;
                    jumpSwitched = true;
                }
                else if (Mathf.Abs(jumpAddValue) + Mathf.Abs(XZNodeSpace) > Mathf.Abs(jumpBox.size.x / 2f))
                {
                    ++j;
                    jumpAddValue = 0f;
                    jumpSwitched = false;
                }
                else
                {
                    jumpAddValue = Mathf.Abs(jumpAddValue) + XZNodeSpace;
                    jumpSwitched = false;
                }
            }



            if (!dropSwitched && dropAddValue != 0f)
            {
                dropAddValue = -dropAddValue;
                dropSwitched = true;
            }
            else if (Mathf.Abs(dropAddValue) + Mathf.Abs(XZNodeSpace) > Mathf.Abs(dropBox.size.x / 2f))
            {
                ++i;
                dropAddValue = 0f;
                dropSwitched = false;
            }
            else
            {
                dropAddValue = Mathf.Abs(dropAddValue) + XZNodeSpace;
                dropSwitched = false;
            }
        }

        Debug.Log($"Created {nodesParentObject.childCount} nodes");

        GetWindow<NavmeshLinkNodesGenerator>().Close();
    }

    private void CreateNodeLink(RaycastHit hitInfo, RaycastHit hitInfo2, string name, int index)
    {
        GameObject nodeObject = new(name);
        nodeObject.transform.position = hitInfo.point;
        nodeObject.transform.parent = nodesParentObject;
        GameObject nodeEndObject = new("EndNode");
        nodeEndObject.transform.position = hitInfo2.point;
        nodeEndObject.transform.parent = nodeObject.transform;
        NodeLink2 nodeLink = nodeObject.AddComponent(typeof(NodeLink2)) as NodeLink2;
        nodeLink.costFactor = 1f;
        nodeLink.nodeTag = (uint)index;
        nodeLink.oneWay = true;
        nodeLink.end = nodeEndObject.transform;
    }
}
#endif
