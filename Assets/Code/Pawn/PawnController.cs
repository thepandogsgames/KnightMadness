using Code.Board;
using Code.Components.Audio;
using Code.Events;
using Code.Horse;
using Code.Utilities.Enums;
using PrimeTween;
using UnityEngine;

namespace Code.Pawn
{
    public class PawnController : MonoBehaviour, IBoardPiece
    {
        [SerializeField] private AudioClip eatedSound;
        [SerializeField] private AudioClip moveSound;
        
        private IBoard _board;
        private IEventManager _eventManager;
        public IBoardCell CurrentCell { get; set; }
        private SpriteRenderer _spriteRenderer;
        private IAudioController _audioController;


        private readonly Vector2Int[] _eatPattern = new Vector2Int[]
        {
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1)
        };

        private void Awake()
        {
            var gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
            _board = gameController.GetBoard();
            _eventManager = gameController.GetEventManager();
            _audioController = gameController.GetAudioController();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }


        public bool TryToEat()
        {
            Vector2Int currentPos = CurrentCell.BoardPosition;
            int rows = _board.Board.GetLength(0);
            int columns = _board.Board.GetLength(1);

            foreach (var move in _eatPattern)
            {
                Vector2Int newPos = currentPos + move;
                if (newPos.x < 0 || newPos.x >= rows || newPos.y < 0 || newPos.y >= columns)
                    continue;

                IBoardCell targetCell = _board.Board[newPos.x, newPos.y];
                if (targetCell.CurrentPiece is HorseController)
                {
                    _audioController.PlaySoundWithRandomPitch(moveSound, false, 0.8f, 1.2f);
                    _spriteRenderer.sortingOrder = 2;
                    Sequence.Create()
                        .Group(Tween.Position(transform,
                            new Vector3(targetCell.BoardPosition.x, targetCell.BoardPosition.y, 0), 0.5f))
                        .Group(
                            Sequence.Create()
                                .Chain(Tween.Scale(transform, new Vector3(1.2f, 1.2f, 1.2f), 0.25f))
                                .Chain(Tween.Scale(transform, new Vector3(0.75f, 0.75f, 0.75f), 0.25f)))
                        .OnComplete(() => targetCell.CurrentPiece.Eaten());
                    return true;
                }
            }

            return false;
        }

        public void Eaten()
        {
            _audioController.PlaySoundWithRandomPitch(eatedSound, false, 0.8f, 1.2f);
            _eventManager.TriggerEventAsync(EventTypeEnum.PawnEaten, this);
        }

        public void Hidden()
        {
            if ((PawnController)CurrentCell.CurrentPiece == this) CurrentCell.CurrentPiece = null;
            CurrentCell = null;
        }

        public void Reset()
        {
            //No needed
        }
    }
}