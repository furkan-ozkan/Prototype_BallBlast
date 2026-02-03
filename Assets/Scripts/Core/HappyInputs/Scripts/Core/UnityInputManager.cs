using System;
using System.Collections.Generic;
using HappyInputs.Scripts.Core.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HappyInputs.Scripts.Core
{
    public class UnityInputManager : MonoBehaviour
{
    public static UnityInputManager Instance { get; private set; }

    [Header("Input Actions")]
    public List<InputActionData> inputActions = new List<InputActionData>();

    // Events for each input type
    public event Action<InputEventData> OnInputPress;
    public event Action<InputEventData> OnInputRelease;
    public event Action<InputEventData> OnInputHold;
    public event Action<InputEventData> OnInputTap;
    public event Action<InputEventData> OnInputDoubleTap;
    public event Action<InputEventData> OnInputContinuous;

    // Internal tracking
    private Dictionary<string, InputTracker> inputTrackers = new Dictionary<string, InputTracker>();

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeInputs();
    }

    void InitializeInputs()
    {
        foreach (var inputData in inputActions)
        {
            if (inputData.actionReference == null) continue;

            var tracker = new InputTracker(inputData);
            inputTrackers[inputData.actionName] = tracker;

            // Subscribe to input events
            var action = inputData.actionReference.action;
            
            action.performed += ctx => OnActionPerformed(inputData.actionName, ctx);
            action.canceled += ctx => OnActionCanceled(inputData.actionName, ctx);
            action.Enable();
        }
    }

    void Update()
    {
        // Update all trackers
        foreach (var tracker in inputTrackers.Values)
        {
            tracker.Update(Time.deltaTime);
            ProcessTrackerEvents(tracker);
        }
    }

    void OnActionPerformed(string actionName, InputAction.CallbackContext context)
    {
        if (inputTrackers.TryGetValue(actionName, out var tracker))
        {
            tracker.OnPerformed(context);
        }
    }

    void OnActionCanceled(string actionName, InputAction.CallbackContext context)
    {
        if (inputTrackers.TryGetValue(actionName, out var tracker))
        {
            tracker.OnCanceled(context);
        }
    }

    void ProcessTrackerEvents(InputTracker tracker)
    {
        var eventData = tracker.GetEventData();
        if (eventData == null) return;

        switch (eventData.type)
        {
            case InputType.Press:
                OnInputPress?.Invoke(eventData);
                break;
            case InputType.Release:
                OnInputRelease?.Invoke(eventData);
                break;
            case InputType.Hold:
                OnInputHold?.Invoke(eventData);
                break;
            case InputType.Tap:
                OnInputTap?.Invoke(eventData);
                break;
            case InputType.DoubleTap:
                OnInputDoubleTap?.Invoke(eventData);
                break;
            case InputType.Continuous:
                OnInputContinuous?.Invoke(eventData);
                break;
        }
    }

    // Public API for getting input state
    public bool GetButton(string actionName)
    {
        return inputTrackers.TryGetValue(actionName, out var tracker) && tracker.IsPressed;
    }

    public float GetAxis(string actionName)
    {
        return inputTrackers.TryGetValue(actionName, out var tracker) ? tracker.AxisValue : 0f;
    }

    public Vector2 GetVector2(string actionName)
    {
        return inputTrackers.TryGetValue(actionName, out var tracker) ? tracker.Vector2Value : Vector2.zero;
    }

    void OnDestroy()
    {
        // Cleanup
        foreach (var inputData in inputActions)
        {
            if (inputData.actionReference != null)
            {
                inputData.actionReference.action.Disable();
            }
        }
    }
}
}