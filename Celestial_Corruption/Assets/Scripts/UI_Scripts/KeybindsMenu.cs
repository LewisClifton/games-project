using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class KeybindsMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset; // Assign this in the editor

    [SerializeField] private GameObject RebindingIndicatorText;

    [SerializeField] private Button[] MoveKeybindButtons;
    // [SerializeField] private Button[] OtherKeybindButtons;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    // On set active
    void OnEnable() 
    {
        RebindingIndicatorText.SetActive(false);
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        // Set text for Move keybinds
        if (MoveKeybindButtons.Length >= 4)
        {
            MoveKeybindButtons[0].transform.Find("KeybindText").GetComponent<TextMeshProUGUI>().text = Settings.moveUp.Split('/')[1];
            MoveKeybindButtons[1].transform.Find("KeybindText").GetComponent<TextMeshProUGUI>().text = Settings.moveDown.Split('/')[1];
            MoveKeybindButtons[2].transform.Find("KeybindText").GetComponent<TextMeshProUGUI>().text = Settings.moveLeft.Split('/')[1];
            MoveKeybindButtons[3].transform.Find("KeybindText").GetComponent<TextMeshProUGUI>().text = Settings.moveRight.Split('/')[1];
            // Similarly, update for other keybind buttons like Jump
        }
        else
        {
            Debug.LogError("MoveKeybindButtons does not have enough elements.");
        }

        // Set text for other keybinds
    }

    // Call this method when the rebind button is clicked, passing the action to rebind
    public void StartRebinding(InputAction actionToRebind, int bindingId)
    {
        actionToRebind.Disable();
        RebindingIndicatorText.SetActive(true);

        // Cancel any existing rebind operation
        rebindingOperation?.Cancel();

        rebindingOperation = actionToRebind.PerformInteractiveRebinding(bindingId)  
            .WithCancelingThrough("<Keyboard>/escape") // Cancel rebind if Escape is pressed
            .OnMatchWaitForAnother(0.1f) // Wait for a short period to avoid accidental inputs
            .OnComplete(operation => FinishRebinding(operation, actionToRebind, bindingId))
            .OnCancel(operation => CancelRebinding(operation, actionToRebind))
            .Start();
            
        Debug.Log($"Rebinding {actionToRebind.name} binding {bindingId}");
    }

    public void DefaultRebind()
    {
        Debug.Log($"Rebinding to default");
    }

    private void FinishRebinding(InputActionRebindingExtensions.RebindingOperation operation, InputAction actionToRebind, int bindingIndex)
    {
        operation.Dispose();

        // Extract the new binding path
        string newBindingPath = actionToRebind.bindings[bindingIndex].effectivePath;
        
        Debug.Log($"Rebinding finished. New binding path: {newBindingPath}");
        Debug.Log($"action name: {actionToRebind.name} binding index: {bindingIndex}");

        // Update the static Settings class with the new binding
        if (actionToRebind.name == "Move")
        {
            switch (bindingIndex)
            {
                case 1:
                    Settings.moveUp = newBindingPath;
                    break;
                case 2:
                    Settings.moveDown = newBindingPath;
                    break;
                case 3:
                    Settings.moveLeft = newBindingPath;
                    break;
                case 4:
                    Settings.moveRight = newBindingPath;
                    break;
                default:
                    Debug.LogWarning($"Unknown action rebind: {actionToRebind.name}");
                    break;
            }
        } else {
            switch (actionToRebind.name)
            {
                case "Jump":
                    Settings.jump = newBindingPath;
                    break;
                case "WalkToggle":
                    Settings.WalkToggle = newBindingPath;
                    break;
                case "Dash":
                    Settings.dash = newBindingPath;
                    break;
                case "AttackDash":
                    Settings.AttackDash = newBindingPath;
                    break;
                case "LockOn":
                    Settings.LockOn = newBindingPath;
                    break;
                default:
                    Debug.LogWarning($"Unknown action rebind: {actionToRebind.name}");
                    break;
            }
        }

        UpdateButtonText();
        RebindingIndicatorText.SetActive(false);
        actionToRebind.Enable();
}

    private void CancelRebinding(InputActionRebindingExtensions.RebindingOperation operation, InputAction actionToRebind)
    {
        operation.Dispose();
        RebindingIndicatorText.SetActive(false);
        actionToRebind.Enable();
        Debug.Log("Rebinding Cancelled");

        // Restore the original bindings if needed or update UI to reflect the cancellation
    }

    #region Main Buttons for Keybinds
    
    // Movement Rebinding Functions
    public void ChangeMoveUpBinding()
    {   
        string action = "Move";
        int bindingIndex = 1;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }

    public void ChangeMoveDownBinding()
    {
        string action = "Move";
        int bindingIndex = 2;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }
    public void ChangeMoveLeftBinding()
    {
        string action = "Move";
        int bindingIndex = 3;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }

    public void ChangeMoveRightBinding()
    {
        string action = "Move";
        int bindingIndex = 4;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }

    // Similarly, add functions for other keybinds
    public void ChangeJumpBinding()
    {
        Debug.Log($"Current Jump binding from ActionAsset: {actionAsset.FindAction("Jump").bindings[0].effectivePath}");
        string action = "Jump";
        int bindingIndex = 0;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }

    public void ChangeWalkToggleBinding()
    {
        Debug.Log($"Current Walk Toggle binding from ActionAsset: {actionAsset.FindAction("WalkToggle").bindings[0].effectivePath}");
        string action = "WalkToggle";
        int bindingIndex = 0;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }

    public void ChangeDashBinding()
    {
        Debug.Log($"Current Dash binding from ActionAsset: {actionAsset.FindAction("Dash").bindings[0].effectivePath}");
        string action = "Dash";
        int bindingIndex = 0;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }

    public void ChangeAttackDashBinding()
    {
        Debug.Log($"Current Attack Dash binding from ActionAsset: {actionAsset.FindAction("AttackDash").bindings[0].effectivePath}");
        string action = "AttackDash";
        int bindingIndex = 0;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }

    public void ChangeLockOnBinding()
    {
        Debug.Log($"Current Lock On binding from ActionAsset: {actionAsset.FindAction("LockOn").bindings[0].effectivePath}");
        string action = "LockOn";
        int bindingIndex = 0;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
    }

    #endregion
}
