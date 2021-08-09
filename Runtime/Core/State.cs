﻿using System;
using System.Collections.Generic;
using UnityEngine;
using static SAS.StateMachineGraph.Actor;

namespace SAS.StateMachineGraph
{
    public class State
    {
        public string Name { get; private set; }

        private StateMachine _stateMachine;
        internal IStateAction[] _onEnter = default;
        internal IStateAction[] _onExit = default;
        internal IStateAction[] _onFixedUpdate = default;
        internal IStateAction[] _onUpdate = default;
        internal IStateAction[] _onLateUpdate = default;
        private List<IAwaitableStateAction> _awaitableStateAction = new List<IAwaitableStateAction>();
        internal TransitionState[] _transitionStates;

        private State _nextState;

        internal State(StateMachine stateMachine, string name)
        {
            _stateMachine = stateMachine;
            Name = name;
        }

        internal void OnEnter()
        {
            FilterAwaitableAction(_onEnter);
            for (int i = 0; i < _onEnter?.Length; ++i)
                _onEnter[i].Execute(_stateMachine.Actor);
        }

        internal void OnExit()
        {
            FilterAwaitableAction(_onExit);
            for (int i = 0; i < _onExit?.Length; ++i)
                _onExit[i].Execute(_stateMachine.Actor);
        }

        internal void OnFixedUpdate()
        {
            for (int i = 0; i < _onFixedUpdate?.Length; ++i)
                _onFixedUpdate[i].Execute(_stateMachine.Actor);
        }

        internal void OnUpdate()
        {
            for (int i = 0; i < _onUpdate?.Length; ++i)
                _onUpdate[i].Execute(_stateMachine.Actor);
        }

        internal void OnLateUpdate()
        {
            for (int i = 0; i < _onLateUpdate?.Length; ++i)
                _onLateUpdate[i].Execute(_stateMachine.Actor);
        }

        internal void TryTransition(StateChanged stateChanged)
        {
            if (_nextState == null)
            {
                for (int i = 0; i < _transitionStates.Length; ++i)
                {
                    if (_transitionStates[i].TryGetTransiton(_stateMachine, out _nextState))
                    {
                        ResetExitTime();
                        break;
                    }
                }
            }

            if (_nextState != null && IsAllAwaitableActionCompleted())
            {
                stateChanged.Invoke(_stateMachine.CurrentState, false);
                _stateMachine.CurrentState = _nextState;
                stateChanged?.Invoke(_nextState, true);
                _nextState = null;
            }
        }

        private void ResetExitTime()
        {
            for (int i = 0; i < _transitionStates.Length; ++i)
                _transitionStates[i].TimeElapsed = 0;
        }

        private bool IsAllAwaitableActionCompleted()
        {
            for (int i = 0; i < _awaitableStateAction.Count; ++i)
            {
                if (!_awaitableStateAction[i].IsCompleted)
                    return false;
            }
            return true;
        }

        private void FilterAwaitableAction(IStateAction[] stateActions)
        {
            _awaitableStateAction.Clear();
            foreach (var action in stateActions)
            {
                if (action is IAwaitableStateAction)
                    _awaitableStateAction.Add(action as IAwaitableStateAction);
            }

        }
    }
}
