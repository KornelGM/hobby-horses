using System;

public class InputBlockerSettings : BlockerSettings
{
    public State PlayerInputStateOnBlock { get; set; }

    public InputBlockerSettings(object caller,
        State playerInputStateOnBlock = null, Action onBlockingBegin = null, Action onBlockingEnd = null)
    {
        Caller = caller;
        PlayerInputStateOnBlock = playerInputStateOnBlock;
        OnBlockingBegin = onBlockingBegin;
        OnBlockingEnd = onBlockingEnd;
    }
}