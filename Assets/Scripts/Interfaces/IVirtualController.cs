using System;
using UnityEngine;

public interface IVirtualController
{
    public Vector3 Movement { get; set; }
    public Vector2 Mouse { get; set; }
    public bool IsSprint { get; set; }
    public bool IsWalk { get; set; }
    public Action OnJumpPerformed { get; set; }
    public Action OnFirstInteractionPerformed { get; set; }
    public Action OnSecondInteractionPerformed { get; set; }
    public Action OnAdditiveInteractionPerformed { get; set; }
    public Action OnMoreInfoInteractionPerformed { get; set; }
    public Action OnFirstInteractionCancelled { get; set; }
    public Action OnSecondInteractionCancelled { get; set; }
    public Action OnAdditiveInteractionCancelled { get; set; }
    public Action OnMoreInfoInteractionCancelled { get; set; }

    public float HologramRotatingAxis { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action OnToggleHologramGridSnappingPerformed {  get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
    public Action OnPausePerformed  { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action OnUnpausePerformed  { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Action OnOpenBookPerformed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action OnCloseBookPerformed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Action OnSetInventory0 { get; set; }
    public Action OnSetInventory1 { get; set; }
    public Action OnSetInventory2 { get; set; }
    public Action OnSetInventory3 { get; set; }
    public Action OnSetInventory4 { get; set; }
    public Action OnSetInventory5 { get; set; }
    public Action OnSetInventory6 { get; set; }
    public Action OnSetInventory7 { get; set; }
    public Action OnSetInventory8 { get; set; }
    public Action OnSetInventory9 { get; set; }
    public Action OnScrollInventoryUp { get; set; }
    public Action OnScrollInventoryDown { get; set; }

    public Vector3 DesignMovement { get; set; }
    public Vector2 DesignMouse { get; set; }

    public Action DesignOnFirstInteractionPerformed { get; set; }
    public Action DesignOnSecondInteractionPerformed { get; set; }
    public Action DesignCancel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
