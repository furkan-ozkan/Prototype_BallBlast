using HappyInputs.Scripts.Core;
using HappyStates.Scripts.Core;
using Runtime.Cannon.CoreComponents;
using Runtime.Cannon.Data;
using UnityEngine;

namespace Runtime.Cannon
{
    public class CannonController : MonoBehaviour
    {
        [SerializeField] private CannonSettings _settings;

        private CannonMovement _movement;
        private StateMachine _stateMachine;
        private CannonStateFactory _stateFactory;

        private void Awake()
        {
            
        }

        private void Start()
        {
            _movement = new CannonMovement(_settings, Camera.main);
            _stateMachine = GetComponent<StateMachine>();
            _stateFactory = new CannonStateFactory(_movement,transform, UnityInputManager.Instance);
            
            _stateFactory.GetState<CannonIdleState>().onNeedToChangeMoveState += ChangeToMovingState;
            _stateFactory.GetState<CannonMovingState>().onNeedToChangeIdleState += ChangeToIdleState;
            
            _stateMachine.ChangeState(_stateFactory.GetState<CannonIdleState>());
        }
        

        public void ChangeToIdleState()
        {
            _stateMachine.ChangeState(_stateFactory.GetState<CannonIdleState>());
        }

        public void ChangeToMovingState()
        {
            _stateMachine.ChangeState(_stateFactory.GetState<CannonMovingState>());
        }
    }
}