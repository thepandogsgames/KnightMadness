using UnityEngine;

namespace Code.Board
{
    public interface IBoardCell
    {
        public void MarkAsValidMove();
        
        public IBoardPiece CurrentPiece { get; set; }
        public Vector2Int BoardPosition { get; set; }

    }
}
