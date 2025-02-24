using System.Collections.Generic;
using Code.Board;
using UnityEngine;

namespace Code.Components.MovementCalculator
{
    public class MovementCalculator : IMovementCalculator
    {
        private readonly Vector2Int[] _movePatterns;

        public MovementCalculator(Vector2Int[] movePatterns)
        {
            _movePatterns = movePatterns;
        }

        public List<IBoardCell> GetValidMoves(IBoardCell currentCell, IBoardCell[,] board)
        {
            List<IBoardCell> validMoves = new List<IBoardCell>();
            Vector2Int currentPos = currentCell.BoardPosition;
            int rows = board.GetLength(0);
            int columns = board.GetLength(1);

            foreach (var move in _movePatterns)
            {
                Vector2Int newPos = currentPos + move;
                if (newPos.x < 0 || newPos.x >= rows || newPos.y < 0 || newPos.y >= columns) continue;
                IBoardCell targetCell = board[newPos.x, newPos.y];
                validMoves.Add(targetCell);
            }

            return validMoves;
        }
    }
}