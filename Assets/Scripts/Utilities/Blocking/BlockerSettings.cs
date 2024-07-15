using System;

public class BlockerSettings
{
    public object Caller { get; protected set; }
    public Action OnBlockingBegin { get; protected set; } = null;
    public Action OnBlockingEnd { get; protected set; } = null;
}