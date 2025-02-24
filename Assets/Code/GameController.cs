using UnityEngine;
using Code.Board;
using Code.Events;
using Code.Utilities.Enums;
using Code.Pawn;

namespace Code
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject horsePrefab;
        [SerializeField] private GameObject pawnPrefab;
        [SerializeField] private GameObject cellPrefab;
        private BoardController _boardController;
        private Spawner.Spawner _spawner;
        private float _time;
        private IEventManager _eventManager;
        private PawnManager _pawnManager;

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
        }

        

        private void OnGameStarted()
        {
            _boardController.ShowBoard();
            SpawnHorse();
            SpawnPawn();
        }

        private void SpawnHorse()
        {
            var cell = _boardController.Board[1, 0];
            Vector3 position = new Vector3(cell.BoardPosition.x, cell.BoardPosition.y, 0);
            var horse = _spawner.SpawnPiece(horsePrefab, position, transform);
            horse.CurrentCell = cell;
            cell.CurrentPiece = horse;
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
            SpawnPawn();
            _eventManager.TriggerEventAsync(EventTypeEnum.HorseCanMove);
        }
        
        private void OnPlayerEaten()
        {
            _eventManager.TriggerEventAsync(EventTypeEnum.GameEnded);
        }
        
        private void OnPawnsHidden()
        {
            _boardController.HiddeBoard();
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