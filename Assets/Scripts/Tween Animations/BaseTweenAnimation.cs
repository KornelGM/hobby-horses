using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseTweenAnimation : MonoBehaviour
{
    public Transform Target { get; private set; }

    public Transform ObjectToAnimation => _objectToAnimation;
    public float WaitingTime => _waitingTime;

    public bool DoKill => _doKill;
    public bool WaitBefore => _waitBefore;
    public bool Append => _append;
    public bool Join => _join;

    [SerializeField] protected Transform _objectToAnimation;
    [SerializeField] protected float _duration;
    [SerializeField] protected Ease _ease = Ease.Linear;
    [SerializeField] protected bool _local;

    [Space(10)]
    [SerializeField] private bool _doKill;

    [OnValueChanged("OnWaitBefore")]
    [SerializeField] private bool _waitBefore;

    [ShowIf("_waitBefore")]
    [SerializeField] private float _waitingTime = 0f;

    [Space(10)]
    [OnValueChanged("IsAppend")]
    [SerializeField] protected bool _append = true;

    [OnValueChanged("IsJoin")]
    [SerializeField] protected bool _join = false;

    public virtual Tween AnimationTween() { return null; }

    public void SetTarget(Transform target)
    {
        Target = target;
    }

    public void SetObjectToAnimation(Transform objectToAnimation)
    {
        _objectToAnimation = objectToAnimation;
    }

    [Button("Find Visuals")]
    private void SetVisualsAsObjectToAnimate()
    {
        VisualItemService visualItemService = transform.root.GetComponentInChildren<VisualItemService>(); 
        if (visualItemService != null)
        {
            _objectToAnimation= visualItemService.transform;
        }
    }

    private void IsAppend()
    {
        if (_append)
            _join = false;
        else
            _join = true;
    }

    private void IsJoin()
    {
        if (_join)
            _append = false;
        else
            _append = true;
    }
    
    private void OnWaitBefore()
    {
        if (!_waitBefore)
            _waitingTime = 0;
    }
}
