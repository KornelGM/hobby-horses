using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NavmeshAddGenerator : MonoBehaviour
{
    [SerializeField] Vector3 boundingBox = new(1f, 0.5f, 1f);

    Color _defaultColor = new(0.25f, 0.25f, 1f, 0.5f);
    NavmeshAdd _navmeshAdd;
    Vector3[] _vertices;
    int[] _triangles;
    bool _generateVerticesAndTriangles = true;

    private void OnEnable()
    {
        // TODO: Work well with rotation
        transform.rotation = Quaternion.identity;

        _navmeshAdd = GetComponent<NavmeshAdd>();
        if (!_navmeshAdd)
        {
            _navmeshAdd = gameObject.AddComponent<NavmeshAdd>();
        }
        _navmeshAdd.enabled = false;
        _navmeshAdd.type = NavmeshAdd.MeshType.CustomMesh;
        _navmeshAdd.useRotationAndScale = false;
        GenerateVertices();
        GenerateTriangles();
        _navmeshAdd.RebuildMesh();
        _navmeshAdd.enabled = true;
        _generateVerticesAndTriangles = false;
    }

    private void GenerateVertices()
    {
        Mesh mesh = new();
        _navmeshAdd.mesh = mesh;
        if (!_generateVerticesAndTriangles)
        {
            mesh.vertices = _vertices;
            return;
        }

        NNInfo info = AstarPath.active.GetNearest(transform.position);
        TriangleMeshNode node = info.node as TriangleMeshNode;
        node.GetVertices(out Int3 v0, out Int3 v1, out Int3 v2);
        Vector3 v0f = Int3ToVector3(v0);
        Vector3 v1f = Int3ToVector3(v1);
        Vector3 v2f = Int3ToVector3(v2);

        BoxCollider col = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
        col.size = boundingBox;

        RaycastHit hit0;

        if (col.Raycast(new Ray(v0f, (v1f - v0f).normalized), out hit0, (v0f - v1f).magnitude))
        {
            col.Raycast(new Ray(v1f, (v0f - v1f).normalized), out RaycastHit hit1, (v0f - v1f).magnitude);
            SetVertices(mesh, node, col, v0f, v1f, hit0, hit1);
        }
        else if (col.Raycast(new Ray(v1f, (v2f - v1f).normalized), out hit0, (v1f - v2f).magnitude))
        {
            col.Raycast(new Ray(v2f, (v1f - v2f).normalized), out RaycastHit hit1, (v1f - v2f).magnitude);
            SetVertices(mesh, node, col, v1f, v2f, hit0, hit1);
        }
        else if (col.Raycast(new Ray(v2f, (v0f - v2f).normalized), out hit0, (v2f - v0f).magnitude))
        {
            col.Raycast(new Ray(v0f, (v2f - v0f).normalized), out RaycastHit hit1, (v2f - v0f).magnitude);
            SetVertices(mesh, node, col, v2f, v0f, hit0, hit1);
        }

        Destroy(col);

        mesh.vertices = _vertices;
    }

    private void GenerateTriangles()
    {
        if (!_generateVerticesAndTriangles)
        {
            _navmeshAdd.mesh.triangles = _triangles;
            return;
        }

        _triangles = new int[12];
        _triangles[0] = 0; _triangles[1] = 1; _triangles[2] = 5;
        _triangles[3] = 5; _triangles[4] = 1; _triangles[5] = 4;
        _triangles[6] = 4; _triangles[7] = 1; _triangles[8] = 2;
        _triangles[9] = 2; _triangles[10] = 3; _triangles[11] = 4;

        _navmeshAdd.mesh.triangles = _triangles;
    }

    private void SetVertices(Mesh mesh, TriangleMeshNode node, BoxCollider col, Vector3 start, Vector3 end, RaycastHit hit0, RaycastHit hit1)
    {
        _vertices = new Vector3[6];
        Vector3 nodePosition = Int3ToVector3(node.position);
        Vector3 hit0NodeVector = (hit0.point - nodePosition).normalized * 0.01f;
        Vector3 hit1NodeVector = (hit1.point - nodePosition).normalized * 0.01f;

        _vertices[0] = -transform.position + start;
        _vertices[1] = -transform.position + hit0.point + hit0NodeVector;
        _vertices[2] = GetMidVertex(hit0.point, hit0NodeVector, col);
        _vertices[3] = GetMidVertex(hit1.point, hit1NodeVector, col);
        _vertices[4] = -transform.position + hit1.point + hit1NodeVector;
        _vertices[5] = -transform.position + end;

        mesh.vertices = _vertices;
    }

    private Vector3 GetMidVertex(Vector3 hitPoint, Vector3 hitNodeVector, Collider col)
    {
        float f0 = (-transform.position + hitPoint + hitNodeVector + new Vector3(-col.bounds.extents.x, hitNodeVector.y, -col.bounds.extents.z)).sqrMagnitude;
        float f1 = (-transform.position + hitPoint + hitNodeVector + new Vector3(-col.bounds.extents.x, hitNodeVector.y, col.bounds.extents.z)).sqrMagnitude;
        float f2 = (-transform.position + hitPoint + hitNodeVector + new Vector3(col.bounds.extents.x, hitNodeVector.y, -col.bounds.extents.z)).sqrMagnitude;
        float f3 = (-transform.position + hitPoint + hitNodeVector + new Vector3(col.bounds.extents.x, hitNodeVector.y, col.bounds.extents.z)).sqrMagnitude;

        float max = Mathf.Max(f0, f1, f2, f3);

        if (max == f0)
        {
            float length = (new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - (transform.position + new Vector3(-col.bounds.extents.x, 0f, -col.bounds.extents.z))).magnitude;
            return new Vector3(-col.bounds.extents.x, hitPoint.y - transform.position.y + length * hitNodeVector.y * 100f, -col.bounds.extents.z);
        }
        else if (max == f1)
        {
            float length = (new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - (transform.position + new Vector3(-col.bounds.extents.x, 0f, col.bounds.extents.z))).magnitude;
            return new Vector3(-col.bounds.extents.x, hitPoint.y - transform.position.y + length * hitNodeVector.y * 100f, col.bounds.extents.z);
        }
        else if (max == f2)
        {
            float length = (new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - (transform.position + new Vector3(col.bounds.extents.x, 0f, -col.bounds.extents.z))).magnitude;
            return new Vector3(col.bounds.extents.x, hitPoint.y - transform.position.y + length * hitNodeVector.y * 100f, -col.bounds.extents.z);
        }
        else if (max == f3)
        {
            float length = (new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - (transform.position + new Vector3(col.bounds.extents.x, 0f, col.bounds.extents.z))).magnitude;
            return new Vector3(col.bounds.extents.x, hitPoint.y - transform.position.y + length * hitNodeVector.y * 100f, col.bounds.extents.z);
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        ShowGizmos();   
    }

    private Vector3 Int3ToVector3(Int3 vector)
    {
        return new Vector3(vector.x / 1000f, vector.y / 1000f, vector.z / 1000f);
    }


    private void ShowGizmos()
    {
        Gizmos.color = _defaultColor;
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Gizmos.DrawCube(
            transform.position,
            new Vector3(
                boundingBox.x / Mathf.Abs(transform.lossyScale.x),
                boundingBox.y / Mathf.Abs(transform.lossyScale.y),
                boundingBox.z / Mathf.Abs(transform.lossyScale.z)
            )
        );
    }
}
