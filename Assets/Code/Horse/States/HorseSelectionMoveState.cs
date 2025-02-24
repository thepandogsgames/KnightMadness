using System.Collections.Generic;
using Code.Board;
using Code.Components.MovementCalculator;
using Code.Components.StateMachine;
using Code.Events;
using Code.Utilities.Enums;
using UnityEngine;

namespace Code.Horse.States
{
    public class HorseSelectionMoveState : IState
    {
        private readonly StateMachineController _stateMachineController;
        private readonly IMovementCalculator _movementCalculator;
        private readonly IEventManager _eventManager;
        private readonly HorseController _horseController;
        private readonly IBoard _board;
        private readonly Camera _camera;
        private List<IBoardCell> _validMovesCells;

        private readonly Vector2Int[] _horseMoves = new Vector2Int[]
        {
            new Vector2Int(2, 1),
            new Vector2Int(1, 2),
            new Vector2Int(-1, 2),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(-1, -2),
            new Vector2Int(1, -2),
            new Vector2Int(2, -1)
        };

        public HorseSelectionMoveState(StateMachineController stateMachineController, IEventManager eventManager,
            HorseController horseController, IBoard board)
        {
            _stateMachineController = stateMachineController;
            _eventManager = eventManager;
            _horseController = horseController;
            _board = board;
            _movementCalculator = new MovementCalculator(_horseMoves);
            _camera = Camera.main;
        }

        public void OnEnterState()
        {
            _validMovesCells =
                _movementCalculator.GetValidMoves(_horseController.CurrentCell, _board.Board);

            foreach (var cell in _validMovesCells)
            {
                cell.MarkAsValidMove();
            }
        }

        public void OnExitState()
        {
            _validMovesCells.Clear();
        }

        public void OnUpdateState()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 worldPos2D = new Vector2(worldPosition.x, worldPosition.y);

                RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
                if (hit.collider is not null)
                {
                    Cell clickedCell = hit.collider.GetComponent<Cell>();
                    if (clickedCell is not null && _validMovesCells.Contains(clickedCell))
                    {
                        _eventManager.TriggerEventAsync(EventTypeEnum.CellSelected, clickedCell);
                        _stateMachineController.ChangeState(PlayerStatesEnum.MoveState);
                    }
                }
            }
        }
    }
}