using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarChart : MonoBehaviour, IAwake, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }

    [SerializeField] private CanvasRenderer _chartRenderer;
    [SerializeField] private CanvasRenderer _backgroundRenderer;
    [SerializeField] private Material _chartRendererMaterial;
    [SerializeField] private Material _backgroundRendererMaterial;
    [SerializeField] private Image _chartBackground;

    private float _chartWidth;
    //private IngredientStats _ingredientStats;

    public void CustomAwake()
    {
        _chartWidth = _chartBackground.rectTransform.rect.width / 2;
    }

    public void UpdateRadarChart(/*IngredientStats ingredientStats*/ float x)
    {
        //_ingredientStats = ingredientStats;

        //if(_ingredientStats != null)
        //    UpdateRadarChart();
    }

    private void UpdateRadarChart()
    {
        Mesh chartMesh = new Mesh();
        Mesh backgroundMesh = new Mesh();
        float angleIncrement = 360; /// _ingredientStats.IngredientPoints.Length;

        //Vector3[] chartVertices = CreateVertex(angleIncrement);
        //Vector3[] backgroundVertices = CreateBackgroundVertex(angleIncrement);
        //Vector2[] uv = CreateUV();
        //List<int> triangles = CreateTriangles(chartVertices);

        //chartMesh.vertices = chartVertices;
        //chartMesh.uv = uv;
        //chartMesh.triangles = triangles.ToArray();

        //backgroundMesh.vertices = backgroundVertices;
        //backgroundMesh.uv = uv;
        //backgroundMesh.triangles = triangles.ToArray();

        _chartRenderer.SetMesh(chartMesh);
        _chartRenderer.SetMaterial(_chartRendererMaterial, null);

        _backgroundRenderer.SetMesh(backgroundMesh);
        _backgroundRenderer.SetMaterial(_backgroundRendererMaterial, null);

    }

    //private Vector3[] CreateVertex(float angleIncrement)
    //{
    //    Vector3[] vertices = new Vector3[_ingredientStats.IngredientPoints.Length + 1];

    //    vertices[0] = Vector3.zero;
    //    for (int i = 1; i < vertices.Length; i++)
    //    {
    //        vertices[i] = Quaternion.Euler(0, 0, -angleIncrement * (i - 1)) * 
    //            Vector3.up * _chartWidth * _ingredientStats.IngredientPoints[i - 1].PointValue.NormalizedValue();
    //    }

    //    return vertices;
    //}
    //    private Vector3[] CreateBackgroundVertex(float angleIncrement)
    //{
    //    Vector3[] vertices = new Vector3[_ingredientStats.IngredientPoints.Length + 1];

    //    vertices[0] = Vector3.zero;
    //    for (int i = 1; i < vertices.Length; i++)
    //    {
    //        vertices[i] = Quaternion.Euler(0, 0, -angleIncrement * (i - 1)) * Vector3.up * _chartWidth;
    //    }

    //    return vertices;
    //}

    private List<int> CreateTriangles(Vector3[] vertices)
    {
        List<int> triangles = new List<int>();

        for (int i = 0; i < vertices.Length - 1; i++)
        {
            int t1 = 0;
            int t2 = i + 1;
            int t3 = i + 2;

            if (i == vertices.Length - 2)
                t3 = 1;

            triangles.Add(t1);
            triangles.Add(t2);
            triangles.Add(t3);
        }

        return triangles;
    }

    //private Vector2[] CreateUV()
    //{
    //    Vector2[] uv = new Vector2[_ingredientStats.IngredientPoints.Length + 1];

    //    uv[0] = Vector2.zero;

    //    for (int i = 1; i < uv.Length; i++)
    //    {
    //        uv[i] = Vector2.one;
    //    }

    //    return uv;
    //}
}
