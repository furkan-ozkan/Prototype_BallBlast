using System;
using System.Collections.Generic;
using HappyInputs.Scripts.Core;
using HappyStates.Scripts.Core;
using Runtime.Cannon.CoreComponents;
using UnityEngine;

namespace Runtime.Cannon.Data
{
    public class CannonStateFactory
    {
        private Dictionary<Type, BaseState> _states;

        public CannonStateFactory(
            CannonMovement movement,
            Transform transform,
            UnityInputManager inputManager
        )
        {
            _states = new Dictionary<Type, BaseState>
            {
                [typeof(CannonIdleState)] = new CannonIdleState(inputManager),
                [typeof(CannonMovingState)] = new CannonMovingState(movement, transform, inputManager)
            };
        }
        
        public TState GetState<TState>() where TState : BaseState
        {
            var type = typeof(TState);
            
            if (_states.TryGetValue(type, out var state))
            {
                return (TState)state;
            }

            throw new Exception($"State Not Found: {type.Name}");
        }
    }
}