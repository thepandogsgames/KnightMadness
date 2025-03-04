using System.Collections.Generic;
using System.Linq;
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
        private GameObject _parent;
        private readonly float _invisibleScale = 0f;
        private readonly float _visibleScale = 0.75f;

        public PawnManager(IEventManager eventManager, GameObject pawnPrefab, Spawner.Spawner spawner)
        {
            _eventManager = eventManager;
            _pawnPrefab = pawnPrefab;
            _spawner = spawner;
            _parent = new GameObject("PawnPool");
            _activePawns = new List<PawnController>();
            _pool = new Queue<PawnController>();
            _eventManager.Subscribe<PawnController>(EventTypeEnum.PawnEaten, OnPawnEaten);
            _eventManager.Subscribe(EventTypeEnum.GameEnded, OnGameEnded);
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < 10; i++)
            {
                var pawn = SpawnPawn();
                var pawnController = pawn as PawnController;
                _pool.Enqueue(pawnController);
            }
        }

        private IBoardPiece SpawnPawn()
        {
            return _spawner.SpawnPiece(_pawnPrefab, Vector3.zero, _parent.transform);
        }

        public IBoardPiece GetPawn(IBoardCell cell)
        {
            var pawn = _pool.Count > 0 ? _pool.Dequeue() : SpawnPawn() as PawnController;
            pawn!.gameObject.SetActive(true);
            _activePawns.Add(pawn);
            pawn.CurrentCell = cell;
            cell.CurrentPiece = pawn;
            pawn.transform.position = new Vector3(cell.BoardPosition.x, cell.BoardPosition.y, 0);
            Tween.Scale(pawn.transform, new Vector3(_visibleScale, _visibleScale, _visibleScale), 0.25f);
            return pawn;
        }


        private void OnPawnEaten(PawnController pawn)
        {
            _eventManager.TriggerEventAsync(EventTypeEnum.AddPoint);
            HiddePawn(pawn);
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

        private void OnGameEnded()
        {
            foreach (var pawn in _activePawns.ToList())
            {
                HiddePawn(pawn);
            }

            _eventManager.TriggerEventAsync(EventTypeEnum.PawnsHidden);
        }

        private void HiddePawn(PawnController pawn)
        {
            _pool.Enqueue(pawn);
            _activePawns.Remove(pawn);
            if (_activePawns.Count == 0) _eventManager.TriggerEventAsync(EventTypeEnum.NoActivePawns);
            Tween.Scale(pawn.transform, new Vector3(_invisibleScale, _invisibleScale, _invisibleScale), 0.4f)
                .OnComplete(() => pawn.gameObject.SetActive(false));
            pawn.Hidden();
        }
    }
}