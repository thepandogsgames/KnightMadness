using UnityEngine;

namespace Code.Components.StateMachine
{
    public interface IState
    {
        public void OnEnterState();
        public void OnExitState();
        public void OnUpdateState();
    }
}