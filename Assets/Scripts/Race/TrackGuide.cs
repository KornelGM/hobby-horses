using Sirenix.OdinInspector;
using UnityEngine;

public class TrackGuide : MonoBehaviour
{
    [SerializeField] private bool _sprite;
    [SerializeField, ShowIf(nameof(_sprite))] private SpriteRenderer _spriteRenderer;
    [SerializeField, HideIf(nameof(_sprite))] private MeshRenderer _meshRenderer;

    [SerializeField, FoldoutGroup("Sprite"), ShowIf(nameof(_sprite))] private Material _deactivatedSprite;
    [SerializeField, FoldoutGroup("Sprite"), ShowIf(nameof(_sprite))] private Material _currentSprite;
    [SerializeField, FoldoutGroup("Sprite"), ShowIf(nameof(_sprite))] private Material _nextSprite;

    [SerializeField, FoldoutGroup("Mesh"), HideIf(nameof(_sprite))] private Material _deactivatedMesh;
    [SerializeField, FoldoutGroup("Mesh"), HideIf(nameof(_sprite))] private Material _currentMesh;
    [SerializeField, FoldoutGroup("Mesh"), HideIf(nameof(_sprite))] private Material _nextMesh;

    public void SetTrackGuide(bool next = false)
    {
        if (_sprite)
        {
            _spriteRenderer.material = next ? _nextSprite : _currentSprite;
        }
        else
        {
            _meshRenderer.material = next ? _nextMesh : _currentMesh;
        }
    }

    public void DeactivatedTrackGuide()
    {
        if (_sprite)
        {
            _spriteRenderer.material = _deactivatedSprite;
        }
        else
        {
            _meshRenderer.material = _deactivatedMesh;
        }
    }
}
