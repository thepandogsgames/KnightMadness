using Code.Components.StateMachine;
using Code.Events;
using Code.Utilities.Enums;

namespace Code.Horse.States
{
    public class HorseWaitState : IState
    {
        private readonly IEventManager _eventManager;
        private readonly StateMachineController _stateMachineController;

        private bool _canMove;

        public HorseWaitState(StateMachineController stateMachineController, IEventManager eventManager)
        {
            _stateMachineController = stateMachineController;
            _eventManager = eventManager;
        }

        public void OnEnterState()
        {
            _eventManager.Subscribe(EventTypeEnum.HorseCanMove, OnHorseCanMove);
            _eventManager.TriggerEventAsync(EventTypeEnum.HorseWaiting);
        }

        public void OnExitState()
        {
            _canMove = false;
            _eventManager.Unsubscribe(EventTypeEnum.HorseCanMove, OnHorseCanMove);
        }

        public void OnUpdateState()
        {
            if (_canMove)
            {
                _stateMachineController.ChangeState(PlayerStatesEnum.SelectState);
            }
        }

        private void OnHorseCanMove()
        {
            _canMove = true;
        }
    }
}