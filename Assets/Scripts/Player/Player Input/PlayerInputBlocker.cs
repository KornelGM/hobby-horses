using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


public class PlayerInputBlocker : MonoBehaviour, IServiceLocatorComponent, IBlocker
{
    public ServiceLocator MyServiceLocator { get; set; }
    public Queue<BlockerSettings> BlockersQueue { get; set; } = new Queue<BlockerSettings>();
    [ServiceLocatorComponent] public InputManager InputManager;
    private PlayerInputReader _playerInputReader;

    private void Awake()
    {
        MyServiceLocator.TryGetServiceLocatorComponent(out _playerInputReader);
        InputManager = _playerInputReader.InputManager;
    }

    public void Block(InputBlockerSettings blockerSettings)
    {
        if (blockerSettings.PlayerInputStateOnBlock == null)
        {
            blockerSettings.PlayerInputStateOnBlock = new PlayerInputDefaultState(InputManager);
        }
        BlockersQueue.Enqueue(blockerSettings);
        if (BlockersQueue.Count == 1)
        {
            _playerInputReader.SwitchPlayerInputState(blockerSettings.PlayerInputStateOnBlock);
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

    private void ConsumeCurrentBlocker()
    {
        InputBlockerSettings blocker = (InputBlockerSettings)BlockersQueue.Dequeue();
        blocker.OnBlockingEnd?.Invoke();
        if (BlockersQueue.Count == 0)
        {
            _playerInputReader.GoToGameplayState();
            return;
        }

        InputBlockerSettings currentBlocker = (InputBlockerSettings)BlockersQueue.Peek();
        currentBlocker.OnBlockingBegin?.Invoke();
        _playerInputReader.SwitchPlayerInputState(currentBlocker.PlayerInputStateOnBlock);
    }
}