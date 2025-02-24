using Code.Board;
using Code.Components.StateMachine;
using Code.Events;
using Code.Utilities.Enums;
using PrimeTween;
using UnityEngine;

namespace Code.Horse.States
{
    public class HorseMoveState : IState
    {
        private readonly HorseController _horseController;
        private readonly StateMachineController _stateMachineController;
        private readonly IEventManager _eventManager;
        private readonly Transform _horseTransform;

        private IBoardCell _targetCell;

        public HorseMoveState(HorseController horseController,StateMachineController stateMachineController, IEventManager eventManager,
            Transform horseTransform)
        {
            _horseController = horseController;
            _stateMachineController = stateMachineController;
            _eventManager = eventManager;
            _horseTransform = horseTransform;
            _eventManager.Subscribe<IBoardCell>(EventTypeEnum.CellSelected, OnCellSelected);
        }

        public void OnEnterState()
        {
            _targetCell.CurrentPiece = _horseController;
            _horseController.CurrentCell = _targetCell;

            Sequence.Create()
                .Group(Tween.Position(_horseTransform,
                    new Vector3(_targetCell.BoardPosition.x, _targetCell.BoardPosition.y, 0), 1))
                .Group(
                    Sequence.Create()
                        .Chain(Tween.Scale(_horseTransform, new Vector3(1.2f, 1.2f, 1.2f), 0.5f))
                        .Chain(Tween.Scale(_horseTransform, new Vector3(1f, 1f, 1f), 0.5f)))
                .OnComplete(OnMoveCompleted);
        }

        public void OnExitState()
        {
            _eventManager.TriggerEventAsync(EventTypeEnum.PlayerMoved);
        }

        public void OnUpdateState()
        {
        }

        private void OnCellSelected(IBoardCell cell)
        {
            _targetCell = cell;
        }

        private void OnMoveCompleted()
        {
            _stateMachineController.ChangeState(PlayerStatesEnum.SelectState);
        }
    }
}