using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.Core
{
    public class Util : MonoBehaviour
    {
        [SerializeField] public static GameObject standardFloorTile;
        [SerializeField] public static List<GameObject> otherFloorTiles;

        [Space]

        // Wall tiles
        public static GameObject wallTile;

        [Space]
        public static GameObject wallTileNorth;
        public static GameObject wallTileEast;
        public static GameObject wallTileSouth;
        public static GameObject wallTileWest;

        [Space]
        // Corner tiles
        public static GameObject wallCornerNw;

        public static GameObject wallCornerNe;
        public static GameObject wallCornerSe;
        public static GameObject wallCornerSw;

        [Space]
        // Outer Corner tiles
        public static GameObject wallCornerNwOuter;

        public static GameObject wallCornerNeOuter;
        public static GameObject wallCornerSeOuter;
        public static GameObject wallCornerSwOuter;

        public static GameObject PlaceTile(GameObject tile, int x, int y, GameObject parent)
        {
            return Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, parent.transform);
        }
        
        private static GameObject GetRandomTile(IReadOnlyList<GameObject> tileCollection, float stdLikelihood)
        {
            var random = Random.Range((stdLikelihood + 1) - 100, stdLikelihood);

            return random > 0 ? Util.standardFloorTile : tileCollection[Random.Range(0, tileCollection.Count)];
        }
    }
}