using Code.Board;
using UnityEngine;

namespace Code.Spawner
{
    public class Spawner : ISpawner
    {
        public Cell SpawnCell(GameObject cellPrefab, Vector3 position, Transform parent)
        {
            return Object.Instantiate(cellPrefab, position, Quaternion.identity, parent)
                .GetComponent<Cell>();
        }

        public IBoardPiece SpawnPiece(GameObject piecePrefab, Vector3 position, Transform parent)
        {
            return Object.Instantiate(piecePrefab, position, Quaternion.identity, parent)
                .GetComponent<IBoardPiece>();
        }
    }
}