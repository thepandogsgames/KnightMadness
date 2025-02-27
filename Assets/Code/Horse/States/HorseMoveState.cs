using Code.Board;
using Code.Components.Audio;
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
        private readonly IAudioController _audioController;
        private readonly Transform _horseTransform;
        private AudioClip _moveSound;

        private IBoardCell _targetCell;

        public HorseMoveState(HorseController horseController, StateMachineController stateMachineController,
            IEventManager eventManager, IAudioController audioController, Transform horseTransform,
            AudioClip moveSound)
        {
            _horseController = horseController;
            _stateMachineController = stateMachineController;
            _eventManager = eventManager;
            _horseTransform = horseTransform;
            _audioController = audioController;
            _moveSound = moveSound;
            _eventManager.Subscribe<IBoardCell>(EventTypeEnum.CellSelected, OnCellSelected);
        }

        public void OnEnterState()
        {
            _audioController.PlaySoundWithRandomPitch(_moveSound, false, 0.8f, 1.2f);
            _eventManager.TriggerEventAsync(EventTypeEnum.PlayerMoves);
            _targetCell.CurrentPiece?.Eaten();
            _targetCell.CurrentPiece = _horseController;
            _horseController.CurrentCell.CurrentPiece = null;
            _horseController.CurrentCell = _targetCell;

            Sequence.Create()
                .Group(Tween.Position(_horseTransform,
                    new Vector3(_targetCell.BoardPosition.x, _targetCell.BoardPosition.y, 0), 0.3f))
                .Group(
                    Sequence.Create()
                        .Chain(Tween.Scale(_horseTransform, new Vector3(1.2f, 1.2f, 1.2f), 0.15f))
                        .Chain(Tween.Scale(_horseTransform, new Vector3(0.75f, 0.75f, 0.75f), 0.15f)))
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
            _stateMachineController.ChangeState(PlayerStatesEnum.WaitState);
        }
    }
}