using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAnimationsContainer : MonoBehaviour
{
    public BaseTweenAnimation[] TweenAnimations => _tweenAnimations;
    [SerializeField] private BaseTweenAnimation[] _tweenAnimations;
}
