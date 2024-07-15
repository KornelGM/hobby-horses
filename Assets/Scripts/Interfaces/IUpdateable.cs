using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdateable
{
    public bool Enabled { get;}

    public void CustomUpdate();
}

public interface IEarlyUpdate
{
    public void CustomEarlyUpdate();
}
