using System;
using System.Collections.Generic;
using Code.Board;
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
        private IEventManager _eventManager;
        private IBoard _board;
        private StateMachineController _stateMachineController;
        private IBoardCell _currentCell;

        public IBoardCell CurrentCell { get; set; }

        private void Awake()
        {
            var gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
            _eventManager = gameController.GetEventManager();
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
                    new HorseMoveState(this, _stateMachineController, _eventManager, transform)
                },
                {
                    PlayerStatesEnum.WaitState,
                    new HorseWaitState(_stateMachineController, _eventManager)
                }
            });
        }

        public void Eaten()
        {
            _eventManager.TriggerEventAsync(EventTypeEnum.PlayerEaten);
            Tween.Scale(transform, Vector3.zero, 0.5f).OnComplete(() => gameObject.SetActive(false));
            _currentCell.CurrentPiece = null;
            _currentCell = null;
        }
    }
}