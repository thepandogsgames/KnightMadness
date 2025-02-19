using System;
using UnityEngine;

namespace Code.Board
{
    public class Cell : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Vector2Int _boardPosition;
        private int _spawnWeight;
        private IBoardPiece _currentPiece;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public Vector2Int BoardPosition
        {
            get => _boardPosition;
            set => _boardPosition = value;
        }


        public int SpawnWeight
        {
            get => _spawnWeight;
            set => _spawnWeight = value;
        }

        public IBoardPiece CurrentPiece
        {
            get => _currentPiece;
            set => _currentPiece = value;
        }

        public void SetColorBasedOnPosition()
        {
            _spriteRenderer.color = (_boardPosition.x + _boardPosition.y) % 2 == 0 ? Color.black : Color.white;
        }
    }
}