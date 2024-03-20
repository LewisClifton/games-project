using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class KeybindsMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset; // Assign this in the editor

    [SerializeField] private Button moveUpButton;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    // Call this method when the rebind button is clicked, passing the action to rebind
    public void StartRebinding(InputAction actionToRebind, int bindingId)
    {
        actionToRebind.Disable();

        // Cancel any existing rebind operation
        rebindingOperation?.Cancel();

        rebindingOperation = actionToRebind.PerformInteractiveRebinding(bindingId)  
            .WithCancelingThrough("<Keyboard>/escape") // Cancel rebind if Escape is pressed
            .OnMatchWaitForAnother(0.1f) // Wait for a short period to avoid accidental inputs
            .OnComplete(operation => FinishRebinding(operation, actionToRebind, bindingId))
            .OnCancel(operation => CancelRebinding(operation))
            .Start();
    
        actionToRebind.Enable();
    }

    public void DefaultRebind()
    {
        Debug.Log($"Rebinding to default");
    }

    private void FinishRebinding(InputActionRebindingExtensions.RebindingOperation operation, InputAction actionToRebind, int bindingId)
    {
        operation.Dispose();

        // Here you can update your UI to reflect the new binding
        Debug.Log($"Rebind Complete: {actionToRebind.bindings[bindingId].effectivePath}");
    }

    private void CancelRebinding(InputActionRebindingExtensions.RebindingOperation operation)
    {
        operation.Dispose();
        Debug.Log("Rebinding Cancelled");

        // Restore the original bindings if needed or update UI to reflect the cancellation
    }

    // Call this method to change the binding of an action
    // Input example: actionName = "Move", newBinding = "WASD/Up"
    // private void ChangeBinding(string actionName, Button specificButton, string bindingName, int bindingIndex = -1)
    // {
    //     Debug.Log($"Changing binding for {actionName} to {bindingName}");

    //     var split_actionName = actionName.Split('/');
    //     var found_action = actionAsset.FindAction(split_actionName[0]);
    //     if (split_actionName.Length > 1)
    //     {
    //         int subAcionts = found_action.actionMap.actions.Count;
    //         for (int i = 0; i < subAcionts; i++)
    //         {
    //             if (found_action.actionMap.actions[i].name == split_actionName[1])
    //             {
    //                 found_action = found_action.actionMap.actions[i];
    //                 break;
    //             }
    //         }
    //     }

    //     int bindingIntex = -1;
    //     for (int i = 0; i < found_action.bindings.Count; i++)
    //     {
    //         if (found_action.bindings[i].name == bindingName)
    //         {
    //             bindingIndex = i;
    //             break;
    //         }
    //     }
    //     Debug.Log($"found_action: {found_action}");
    //     Debug.Log($"Binding index: {bindingIndex}");
    //     string newBinding = StartRebinding(found_action, bindingIndex);
    //     Debug.Log($"New binding: {newBinding}");
    // }

    #region Main Buttons for Keybinds
    
    public void ChangeMoveUpBinding()
    {
        Debug.Log("Move Up button clicked");
        Debug.Log($"Current binding2: {actionAsset.FindAction("Move").bindings[2].effectivePath}");
        Debug.Log($"Current binding3: {actionAsset.FindAction("Move").bindings[3].effectivePath}");
        
        string action = "Move";
        int bindingIndex = 1;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
        Debug.Log($"New binding: {actionAsset.FindAction("Move").bindings[1].effectivePath}");
    }

    public void ChangeMoveDownBinding()
    {
        var currentBinding = actionAsset.FindAction("Move").bindings[2].effectivePath;
        // var moveAction = actionAsset.FindAction("Move");
        // ChangeBinding("Move", moveUpButton, "Keyboard/w", -1);
    }
    public void ChangeMoveLeftBinding()
    {
        var currentBinding = actionAsset.FindAction("Move").bindings[3].effectivePath;
        // var moveAction = actionAsset.FindAction("Move");
        // ChangeBinding("Move", moveUpButton, "Keyboard/w", -1);
    }

    public void ChangeMoveRightBinding()
    {
        var currentBinding = actionAsset.FindAction("Move").bindings[4].effectivePath;
        // var moveAction = actionAsset.FindAction("Move");
        // ChangeBinding("Move", moveUpButton, "Keyboard/w", -1);
    }

    #endregion
}
