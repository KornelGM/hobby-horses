using System;
public interface IControllable 
{
    public Action OnControllingStarted {get; set;}
    public Action OnControllingFinished {get; set;}
    public Action OnControllingInterrupted {get; set;}
    public Action OnPlayerUnlocked {get; set;}
    public void ControlObject(IVirtualController controller);
    public void OnControllingEntered(IVirtualController controller);
    public void OnControllingExited(IVirtualController controller);
}
