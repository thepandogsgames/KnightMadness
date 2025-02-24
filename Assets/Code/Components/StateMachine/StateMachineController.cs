using System;
using System.Collections.Generic;

namespace Code.Components.StateMachine
{
    public class StateMachineController
    {
        private IState _currentState;
        private Dictionary<Enum, IState> _states;


        public void ConfigMachine(Dictionary<Enum, IState> states)
        {
            _states = states;
        }

        public void ChangeState(Enum state)
        {
            _currentState?.OnExitState();
            _currentState = _states[state];
            _currentState?.OnEnterState();
        }

        public void UpdateMachine()
        {
            _currentState?.OnUpdateState();
        }
    }
}