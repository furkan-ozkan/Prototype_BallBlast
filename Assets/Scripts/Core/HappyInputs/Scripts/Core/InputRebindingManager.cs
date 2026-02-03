using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HappyInputs.Scripts.Core
{
    public class InputRebindingManager : MonoBehaviour
{
    public static InputRebindingManager Instance { get; private set; }

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartRebinding(string actionName, int bindingIndex, Action<bool> onComplete)
    {
        var inputData = UnityInputManager.Instance.inputActions.Find(x => x.actionName == actionName);
        if (inputData == null || inputData.actionReference == null)
        {
            onComplete?.Invoke(false);
            return;
        }

        var action = inputData.actionReference.action;
        action.Disable();

        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(operation =>
            {
                action.Enable();
                rebindOperation.Dispose();
                onComplete?.Invoke(true);
            })
            .OnCancel(operation =>
            {
                action.Enable();
                rebindOperation.Dispose();
                onComplete?.Invoke(false);
            })
            .Start();
    }

    public void CancelRebinding()
    {
        rebindOperation?.Cancel();
    }

    public string GetBindingDisplayString(string actionName, int bindingIndex)
    {
        var inputData = UnityInputManager.Instance.inputActions.Find(x => x.actionName == actionName);
        if (inputData == null || inputData.actionReference == null) return "";

        return inputData.actionReference.action.bindings[bindingIndex].ToDisplayString();
    }

    public void ResetBinding(string actionName, int bindingIndex)
    {
        var inputData = UnityInputManager.Instance.inputActions.Find(x => x.actionName == actionName);
        if (inputData == null || inputData.actionReference == null) return;

        var action = inputData.actionReference.action;
        action.RemoveBindingOverride(bindingIndex);
    }

    public void SaveBindings()
    {
        var inputAsset = UnityInputManager.Instance.inputActions[0].actionReference.asset;
        var rebinds = inputAsset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("InputRebinds", rebinds);
        PlayerPrefs.Save();
    }

    public void LoadBindings()
    {
        var rebinds = PlayerPrefs.GetString("InputRebinds", string.Empty);
        if (!string.IsNullOrEmpty(rebinds))
        {
            var inputAsset = UnityInputManager.Instance.inputActions[0].actionReference.asset;
            inputAsset.LoadBindingOverridesFromJson(rebinds);
        }
    }
}
}