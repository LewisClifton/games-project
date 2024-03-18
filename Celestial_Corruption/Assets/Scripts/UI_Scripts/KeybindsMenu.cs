using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindsMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset; // Assign this in the editor

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    // Call this method when the rebind button is clicked, passing the action to rebind
    public void StartRebinding(InputAction actionToRebind, int bindingId)
    {
        // Cancel any existing rebind operation
        rebindingOperation?.Cancel();

        rebindingOperation = actionToRebind.PerformInteractiveRebinding(bindingId)  
            .WithCancelingThrough("<Keyboard>/escape") // Cancel rebind if Escape is pressed
            .OnMatchWaitForAnother(0.1f) // Wait for a short period to avoid accidental inputs
            .OnComplete(operation => FinishRebinding(operation, actionToRebind))
            .OnCancel(operation => CancelRebinding(operation))
            .Start();
    }

    private void FinishRebinding(InputActionRebindingExtensions.RebindingOperation operation, InputAction actionToRebind)
    {
        operation.Dispose();

        // Here you can update your UI to reflect the new binding
        Debug.Log($"Rebind Complete: {actionToRebind.bindings[0].effectivePath}");

        // Optionally, save the bindings here
    }

    private void CancelRebinding(InputActionRebindingExtensions.RebindingOperation operation)
    {
        operation.Dispose();
        Debug.Log("Rebinding Cancelled");

        // Restore the original bindings if needed or update UI to reflect the cancellation
    }

    // Call this method to change the binding of an action
    // Input example: actionName = "Move", newBinding = "WASD/Up"
    private void ChangeBinding(string actionName, string newBinding)
    {
        var action = actionAsset.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"Action {actionName} not found");
            return;
        }

        int bindingIndex = -1;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].name == bindingName)
            {
                bindingIndex = i;
                break;
            }
        }

        // Find the action by name
        InputAction action = actionAsset.FindAction(actionName);

        // Create a new binding with the new input string
        InputBinding newInputBinding = new InputBinding {overridePath = newBinding};

        // Replace the first binding with the new one
        action.ApplyBindingOverride(0, newInputBinding);
    }
}
