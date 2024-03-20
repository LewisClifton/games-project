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

    [SerializeField] private Button[] MoveKeybindButtons;
    // [SerializeField] private Button[] OtherKeybindButtons;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    // On set active
    void OnEnable() 
    {
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        // Set text for Move keybinds
        for (int i = 0; i < MoveKeybindButtons.Length; i++)
        {
            MoveKeybindButtons[i].transform.Find("KeybindText").GetComponent<TextMeshProUGUI>().text = actionAsset.FindAction("Move").bindings[i+1].path.Split('/')[1];
            Debug.Log($"Move Keybind {i+1}: {actionAsset.FindAction("Move").bindings[i+1].effectivePath}");
        }

        // Set text for other keybinds
    }

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

    private void ChangeButtonText(Button button, string newText)
    {
        button.transform.Find("KeybindText").GetComponent<TextMeshProUGUI>().text = newText;
    }

    #region Main Buttons for Keybinds
    
    // Movement Rebinding Functions
    public void ChangeMoveUpBinding()
    {   
        string action = "Move";
        int bindingIndex = 1;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
        
        UpdateButtonText();
        Debug.Log($"New binding: {actionAsset.FindAction("Move").bindings[1].effectivePath}");
    }

    public void ChangeMoveDownBinding()
    {
        string action = "Move";
        int bindingIndex = 2;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
        
        UpdateButtonText();
        Debug.Log($"New binding: {actionAsset.FindAction("Move").bindings[2].effectivePath}");
    }
    public void ChangeMoveLeftBinding()
    {
        string action = "Move";
        int bindingIndex = 3;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
        
        UpdateButtonText();
        Debug.Log($"New binding: {actionAsset.FindAction("Move").bindings[3].effectivePath}");
    }

    public void ChangeMoveRightBinding()
    {
        string action = "Move";
        int bindingIndex = 4;

        StartRebinding(actionAsset.FindAction(action), bindingIndex);
        
        UpdateButtonText();
        Debug.Log($"New binding: {actionAsset.FindAction("Move").bindings[1].effectivePath}");
    }

    #endregion
}
