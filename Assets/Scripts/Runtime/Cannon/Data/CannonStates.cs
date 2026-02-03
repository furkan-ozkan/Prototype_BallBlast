using System;
using HappyInputs.Scripts.Core;
using HappyInputs.Scripts.Core.Data;
using HappyStates.Scripts.Core;
using Runtime.Cannon.CoreComponents;
using UnityEngine;

namespace Runtime.Cannon.Data
{
    public class CannonIdleState : BaseState
    {
        public event Action onNeedToChangeMoveState;
        
        private readonly UnityInputManager _inputManager;
        
        public CannonIdleState(UnityInputManager inputManager)
        {
            _inputManager = inputManager;
        }

        protected override void OnEnterState()
        {
            base.OnEnterState();
            
            _inputManager.OnInputPress += OnPress;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _inputManager.OnInputPress -= OnPress;
        }

        private void OnPress(InputEventData data)
        {
            if (data.actionName == "PressDown")
            {
                onNeedToChangeMoveState?.Invoke();
            }
        }
    }

    public class CannonMovingState : BaseState
    {
        public event Action onNeedToChangeIdleState;
        
        private readonly CannonMovement _movement;
        private readonly Transform _transform;
        private readonly UnityInputManager _inputManager;
        
        public CannonMovingState(
            CannonMovement movement,
            Transform cannonTransform,
            UnityInputManager inputManager
            )
        {
            _movement = movement;
            _transform = cannonTransform;
            _inputManager = inputManager;
        }

        protected override void OnEnterState()
        {
            base.OnEnterState();
            
            _inputManager.OnInputContinuous += OnDrag;
            _inputManager.OnInputRelease += OnRelease;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _movement.UpdatePosition(_transform);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _inputManager.OnInputContinuous -= OnDrag;
            _inputManager.OnInputRelease -= OnRelease;
        }

        private void OnDrag(InputEventData data)
        {
            if (data.actionName == "InputPosition")
            {
                _movement.SetTargetFromScreen(data.vector2Value, _transform.position.z);
            }
        }

        private void OnRelease(InputEventData data)
        {
            if (data.actionName == "PressUp")
            {
                onNeedToChangeIdleState?.Invoke();
            }
        }
    }
}