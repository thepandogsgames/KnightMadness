using System;
using System.Collections.Generic;
using Code.Board;
using Code.Components.Audio;
using Code.Components.StateMachine;
using Code.Events;
using Code.Horse.States;
using Code.Utilities.Enums;
using PrimeTween;
using UnityEngine;

namespace Code.Horse
{
    public class HorseController : MonoBehaviour, IBoardPiece
    {
        [SerializeField] private AudioClip eatedSound;
        [SerializeField] private AudioClip moveSound;

        private IEventManager _eventManager;
        private IBoard _board;
        private StateMachineController _stateMachineController;
        private IBoardCell _currentCell;
        private Transform _transform;
        private IAudioController _audioController;

        public IBoardCell CurrentCell { get; set; }

        private void Awake()
        {
            _transform = transform;
            var gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
            _eventManager = gameController.GetEventManager();
            _audioController = gameController.GetAudioController();
            _board = gameController.GetBoard();
            _stateMachineController = new StateMachineController();
            ConfigureStates();
        }

        private void Start()
        {
            _stateMachineController.ChangeState(PlayerStatesEnum.SelectState);
        }

        private void Update()
        {
            if (!isActiveAndEnabled) return;
            _stateMachineController.UpdateMachine();
        }

        private void ConfigureStates()
        {
            _stateMachineController.ConfigMachine(new Dictionary<Enum, IState>()
            {
                {
                    PlayerStatesEnum.SelectState,
                    new HorseSelectionMoveState(_stateMachineController, _eventManager, this, _board)
                },
                {
                    PlayerStatesEnum.MoveState,
                    new HorseMoveState(this, _stateMachineController, _eventManager, _audioController, transform,
                        moveSound)
                },
                {
                    PlayerStatesEnum.WaitState,
                    new HorseWaitState(_stateMachineController, _eventManager)
                }
            });
        }

        public void Eaten()
        {
            _audioController.PlaySoundWithRandomPitch(eatedSound, false, 0.8f, 1.2f);
            Tween.Scale(_transform, Vector3.zero, 0.5f)
                .OnComplete(() => _eventManager.TriggerEventAsync(EventTypeEnum.PlayerEaten), false);
        }

        public void Reset()
        {
            gameObject.SetActive(true);
            Tween.Scale(_transform, new Vector3(0.75f, 0.75f, 0.75f), 0.5f);
        }
    }
}