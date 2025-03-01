using Code.Events;
using Code.Utilities.Enums;
using UnityEngine;

namespace Code.Board
{
    public class Cell : MonoBehaviour, IBoardCell
    {
        private IEventManager _eventManager;
        private SpriteRenderer _spriteRenderer;
        private Vector2Int _boardPosition;
        private Color _baseColor;
        private int _spawnWeight;

        public IBoardPiece CurrentPiece { get; set; }

        public Vector2Int BoardPosition
        {
            get => _boardPosition;
            set => _boardPosition = value;
        }

        private void Awake()
        {
            _eventManager = GameObject.FindWithTag("GameController").GetComponent<GameController>().GetEventManager();
            _eventManager.Subscribe(EventTypeEnum.PlayerMoves, OnPlayerMoves);
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetColorBasedOnPosition()
        {
            _baseColor = (_boardPosition.x + _boardPosition.y) % 2 == 0 ? Color.white : Color.black;
            _spriteRenderer.color = _baseColor;
        }

        public void MarkAsValidMove()
        {
            _spriteRenderer.color = Color.blue;
        }

        public void ClearCell()
        {
            CurrentPiece = null;
        }

        private void OnPlayerMoves()
        {
            _spriteRenderer.color = _baseColor;
        }
    }
}