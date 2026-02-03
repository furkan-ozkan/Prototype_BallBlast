using HappyInputs.Scripts.Core.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HappyInputs.Scripts.Core
{
    public class InputTracker
    {
        private InputActionData data;
        private bool isPressed;
        private float pressTime;
        private float lastTapTime;
        private int tapCount;
        private bool holdTriggered;
        private float axisValue;
        private Vector2 vector2Value;
        private InputEventData pendingEvent;
    
        public bool IsPressed => isPressed;
        public float AxisValue => axisValue;
        public Vector2 Vector2Value => vector2Value;
    
        public InputTracker(InputActionData inputData)
        {
            data = inputData;
        }
    
        public void OnPerformed(InputAction.CallbackContext context)
        {
            isPressed = true;
            pressTime = Time.time;
            holdTriggered = false;
    
            // Read value based on action type
            if (context.action.expectedControlType == "Vector2")
            {
                vector2Value = context.ReadValue<Vector2>();
            }
            else if (context.action.expectedControlType == "Axis")
            {
                axisValue = context.ReadValue<float>();
            }
            else
            {
                axisValue = context.ReadValue<float>();
            }
    
            // Handle different input types
            switch (data.inputType)
            {
                case InputType.Press:
                    CreateEvent(InputType.Press, axisValue);
                    break;
    
                case InputType.Tap:
                case InputType.DoubleTap:
                    HandleTap();
                    break;
            }
        }
    
        public void OnCanceled(InputAction.CallbackContext context)
        {
            float pressDuration = Time.time - pressTime;
            isPressed = false;
    
            if (data.inputType == InputType.Release)
            {
                CreateEvent(InputType.Release, axisValue);
            }
    
            // Reset continuous values
            axisValue = 0f;
            vector2Value = Vector2.zero;
        }
    
        public void Update(float deltaTime)
        {
            if (!isPressed) return;
    
            // Handle hold
            if (data.inputType == InputType.Hold && !holdTriggered)
            {
                float pressDuration = Time.time - pressTime;
                if (pressDuration >= data.holdDuration)
                {
                    holdTriggered = true;
                    CreateEvent(InputType.Hold, axisValue);
                }
            }
    
            // Handle continuous
            if (data.inputType == InputType.Continuous)
            {
                CreateEvent(InputType.Continuous, axisValue);
            }
        }
    
        void HandleTap()
        {
            float timeSinceLastTap = Time.time - lastTapTime;
    
            if (data.inputType == InputType.DoubleTap)
            {
                if (timeSinceLastTap <= data.doubleTapWindow)
                {
                    tapCount++;
                    if (tapCount >= 2)
                    {
                        CreateEvent(InputType.DoubleTap, axisValue);
                        tapCount = 0;
                    }
                }
                else
                {
                    tapCount = 1;
                }
            }
            else if (data.inputType == InputType.Tap)
            {
                CreateEvent(InputType.Tap, axisValue);
            }
    
            lastTapTime = Time.time;
        }
    
        void CreateEvent(InputType type, float value)
        {
            pendingEvent = new InputEventData
            {
                actionName = data.actionName,
                type = type,
                value = value,
                vector2Value = vector2Value,
                isActive = isPressed
            };
        }
    
        public InputEventData GetEventData()
        {
            var evt = pendingEvent;
            pendingEvent = null;
            return evt;
        }
    }
    public enum InputType
    {
        Press,          // Single press
        Release,        // On release
        Hold,           // Hold for X seconds
        Tap,            // Quick tap
        DoubleTap,      // Double tap within time window
        Continuous      // Continuous (like axis movement)
    }
}