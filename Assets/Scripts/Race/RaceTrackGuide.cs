using Sirenix.OdinInspector;
using UnityEngine;

public class RaceTrackGuide : MonoBehaviour
{
    [SerializeField] private Track _track;
    [SerializeField] private GameObject _guidePrefab;

    float _angle;

    private void OnEnable()
    {
        InitializeTrackGuide();
    }

    private void InitializeTrackGuide()
    {
        for (int i = 0; i < _track.AllPositions.Count; i++)
        {
            if(i < _track.AllPositions.Count - 1)
            {
                Vector3 direction = _track.AllPositions[i + 1] - _track.AllPositions[i];
                _angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            }

            GameObject guide = Instantiate(_guidePrefab, _track.AllPositions[i], Quaternion.Euler(0, _angle, 0));
            guide.transform.SetParent(transform);
        }
    }

    [Button("Test")]
    private void Test()
    {
        InitializeTrackGuide();
    }
}
