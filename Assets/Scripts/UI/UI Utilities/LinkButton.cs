using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkButton : MonoBehaviour
{
    [SerializeField] private string _linkToOpen;

    public void OpenLink()
    {
        Application.OpenURL(_linkToOpen);
    }
}
