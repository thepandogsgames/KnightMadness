using UnityEngine;

namespace Code.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        private Cell[,] _board;

        private void Awake()
        {
            _board = new Cell[4, 4];
            CreateBoard();
            CenterBoard();
        }

        private void CreateBoard()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 worldPosition = new Vector3(i, j, 0);

                    GameObject cellObj = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform);

                    Cell cell = cellObj.GetComponent<Cell>();

                    cell.BoardPosition = new Vector2Int(i, j);
                    cell.SetColorBasedOnPosition();

                    _board[i, j] = cell;
                }
            }
        }

        private void CenterBoard()
        {
            Vector2 boardCenter = new Vector2((4 - 1) / 2f, (4 - 1) / 2f);
            transform.position = - (Vector3)boardCenter;
        }
    }
}