using System.Collections.Generic;
using Code.Board;
using Code.Events;
using Code.Utilities.Enums;
using PrimeTween;
using UnityEngine;

namespace Code.Pawn
{
    public class PawnManager
    {
        private readonly GameObject _pawnPrefab;
        private readonly Spawner.Spawner _spawner;
        private readonly IEventManager _eventManager;
        private readonly List<PawnController> _activePawns;
        private Queue<PawnController> _pool;

        private readonly float _invisibleScale = 0f;
        private readonly float _visibleScale = 0.75f;

        public PawnManager(IEventManager eventManager, GameObject pawnPrefab, Spawner.Spawner spawner)
        {
            _eventManager = eventManager;
            _pawnPrefab = pawnPrefab;
            _spawner = spawner;
            _eventManager.Subscribe<PawnController>(EventTypeEnum.PawnEaten, OnPawnEaten);
            _activePawns = new List<PawnController>();
            _pool = new Queue<PawnController>();
            InitializePool();
        }

        private void InitializePool()
        {
            var parent = new GameObject("PawnPool");
            for (int i = 0; i < 10; i++)
            {
                var pawn = _spawner.SpawnPiece(_pawnPrefab, Vector3.zero, parent.transform);
                var pawnController = pawn as PawnController;
                _pool.Enqueue(pawnController);
            }
        }

        public IBoardPiece GetPawn(IBoardCell cell)
        {
            var pawn = _pool.Dequeue();
            pawn.gameObject.SetActive(true);
            pawn.CurrentCell = cell;
            cell.CurrentPiece = pawn;
            pawn.transform.position = new Vector3(cell.BoardPosition.x, cell.BoardPosition.y, 0);
            Tween.Scale(pawn.transform, new Vector3(_visibleScale, _visibleScale, _visibleScale), 0.5f);
            _activePawns.Add(pawn);
            return pawn;
        }


        private void OnPawnEaten(PawnController pawn)
        {
            Tween.Scale(pawn.transform, new Vector3(_invisibleScale, _invisibleScale, _invisibleScale), 0.5f)
                .OnComplete(() => pawn.gameObject.SetActive(false));
            _eventManager.TriggerEventAsync(EventTypeEnum.AddPoint);
            _activePawns.Remove(pawn);
            _pool.Enqueue(pawn);
        }

        public bool TryToEat()
        {
            foreach (var pawn in _activePawns)
            {
                if (pawn.TryToEat())
                {
                    return true;
                }
            }

            return false;
        }
    }
}