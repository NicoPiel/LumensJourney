using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralGeneration.Core
{
    [BurstCompile]
    public class Generator : MonoBehaviour
    {
        [Header("Room Settings")] [Range(1, 30)]
        public int roomNumber;

        [Space] [Range(5, 30)] public int minWidth;
        [Range(5, 30)] public int maxWidth;
        [Space] [Range(5, 30)] public int minHeight;
        [Range(5, 30)] public int maxHeight;
        [Space] [Range(4, 40)] public int minSpacingX;
        [Range(4, 40)] public int maxSpacingX;
        [Space] [Range(-80, 0)] public int minSpacingY;
        [Range(0, 80)] public int maxSpacingY;

        [Header("Tileset")]
        [Tooltip("Determines the likelihood with which the standard floor tile will spawn.")]
        [Range(0, 100)]
        public int stdLikelihood;

        [SerializeField] public GameObject standardFloorTile;
        [SerializeField] public List<GameObject> otherFloorTiles;

        [Space]

        // Wall tiles
        [SerializeField]
        public GameObject wallTile;

        [Space] [SerializeField] public GameObject wallTileNorth;
        [SerializeField] public GameObject wallTileEast;
        [SerializeField] public GameObject wallTileSouth;
        [SerializeField] public GameObject wallTileWest;

        [Space]
        // Corner tiles
        [SerializeField]
        public GameObject wallCornerNw;

        [SerializeField] public GameObject wallCornerNe;
        [SerializeField] public GameObject wallCornerSe;
        [SerializeField] public GameObject wallCornerSw;

        [Space]
        // Outer Corner tiles
        [SerializeField]
        public GameObject wallCornerNwOuter;

        [SerializeField] public GameObject wallCornerNeOuter;
        [SerializeField] public GameObject wallCornerSeOuter;
        [SerializeField] public GameObject wallCornerSwOuter;

        [Space] [Header("Doors")] [SerializeField]
        public List<int[]> leftDoors;

        [SerializeField] public List<int[]> rightDoors;
        [SerializeField] public Dictionary<int[], int[]> doorRelations;
        [Space] private List<Rect> _rooms;

        public bool Generate()
        {
            if (minWidth > maxWidth
                || minHeight > maxHeight
                || minSpacingX > maxSpacingX
                || minSpacingY > maxSpacingY)
            {
                Debug.LogError(
                    "Parameters were misconfigured. Check, if all your 'min..' values are less than or equal to your 'max..' values");
                return false;
            }

            GenerateRooms();

            InstantiatePlayer();

            return true;
        }

        private void GenerateRooms()
        {
            Debug.Log("Generating rooms..");

            GameObject dungeon = CreateDungeonObject();
            GameObject spawner = CreateRoomSpawner();

            _rooms = new List<Rect>();
            rightDoors = new List<int[]>();
            leftDoors = new List<int[]>();
            doorRelations = new Dictionary<int[], int[]>();

            for (var i = 0; i < roomNumber; i++)
            {
                var randomRoomWidth = Random.Range(minWidth, maxWidth);
                var randomRoomHeight = Random.Range(minHeight, maxHeight);

                Vector3 position = spawner.transform.position;

                var room = new Rect(new Vector2(position.x / 2, position.y / 2),
                    new Vector2(randomRoomWidth, randomRoomHeight));

                var xMin = (int) math.floor(room.xMin);
                var xMax = (int) math.ceil(room.xMax);
                var yMin = (int) math.floor(room.yMin);
                var yMax = (int) math.ceil(room.yMax);

                _rooms.Add(room);

                // Make random position for the doors
                var doorLeft = Random.Range(yMin + 1, yMax - 1);
                var doorRight = Random.Range(yMin + 1, yMax - 1);

                // Generate all room tiles
                for (var x = xMin; x <= xMax; x++)
                {
                    for (var y = yMin; y <= yMax; y++)
                    {
                        // Generate floor tiles
                        if (x > xMin && x < xMax && y > yMin && y < yMax - 1)
                        {
                            PlaceTile(GetRandomTile(otherFloorTiles), x, y, dungeon);
                        }

                        else if (x != xMin && x != xMax && y == yMax - 1)
                        {
                            PlaceTile(wallTile, x, y, dungeon);
                        }

                        // Northwest corner
                        else if (x == xMin && y == yMax)
                        {
                            PlaceTile(wallCornerNw, x, y, dungeon);
                        }

                        // Northeast corner
                        else if (x == xMax && y == yMax)
                        {
                            PlaceTile(wallCornerNe, x, y, dungeon);
                        }

                        // Southwest corner
                        else if (x == xMin && y == yMin)
                        {
                            PlaceTile(wallCornerSw, x, y, dungeon);
                        }

                        // Southeast corner
                        else if (x == xMax && y == yMin)
                        {
                            PlaceTile(wallCornerSe, x, y, dungeon);
                        }

                        // Erect vertical walls
                        else if (x == xMin && y > yMin && y < yMax)
                        {
                            // Generate bottom wall, and make room for a door
                            if (y != doorLeft)
                            {
                                PlaceTile(wallTileEast, x, y, dungeon);
                            }
                            else if (y == doorLeft)
                            {
                                if (_rooms.Count > 1)
                                {
                                    PlaceTile(standardFloorTile, x, y, dungeon);
                                    leftDoors.Add(new[] {xMin, doorLeft});
                                }
                                else
                                {
                                    PlaceTile(wallTileEast, x, y, dungeon);
                                }
                            }
                        }
                        else if (x == xMax && y > yMin && y < yMax)
                        {
                            // Generate top wall, and make room for a door
                            if (y != doorRight)
                            {
                                PlaceTile(wallTileWest, x, y, dungeon);
                            }
                            else if (y == doorRight)
                            {
                                if (_rooms.Count < roomNumber)
                                {
                                    PlaceTile(standardFloorTile, x, y, dungeon);
                                    rightDoors.Add(new[] {xMax, doorRight});
                                }
                                else
                                {
                                    PlaceTile(wallTileWest, x, y, dungeon);
                                }
                            }
                        }

                        // Erect horizontal walls
                        else if (y == yMin)
                        {
                            PlaceTile(wallTileSouth, x, y, dungeon);
                        }
                        else if (y == yMax)
                        {
                            PlaceTile(wallTileNorth, x, y, dungeon);
                        }
                    }
                }

                // Move spawner to the right
                spawner.transform.position =
                    new Vector2(position.x + room.width * 2 + Random.Range(minSpacingX, maxSpacingX),
                        room.y + Random.Range(minSpacingY, maxSpacingY));
            }

            for (var i = 0; i < rightDoors.Count; i++)
            {
                doorRelations.Add(rightDoors[i], leftDoors[i]);
            }

            GenerateCorridors(dungeon);
        }

        private void GenerateCorridors(GameObject dungeon)
        {
            Debug.Log("Generating corridors..");

            foreach (var doorRelation in doorRelations)
            {
                var leftDoorX = doorRelation.Key[0];
                var leftDoorY = doorRelation.Key[1];
                var rightDoorX = doorRelation.Value[0];
                var rightDoorY = doorRelation.Value[1];

                int buffer = 0;

                int width = math.abs(rightDoorX - leftDoorX);
                int height = rightDoorY - leftDoorY;
                int middle = (int) math.floor(rightDoorX - width / 2);

                for (var x = leftDoorX + 1; x < middle; x++)
                {
                    if (width > 1)
                    {
                        if (x < middle)
                        {
                            // Place a wall directly above the corridor
                            PlaceTile(wallTile, x, leftDoorY + 1, dungeon);
                            // Place a dark tile above that
                            PlaceTile(wallTileNorth, x, leftDoorY + 2, dungeon);
                        }

                        // Place a dark tile directly below the corridor
                        PlaceTile(wallTileSouth, x, leftDoorY - 1, dungeon);
                    }

                    // Place floor tile
                    PlaceTile(standardFloorTile, x, leftDoorY, dungeon);

                    buffer = x + 2;
                }

                if (height > 0)
                {
                    // TODO: South-West Corner
                    
                    //
                    for (var y = leftDoorY + 3; y < rightDoorY - 1; y++)
                    {
                        if (width > 1 && y > leftDoorY)
                        {
                            if (buffer > leftDoorX)
                            {
                                PlaceTile(wallTileEast, buffer - 1, y, dungeon);
                            }

                            if (buffer < rightDoorX)
                            {
                                PlaceTile(wallTileWest, buffer + 1, y, dungeon);
                            }
                        }

                        PlaceTile(standardFloorTile, buffer, y, dungeon);
                    }
                    
                    
                    // North-West Corner
                    var tempBuffer = buffer;
                    
                    PlaceTile(standardFloorTile, tempBuffer, rightDoorY, dungeon);
                    PlaceTile(standardFloorTile, tempBuffer, rightDoorY - 1, dungeon);
                    
                    PlaceTile(standardFloorTile, ++tempBuffer, rightDoorY, dungeon);
                }
                
                else if (height < 0)
                {
                    // North-East Corner
                    var tempBuffer = buffer-2;
                    
                    PlaceTile(standardFloorTile, ++tempBuffer, leftDoorY, dungeon);
                    PlaceTile(standardFloorTile, ++tempBuffer, leftDoorY, dungeon);

                    PlaceTile(standardFloorTile, tempBuffer, leftDoorY - 1, dungeon);
                    
                    //
                    for (var y = leftDoorY - 2; y > rightDoorY + 2; y--)
                    {
                        if (width > 1 && y < leftDoorY)
                        {
                            if (buffer > leftDoorX)
                            {
                                PlaceTile(wallTileEast, buffer - 1, y, dungeon);
                            }

                            if (buffer < rightDoorX)
                            {
                                PlaceTile(wallTileWest, buffer + 1, y, dungeon);
                            }
                        }

                        PlaceTile(standardFloorTile, buffer, y, dungeon);
                    }
                    
                    // TODO: South-East Corner
                }
                
                for (var x = buffer + 2; x < rightDoorX; x++)
                {
                    if (width > 1)
                    {
                        if (x > middle - 1)
                        {
                            // Place a wall directly above the corridor
                            PlaceTile(wallTile, x, rightDoorY + 1, dungeon);
                            // Place a dark tile above that
                            PlaceTile(wallTileNorth, x, rightDoorY + 2, dungeon);
                        }

                        // Place a dark tile directly below the corridor
                        PlaceTile(wallTileSouth, x, rightDoorY - 1, dungeon);
                    }

                    PlaceTile(standardFloorTile, x, rightDoorY, dungeon);
                }
            }
        }

        private static GameObject CreateDungeonObject()
        {
            GameObject dungeon = GameObject.Find("Dungeon(Clone)");
            Destroy(GameObject.Find("Dungeon"));

            if (dungeon != null) Destroy(dungeon);

            dungeon = new GameObject {name = "Dungeon"};

            return Instantiate(dungeon, new Vector3(0, 0, 0), Quaternion.identity);
        }

        private static GameObject CreateRoomSpawner()
        {
            GameObject spawner = GameObject.Find("RoomSpawner");
            Destroy(GameObject.Find("RoomSpawner(Clone)"));

            if (spawner != null) Destroy(spawner);

            spawner = new GameObject {name = "RoomSpawner"};
            spawner.transform.position = new Vector2(0, 0);

            return Instantiate(spawner);
        }

        private GameObject PlaceTile(GameObject tile, int x, int y, GameObject parent)
        {
            return Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, parent.transform);
        }

        private GameObject InstantiatePlayer()
        {
            GameObject player = GameObject.Find("Player(Clone)");
            Destroy(GameObject.Find("Player"));

            if (player != null)
            {
                Destroy(player);
            }

            player = (GameObject) Instantiate(Resources.Load("Player/Player"),
                new Vector3(_rooms[0].x + 1, _rooms[0].y + 1, 0), Quaternion.identity, gameObject.transform);

            if (Camera.main != null) Camera.main.transform.parent = player.transform;

            return player;
        }

        private GameObject GetRandomTile(IReadOnlyList<GameObject> tileCollection)
        {
            var random = Random.Range((stdLikelihood + 1) - 100, stdLikelihood);

            return random > 0 ? standardFloorTile : tileCollection[Random.Range(0, tileCollection.Count)];
        }
    }
}