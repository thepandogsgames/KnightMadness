using UnityEngine;
using Code.Board;
namespace Code
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject horsePrefab;
        [SerializeField] private GameObject cellPrefab;
        private BoardController _boardController;
        private Spawner.Spawner _spawner;
        private float _time;
        void Awake()
        {
            _spawner = new Spawner.Spawner();
            _boardController = new BoardController(cellPrefab, _spawner);
            _boardController.CreateBoard();
            SpawnHorse();
        }

        private void SpawnHorse()
        {
            var cell = _boardController.Board[1, 0];
            Vector3 position = new Vector3(cell.BoardPosition.x, cell.BoardPosition.y, 0);
            cell.CurrentPiece = _spawner.SpawnPiece(horsePrefab, position, transform);
        }

        private void Update()
        {
            _time += Time.deltaTime;
            if (_time > 2.5f)
            {
                var freeCell = _boardController.GetFreeCell();
                if (freeCell is not null)
                {
                    freeCell.GetComponent<SpriteRenderer>().color = Color.red;
                }
                _time = 0;

            }
        }
    }
}
