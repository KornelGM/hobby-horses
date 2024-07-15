using System.Collections;
using System.Collections.Generic;

public abstract class State
{
    public abstract void Enter();
    public abstract void CustomUpdate();
    public abstract void CustomFixedUpdate();
    public abstract void Exit();
}

