using UnityEngine;
using Code.Board;
using Code.Components.Audio;
using Code.Events;
using Code.Horse;
using Code.Utilities.Enums;
using Code.Pawn;
using Code.Persistence;
using UnityEngine.Audio;

namespace Code
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject horsePrefab;
        [SerializeField] private GameObject pawnPrefab;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private int movesToSpawnPawn;
        [SerializeField] private int movesToSpawnPawnSecondary;

        [Tooltip("-1 to disable")] [SerializeField]
        private int pawnsEatenToActivateAuxiliarySpawn;

        [SerializeField] private GameObject glovesGameObject;
        [Header("Audio")] [SerializeField] private AudioMixer audioMixer;

        private BoardController _boardController;
        private Spawner.Spawner _spawner;
        private float _time;
        private IEventManager _eventManager;
        private PawnManager _pawnManager;
        private IBoardPiece _horseInstance;
        private LocalPersistence _localPersistence;
        private IAudioController _audioController;
        private int _movesCount;
        private int _movesCountAuxiliary;
        private int _pawnsEaten;
        private bool _isGamePlaying;

        private int _maxScore;

        private bool _extraPawnSpawned;

        private void Awake()
        {
            _eventManager = new EventManager();
            _spawner = new Spawner.Spawner();
            _boardController = new BoardController(cellPrefab, _spawner);
            _boardController.CreateBoard();
            _pawnManager = new PawnManager(_eventManager, pawnPrefab, _spawner);
            _localPersistence = new LocalPersistence();
            _audioController = new AudioController(audioMixer);
            _maxScore = _localPersistence.GetMaxScore();
            _eventManager.Subscribe(EventTypeEnum.HorseWaiting, OnHorseWaiting);
            _eventManager.Subscribe(EventTypeEnum.GameStarted, OnGameStarted);
            _eventManager.Subscribe(EventTypeEnum.PlayerEaten, OnPlayerEaten);
            _eventManager.Subscribe(EventTypeEnum.PawnsHidden, OnPawnsHidden);
            _eventManager.Subscribe(EventTypeEnum.NoActivePawns, OnNoActivePawns);
            _eventManager.Subscribe<IBoardPiece>(EventTypeEnum.PawnEaten, OnPawnEaten);
            SpawnHorse();
        }


        private void OnGameStarted()
        {
            _extraPawnSpawned = false;
            _isGamePlaying = true;
            _movesCount = 0;
            _movesCountAuxiliary = 0;
            _pawnsEaten = 0;
            _boardController.ShowBoard();
            glovesGameObject.SetActive(true);
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

        private void OnHorseWaiting()
        {
            if (_extraPawnSpawned)
            {
                _extraPawnSpawned = false;
            }
            else if (_pawnManager.TryToEat())
            {
                return;
            }

            CheckAndSpawnPawn(ref _movesCount, movesToSpawnPawn);
            CheckAndSpawnPawn(ref _movesCountAuxiliary, movesToSpawnPawnSecondary,
                _pawnsEaten,
                pawnsEatenToActivateAuxiliarySpawn);

            _eventManager.TriggerEventAsync(EventTypeEnum.HorseCanMove);
        }

        private void CheckAndSpawnPawn(ref int moveCounter, int movesRequired, int conditionCounter = 0,
            int conditionThreshold = 0)
        {
            if (conditionThreshold != -1 && conditionCounter < conditionThreshold) return;

            moveCounter++;
            if (moveCounter < movesRequired) return;

            moveCounter = 0;
            SpawnPawn();
        }

        private void OnPlayerEaten()
        {
            _isGamePlaying = false;
            if (_maxScore < _pawnsEaten) _maxScore = _pawnsEaten;
            _localPersistence.SetMaxScore(_maxScore);
            _boardController.ClearBoard();
            _eventManager.TriggerEventAsync(EventTypeEnum.GameEnded);
            glovesGameObject.SetActive(false);
        }

        private void OnPawnsHidden()
        {
            _boardController.HiddeBoard();
        }

        private void OnNoActivePawns()
        {
            if (!_isGamePlaying) return;
            _extraPawnSpawned = true;
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

        public LocalPersistence GetLocalPersistence()
        {
            return _localPersistence;
        }

        public Spawner.Spawner GetSpawner()
        {
            return _spawner;
        }

        public IAudioController GetAudioController()
        {
            return _audioController;
        }
    }
}