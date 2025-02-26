using UnityEngine;
using Code.Board;
using Code.Events;
using Code.Horse;
using Code.Utilities.Enums;
using Code.Pawn;

namespace Code
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject horsePrefab;
        [SerializeField] private GameObject pawnPrefab;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private int movesToSpawnPawn;
        [SerializeField] private int movesToSpawnPawnSecondary;
        [SerializeField] private int pawnsEatenToActivateAuxiliarySpawn;
        private BoardController _boardController;
        private Spawner.Spawner _spawner;
        private float _time;
        private IEventManager _eventManager;
        private PawnManager _pawnManager;
        private IBoardPiece _horseInstance;
        private int _movesCount;
        private int _movesCountAuxiliary;
        private int _pawnsEaten;
        private bool _isGamePlaying;

        private void Awake()
        {
            _eventManager = new EventManager();
            _spawner = new Spawner.Spawner();
            _boardController = new BoardController(cellPrefab, _spawner);
            _boardController.CreateBoard();
            _pawnManager = new PawnManager(_eventManager, pawnPrefab, _spawner);
            _eventManager.Subscribe(EventTypeEnum.PlayerMoved, OnPlayerMoved);
            _eventManager.Subscribe(EventTypeEnum.GameStarted, OnGameStarted);
            _eventManager.Subscribe(EventTypeEnum.PlayerEaten, OnPlayerEaten);
            _eventManager.Subscribe(EventTypeEnum.PawnsHidden, OnPawnsHidden);
            _eventManager.Subscribe(EventTypeEnum.NoActivePawns, OnNoActivePawns);
            _eventManager.Subscribe<IBoardPiece>(EventTypeEnum.PawnEaten, OnPawnEaten);
            SpawnHorse();
        }


        private void OnGameStarted()
        {
            _isGamePlaying = true;
            _movesCount = 0;
            _movesCountAuxiliary = 0;
            _pawnsEaten = 0;
            _boardController.ShowBoard();
            PlaceHorse();
            SpawnPawn();
            _eventManager.TriggerEventAsync(EventTypeEnum.HorseCanMove);
        }

        private void SpawnHorse()
        {
            _horseInstance = _spawner.SpawnPiece(horsePrefab, Vector3.zero, transform);
        }

        private void PlaceHorse()
        {
            _movesCount = 0;
            var cell = _boardController.Board[1, 0];
            Vector3 position = new Vector3(cell.BoardPosition.x, cell.BoardPosition.y, 0);
            var horse = _horseInstance as HorseController;
            horse!.transform.position = position;
            _horseInstance.Reset();
            _horseInstance.CurrentCell = cell;
            cell.CurrentPiece = _horseInstance;
        }

        private void SpawnPawn()
        {
            var freeCell = _boardController.GetFreeCell();
            if (freeCell is null) return;
            _pawnManager.GetPawn(freeCell);
        }

        private void OnPlayerMoved()
        {
            if (_pawnManager.TryToEat()) return;

            CheckAndSpawnPawn(ref _movesCount, movesToSpawnPawn);
            
            CheckAndSpawnPawn(ref _movesCountAuxiliary, movesToSpawnPawnSecondary,
                _pawnsEaten,
                pawnsEatenToActivateAuxiliarySpawn);

            _eventManager.TriggerEventAsync(EventTypeEnum.HorseCanMove);
        }

        private void CheckAndSpawnPawn(ref int moveCounter, int movesRequired, int conditionCounter = 0,
            int conditionThreshold = 0)
        {
            if (conditionCounter < conditionThreshold) return;

            moveCounter++;
            if (moveCounter < movesRequired) return;

            moveCounter = 0;
            SpawnPawn();
        }

        private void OnPlayerEaten()
        {
            _isGamePlaying = false;
            _boardController.ClearBoard();
            _eventManager.TriggerEventAsync(EventTypeEnum.GameEnded);
        }

        private void OnPawnsHidden()
        {
            _boardController.HiddeBoard();
        }

        private void OnNoActivePawns()
        {
            if (!_isGamePlaying) return;
            SpawnPawn();
            _movesCount = 0;
        }

        private void OnPawnEaten(IBoardPiece pawn)
        {
            _pawnsEaten++;
        }

        public IBoard GetBoard()
        {
            return _boardController;
        }

        public IEventManager GetEventManager()
        {
            return _eventManager;
        }

        public Spawner.Spawner GetSpawner()
        {
            return _spawner;
        }
    }
}