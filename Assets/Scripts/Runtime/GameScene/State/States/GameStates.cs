using HappyStates.Scripts.Core;
using Runtime.GameScene.State.Manager;

namespace Runtime.GameScene.State.States
{
    public class MainMenuState : BaseState<GameStateManager>
    {
        public MainMenuState(GameStateManager owner) : base(owner)
        {
        }
        protected override void OnEnterState()
        {
            
        }
    }
    public class BeforeLevelState : BaseState<GameStateManager>
    {
        public BeforeLevelState(GameStateManager owner) : base(owner)
        {
        }
        protected override void OnEnterState()
        {
            
        }
    }
    public class InLevelState : BaseState<GameStateManager>
    {
        public InLevelState(GameStateManager owner) : base(owner)
        {
        }
        protected override void OnEnterState()
        {
            
        }
    }
    public class PauseState : BaseState<GameStateManager>
    {
        public PauseState(GameStateManager owner) : base(owner)
        {
        }
        protected override void OnEnterState()
        {
            
        }
    }
    public class SuccessState : BaseState<GameStateManager>
    {
        public SuccessState(GameStateManager owner) : base(owner)
        {
        }
        protected override void OnEnterState()
        {
            
        }
    }
    public class FailedState : BaseState<GameStateManager>
    {
        public FailedState(GameStateManager owner) : base(owner)
        {
        }
        protected override void OnEnterState()
        {
            
        }
    }
    public class AfterLevelState : BaseState<GameStateManager>
    {
        public AfterLevelState(GameStateManager owner) : base(owner)
        {
        }
        protected override void OnEnterState()
        {
            
        }
    }
}