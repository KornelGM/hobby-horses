
using System.Collections.Generic;
using JetBrains.Annotations;

public interface IBlocker
{
    public Queue<BlockerSettings> BlockersQueue { get; set; }

    public bool IsBlocked => BlockersQueue.Count > 0;

    public void Block(InputBlockerSettings blockerSettings)
    {
        BlockersQueue.Enqueue(blockerSettings);
        if (BlockersQueue.Count == 1)
        {
            blockerSettings.OnBlockingBegin?.Invoke();
        }
    }
    public bool TryUnblock([CanBeNull] object caller = null)
    {
        if (BlockersQueue.Count == 0)
        {
            return false;
        }

        if (caller != null)
        {
            if (caller != BlockersQueue.Peek().Caller)
            {
                return false;
            }
        }

        ConsumeCurrentBlocker();
        return true;
    }
    public void ConsumeCurrentBlocker()
    {
        BlockerSettings blocker = BlockersQueue.Dequeue();
        blocker.OnBlockingEnd?.Invoke();
        if (BlockersQueue.Count == 0)
        {
            return;
        }

        BlockerSettings currentBlocker = BlockersQueue.Peek();
        currentBlocker.OnBlockingBegin?.Invoke();
    }

}
