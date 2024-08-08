using UnityEngine;

public class SpeedBoost : Boost
{
    [SerializeField] private float _boostValue;
    private HobbyHorseMovement _movement;

    public override void ActivateBoost()
    {
        if (_movement == null)
            return;

        _movement.IncreaseSetSpeed(_boostValue);
    }

    public override void InitializeBoost()
    {
        _playerManager.LocalPlayer.TryGetServiceLocatorComponent(out _movement);
    }
}
