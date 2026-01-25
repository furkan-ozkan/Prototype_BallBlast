using UnityEngine;

namespace HappyStates.Scripts.Core
{
    public abstract class BaseState : IState 
    {
        protected float StartTime;
        public float TimeInState => Time.time - StartTime;
        public void OnEnter() {
            StartTime = Time.time;
            OnEnterState();
        }
        
        protected virtual void OnEnterState() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnExit() { }
    }
    
    public abstract class BaseState<T> : BaseState where T : class
    {
        protected readonly T Owner;
        protected BaseState(T owner) => Owner = owner;
    }
}