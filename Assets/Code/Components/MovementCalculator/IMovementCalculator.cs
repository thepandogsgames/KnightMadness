using System.Collections.Generic;
using Code.Board;

namespace Code.Components.MovementCalculator
{
    public interface IMovementCalculator
    {
        List<IBoardCell> GetValidMoves(IBoardCell currentCell, IBoardCell[,] board);
    }
}
