using System;
using HappyStates.Scripts.Core;
using Runtime.GameScene.State.States;
using UnityEngine;

namespace Runtime.GameScene.State.Manager
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] private StateMachine _gameStateMachine;
        private void Start()
        {
            _gameStateMachine.ChangeState(new MainMenuState(this));
        }
    }
}