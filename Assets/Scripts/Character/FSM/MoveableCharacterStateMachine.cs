using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class MoveableCharacterStateMachine : StateMachine, IServiceLocatorComponent, IStartable
{
    public ServiceLocator MyServiceLocator { get; set; }

    [ServiceLocatorComponent] protected TimeManager _timeManager;
    public TimeManager TimeManager => _timeManager;

    [ReadOnly] public Vector3 Velocity;

    [field: SerializeField] public Animator Animator { get; protected set; }
    [field: SerializeField] public CharacterController Controller { get; protected set; }
    [field: SerializeField] public MovementSettings MovementSettings { get; protected set; }
    [field: SerializeField] public MovementSmoothingSettings SmoothingSettings {get; protected set;}
    public Vector3 VelocityXZ => new(Velocity.x, 0f, Velocity.z);
    public IVirtualController VirtualController { get; protected set; }

    public float maxSpeed {get; private set;}



    public virtual void CustomStart()
    {
        MyServiceLocator.TryGetServiceLocatorComponent(out IVirtualController virtualController);
        
        VirtualController = virtualController;

        //Animator.IsNotNull(this);
        Controller.IsNotNull(this, nameof(Controller));
        MovementSettings.IsNotNull(this, nameof(MovementSettings));
        maxSpeed=MovementSettings.MovementSpeed*MovementSettings.SprintAnimatorMomenentSpeed;
    }

    public void SetAnimatorMovementParameters(Vector3 direction, float actualSpeed)
    { 
        //TODOIGOR remove this after root motion is finished
        if(Animator==null)return;
        Vector2 dirVect = new Vector2(direction.x,direction.z);
        float speedParam = actualSpeed/maxSpeed;
        Vector3 forward = transform.forward;
        float xMove = Mathf.Clamp(Vector2.SignedAngle(dirVect,
                                    new Vector2(forward.x,forward.z))/180*SmoothingSettings.TurnSpeed, -1f, 1f);
        Animator.SetFloat("XMove", xMove, 0.1f, TimeManager.GetDeltaTime());
        Animator.SetFloat("ZMove",
                            speedParam*(1-SmoothingSettings.TurnCurve.Evaluate(Mathf.Abs(xMove*2)))
                            ,0.1f,TimeManager.GetDeltaTime());
    }

}
