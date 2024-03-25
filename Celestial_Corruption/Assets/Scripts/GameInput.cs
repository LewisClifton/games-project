using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    public InputActionAsset actionAsset;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Debug.Log($"Move input is set to {playerInputActions.Player.Move.bindings[0].effectivePath}");
        return playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    public bool IsJumpPressed()
    {
        Debug.Log($"Jump input is set to {playerInputActions.Player.Jump.bindings[0].effectivePath}");
        Debug.Log($"Jump input is set to2 {actionAsset.FindAction("Jump").bindings[0].effectivePath}");
        return actionAsset.FindAction("Jump").triggered;
        // return playerInputActions.Player.Jump.triggered;
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

    public bool IsEscapePressed()
    {
        return playerInputActions.Player.Escape.triggered;
    }
    
    public bool IsLockOnPressed()
    {
        return playerInputActions.Player.LockOn.triggered;
    }
}
