using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Update()
    {
        PlayMapAnimation();
    }

    private void PlayMapAnimation()
    {
        var currentControllerType = ReInput.controllers.GetLastActiveController().type;
        if (currentControllerType == ControllerType.Joystick)
        {
            _animator.SetBool("Keyboard", false);
            _animator.SetBool("Joystick", true);
        }
        else
        {
            _animator.SetBool("Joystick", false);
            _animator.SetBool("Keyboard", true);
        }
    }
}
