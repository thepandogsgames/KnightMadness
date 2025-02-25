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
        private BoardController _boardController;
        private Spawner.Spawner _spawner;
        private float _time;
        private IEventManager _eventManager;
        private PawnManager _pawnManager;
        private IBoardPiece _horseInstance;
        private int _movesCount;
        private bool _isGamePlaying = false;

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
            SpawnHorse();
        }


        private void OnGameStarted()
        {
            _isGamePlaying = true;
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
            _movesCount++;
            if (_movesCount >= movesToSpawnPawn)
            {
                SpawnPawn();
                _movesCount = 0;
            }

            _eventManager.TriggerEventAsync(EventTypeEnum.HorseCanMove);
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
            Debug.Log("No active pawns in the game");
            SpawnPawn();
            _movesCount = 0;
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