using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        return playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    public bool IsJumpPressed()
    {
        return playerInputActions.Player.Jump.triggered;
    }

    public bool IsWalkTogglePressed()
    {
        return playerInputActions.Player.WalkToggle.triggered;
    }

    public bool IsFreeDashPressed()
    {
        return playerInputActions.Player.Dash.triggered;
    }

    public bool IsAttackDashPressed()
    {
        return playerInputActions.Player.AttackDash.triggered;
    }
    public bool IsLockOnPressed()
    {
        return playerInputActions.Player.LockOn.triggered;
    }
}
