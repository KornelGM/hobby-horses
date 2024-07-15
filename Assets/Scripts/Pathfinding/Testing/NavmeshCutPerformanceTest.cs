using UnityEngine;

public class NavmeshCutPerformanceTest : MonoBehaviour
{
    [SerializeField] private GameObject _cube;
    [SerializeField] private KeyCode _spawnCubesKey;

    [SerializeField]private int _size=20;
    private bool _cubesSpawned;

    void Update()
    {
        if (Input.GetKeyDown(_spawnCubesKey) && !_cubesSpawned)
        {
            SpawnCubes();
            _cubesSpawned = true;
        }
    }
    void SpawnCubes()
    {
        for (int i = -_size; i < _size; i++)
        {
            for (int j = -_size; j < _size; j++)
            {
                Instantiate(_cube, new Vector3(i * 2, 0, j * 2), Quaternion.identity);
            }
        }

    }
}
