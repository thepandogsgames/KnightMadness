using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Board
{
    public class BoardController : IBoard
    {
        private readonly int _rows;
        private readonly int _columns;
        private readonly Cell[,] _board;
        private GameObject _cellPrefab;
        private Spawner.Spawner _spawner;
        public IBoardCell[,] Board => _board;

        private GameObject _boardParent;

        public BoardController(GameObject cellPrefab, Spawner.Spawner spawner)
        {
            _rows = 4;
            _columns = 4;
            _board = new Cell[_rows, _columns];
            _cellPrefab = cellPrefab;
            _spawner = spawner;
        }


        public void CreateBoard()
        {
            _boardParent = new GameObject("Board");
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    Vector3 worldPosition = new Vector3(i, j, 0);

                    Cell cell = _spawner.SpawnCell(_cellPrefab, worldPosition, _boardParent.transform);

                    cell.BoardPosition = new Vector2Int(i, j);
                    cell.SetColorBasedOnPosition();
                    _board[i, j] = cell;
                }
            }
            _boardParent.SetActive(false);
        }

        public void ShowBoard()
        {
            _boardParent.SetActive(true);
        }

        public void HiddeBoard()
        {
            _boardParent.SetActive(false);
        }

        public IBoardCell GetFreeCell()
        {
            List<(Cell cell, float weight)> freeCells = new List<(Cell, float)>();

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    Cell cell = _board[i, j];
                    if (cell.CurrentPiece is not null) continue;
                    float weight = IsCorner(i, j) ? 2.5f : 7.5f;
                    freeCells.Add((cell, weight));
                }
            }

            if (freeCells.Count == 0) return null;

            float totalWeight = freeCells.Sum(entry => entry.weight);

            float randomValue = Random.Range(0, totalWeight);

            foreach (var entry in freeCells)
            {
                randomValue -= entry.weight;
                if (randomValue <= 0)
                {
                    return entry.cell;
                }
            }

            return freeCells.Last().cell;
        }

        private bool IsCorner(int i, int j)
        {
            return (i == 0 && j == 0) ||
                   (i == 0 && j == _columns - 1) ||
                   (i == _rows - 1 && j == 0) ||
                   (i == _rows - 1 && j == _columns - 1);
        }

        public void ClearBoard()
        {
            foreach (var boardCell in _board)
            {
                boardCell.ClearCell();
            }
        }
    }
}