using Code.Board;
using UnityEngine;

namespace Code.Spawner
{
    public interface ISpawner
    {
        public Cell SpawnCell(GameObject cellPrefab, Vector3 position, Transform parent);
        public IBoardPiece SpawnPiece(GameObject piecePrefab, Vector3 position, Transform parent);
    }
}