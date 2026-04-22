
using System;
using System.Collections.Generic;
using LSW._02._Code.Entity;
using UnityEngine;

namespace LSW._02._Code.System___Manager.StateMachine
{
    public class StateMachineCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private string[] stateNames;
        
        private Entities.Entity _owner;
        private readonly Dictionary<string, State> _states = new Dictionary<string, State>();

        private State _currentState;
        private bool _isInitialized;
        
        public void Initialize(Entities.Entity entity)
        {
            _owner = entity;
            InitializeState();
        }

        private void InitializeState()
        {
            if (_owner == null) 
                return;
            
            if(stateNames.Length <= 0)
                return;
            
            if (_states.Count == 0)
            {
                for (int i = 0; i < stateNames.Length; i++)
                {
                    if(stateNames[i] == string.Empty)
                        continue;
                    
                    Type type = Type.GetType(stateNames[i]);
                    if (type == null) 
                        continue;
                    
                    try
                    {
                        var state = (State)Activator.CreateInstance(type, _owner, _owner.stat, this);
                        _states.Add(stateNames[i], state);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
            
            for (int i = 0; i < stateNames.Length; i++)
            {
                if(stateNames[i] == string.Empty)
                    continue;
                
                _currentState = _states[stateNames[i]];
                _currentState.Enter();
                break;
            }
            
            _isInitialized = true;
        }

        private void Update()
        {
            if(!_isInitialized || _currentState == null)
                return;
            
            _currentState.UpdateState();
        }
        
        private void FixedUpdate()
        {
            if(!_isInitialized || _currentState == null)
                return;
            
            _currentState.FixedUpdateState();
        }
        
        public void TransitionState(string newStateName)
        {
            if (!_isInitialized || !_states.ContainsKey(newStateName)) 
                return;

            _currentState?.Exit();
            _currentState = _states[newStateName];
            _currentState.Enter();
        }

        private void OnDisable()
        {
            if(!_isInitialized || _currentState == null)
                return;
            
            _currentState.Exit();
        }
    }
}