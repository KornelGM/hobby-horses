using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class RaceTrackGuide : MonoBehaviour
{
    [SerializeField] private Track _track;
    [SerializeField] private TrackGuide _guidePrefab;

    float _angle;

    private List<TrackGuide> _spawnedGuides = new();

    public void InitializeTrackGuide()
    {
        for (int i = 0; i < _track.AllPositions.Count; i++)
        {
            if(i < _track.AllPositions.Count - 1)
            {
                Vector3 direction = _track.AllPositions[i + 1] - _track.AllPositions[i];
                _angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            }

            TrackGuide guide = Instantiate(_guidePrefab, _track.AllPositions[i], Quaternion.Euler(0, _angle, 0));
            guide.transform.SetParent(transform);
            _spawnedGuides.Add(guide);
        }
    }

    public void SetGuidesActive(bool next = false)
    {
        foreach (var guide in _spawnedGuides)
        {
            guide.SetTrackGuide(next);
        }
    }

    public void SetGuidesDeactivated()
    {
        foreach (var guide in _spawnedGuides)
        {
            guide.DeactivatedTrackGuide();
        }
    }

    [Button("Test")]
    private void Test()
    {
        InitializeTrackGuide();
    }
}
