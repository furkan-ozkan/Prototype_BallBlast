using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HappyStates.Scripts.Core
{
    // Geçmişteki her bir kaydı veri olarak tutan yapı
    public struct StateHistoryLog
    {
        public string Duration;
        public string FromState;
        public string ToState;
        public string Logic;
        public string FilePath;
        public int LineNumber;

        public override string ToString() => $"[{Duration}s] {FromState} -> {ToState} | Rule: {Logic}";
    }

    public class StateMachine : MonoBehaviour 
    {
        public IState CurrentState { get; private set; }
        private Dictionary<Type, List<Transition>> _transitions = new();
        private List<Transition> _currentTransitions = new();
        private List<Transition> _anyTransitions = new();
    
        // Sadece yazı değil, veri tutan liste
        public List<StateHistoryLog> History { get; } = new List<StateHistoryLog>();
        public static List<StateMachine> AllMachines = new List<StateMachine>();

        void OnEnable() => AllMachines.Add(this);
        void OnDisable() => AllMachines.Remove(this);

        void Update() 
        {
            var t = GetTransition();
            if (t != null) 
                ChangeState(t.To, t); // Geçişi tüm verisiyle (t) gönderiyoruz
            
            CurrentState?.OnUpdate();
        }

        private Transition GetTransition() 
        {
            foreach (var t in _anyTransitions) if (t.Condition()) return t;
            foreach (var t in _currentTransitions) if (t.Condition()) return t;
            return null;
        }

        public void ChangeState(IState newState, Transition transition = null) 
        {
            if (newState == CurrentState) return;

            // 1. Süre Hesapla
            string duration = "0.00";
            if (CurrentState is BaseState baseState) 
                duration = baseState.TimeInState.ToString("F2");

            string fromStateName = CurrentState?.GetType().Name ?? "Entry";
            string toStateName = newState.GetType().Name;

            // 2. Geçmişi Veri Olarak Kaydet (Vizyon burası)
            var log = new StateHistoryLog
            {
                Duration = duration,
                FromState = fromStateName,
                ToState = toStateName,
                Logic = transition != null ? transition.LogicString : "Manual/Direct",
                FilePath = transition?.FilePath ?? "",
                LineNumber = transition?.LineNumber ?? 0
            };
            
            UpdateHistory(log);

            // 3. State Değişimi
            CurrentState?.OnExit();
            CurrentState = newState;

            _transitions.TryGetValue(CurrentState.GetType(), out _currentTransitions);
            _currentTransitions ??= new List<Transition>();

            CurrentState.OnEnter();
        }
        
        // NORMAL GEÇİŞ: Dosya ve satırı otomatik yakalar
        public void AddTransition(IState from, IState to, Expression<Func<bool>> condExpr, 
            [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) 
        {
            if (!_transitions.TryGetValue(from.GetType(), out var list))
                _transitions[from.GetType()] = list = new List<Transition>();
            
            list.Add(new Transition(to, condExpr, path, line));
        }

        // ANY GEÇİŞ: Bunu da unutmadık, dosya ve satırı otomatik yakalar
        public void AddAnyTransition(IState to, Expression<Func<bool>> condExpr, 
            [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            _anyTransitions.Add(new Transition(to, condExpr, path, line));
        }
        
        private void UpdateHistory(StateHistoryLog logEntry) 
        {
            if (History.Count >= 10) History.RemoveAt(0);
            History.Add(logEntry);
        }
    }
}