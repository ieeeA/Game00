using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class FSM<TContext> where TContext : class
    {
        private TContext context;
        private IFSMState<TContext> currentState;
        private Dictionary<string, IFSMState<TContext>> states = new Dictionary<string, IFSMState<TContext>>();

        public FSM(TContext context)
        {
            this.context = context;
        }

        public FSM()
        {
            this.context = null;
        }

        public void Start()
        {
            currentState.Enter(context);
        }

        public void Update()
        {
            currentState.Update(context);
            string? targetName = currentState.CheckTransit(context);
            if (targetName != null)
            {
                if (states.ContainsKey(targetName))
                {
                    currentState.Exit(context);
                    currentState = states[targetName];
                    currentState.Enter(context);
                }
                else
                {
                    Debug.LogError($"[FSM] ëJà⁄êÊ:{targetName}");
                }
            }
        }

        public void SetCurrentState(string stateName, bool callEnter = false)
        {
            currentState = states[stateName];
            if (callEnter)
            {
                currentState.Enter(context);
            }
        }

        public void AddState(FSMState<TContext> state)
        {
            states.Add(state.Name, state);
        }

        public string GetCurrentStateName()
        {
            return currentState.Name;
        }
    }

    interface IFSMState<TContext> where TContext : class
    {
        string Name { get; }
        void Enter(TContext context);
        void Update(TContext context);
        void Exit(TContext context);
        string? CheckTransit(TContext context);
    }

    public class FSMState<TContext> : IFSMState<TContext> where TContext : class
    {
        public string Name { get; }
        public event Action<TContext> OnEnter;
        public event Action<TContext> OnUpdate;
        public event Action<TContext> OnExit;
        
        private List<FSMTransition<TContext>> transitions = new List<FSMTransition<TContext>>();

        public FSMState(string name)
        {
            this.Name = name;
        }

        public void AddTransition(FSMTransition<TContext> transition)
        {
            transitions.Add(transition);
        }

        public void AddTransition(string targetName, Func<TContext, bool> condition, int priority = -1)
        {
            transitions.Add(new FSMTransition<TContext>(targetName, condition, priority));
        }

        void IFSMState<TContext>.Enter(TContext context)
        {
            OnEnter?.Invoke(context);
        }

        void IFSMState<TContext>.Update(TContext context)
        {
            OnUpdate?.Invoke(context);
        }

        void IFSMState<TContext>.Exit(TContext context)
        {
            OnExit?.Invoke(context);
        }

        string? IFSMState<TContext>.CheckTransit(TContext context)
        {
            if (transitions.Count == 0) return null;

            return transitions.Where(t => t.Condition.Invoke(context))
                .OrderBy(t => t.Priority)
                .FirstOrDefault()?.TargetName;
        }
    }

    public class FSMTransition<TContext> where TContext : class
    {
        public string TargetName { get; }
        public Func<TContext, bool> Condition { get; }
        public int Priority { get; }

        public FSMTransition(string targetName, Func<TContext, bool> condition, int priority = -1)
        {
            this.TargetName = targetName;
            this.Condition = condition;
            this.Priority = priority;
        }
    }
}
