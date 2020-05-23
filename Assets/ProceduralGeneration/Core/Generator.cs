using System.Collections.Generic;
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
        public GameObject wallTileTopDown;

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

            GenerateRooms(roomNumber = 10);

            InstantiatePlayer();

            return true;
        }
        
        public bool Generate(int numberOfRooms)
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

            GenerateRooms(numberOfRooms);

            InstantiatePlayer();

            return true;
        }

        private void GenerateRooms(int numberOfRooms)
        {
            Debug.Log("Generating rooms..");

            GameObject dungeon = CreateDungeonObject();
            GameObject spawner = CreateRoomSpawner();

            _rooms = new List<Rect>();
            rightDoors = new List<int[]>();
            leftDoors = new List<int[]>();
            doorRelations = new Dictionary<int[], int[]>();

            for (var i = 0; i < numberOfRooms; i++)
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
                            PlaceTile(wallTileTopDown, x, y, dungeon);
                        }

                        // Northeast corner
                        else if (x == xMax && y == yMax)
                        {
                            PlaceTile(wallTileTopDown, x, y, dungeon);
                        }

                        // Southwest corner
                        else if (x == xMin && y == yMin)
                        {
                            PlaceTile(wallTileTopDown, x, y, dungeon);
                        }

                        // Southeast corner
                        else if (x == xMax && y == yMin)
                        {
                            PlaceTile(wallTileTopDown, x, y, dungeon);
                        }

                        // Erect vertical walls
                        else if (x == xMin && y > yMin && y < yMax)
                        {
                            // Generate bottom wall, and make room for a door
                            if (y != doorLeft)
                            {
                                PlaceTile(wallTileTopDown, x, y, dungeon);
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
                                    PlaceTile(wallTileTopDown, x, y, dungeon);
                                }
                            }
                        }
                        else if (x == xMax && y > yMin && y < yMax)
                        {
                            // Generate top wall, and make room for a door
                            if (y != doorRight)
                            {
                                PlaceTile(wallTileTopDown, x, y, dungeon);
                            }
                            else if (y == doorRight)
                            {
                                if (_rooms.Count < numberOfRooms)
                                {
                                    PlaceTile(standardFloorTile, x, y, dungeon);
                                    rightDoors.Add(new[] {xMax, doorRight});
                                }
                                else
                                {
                                    PlaceTile(wallTileTopDown, x, y, dungeon);
                                }
                            }
                        }

                        // Erect horizontal walls
                        else if (y == yMin)
                        {
                            PlaceTile(wallTileTopDown, x, y, dungeon);
                        }
                        else if (y == yMax)
                        {
                            PlaceTile(wallTileTopDown, x, y, dungeon);
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

                int xBuffer = 0;

                int width = math.abs(rightDoorX - leftDoorX);
                int height = rightDoorY - leftDoorY;
                int middle = (int) math.floor(rightDoorX - width / 2);

                for (var x = leftDoorX + 1; x < middle; x++)
                {
                    if (width > 1)
                    {
                        if (x < middle - 1)
                        {
                            // Place a wall directly above the corridor
                            PlaceTile(wallTile, x, leftDoorY + 1, dungeon);
                            // Place a dark tile above that
                            PlaceTile(wallTileTopDown, x, leftDoorY + 2, dungeon);
                        }

                        // Place a dark tile directly below the corridor
                        PlaceTile(wallTileTopDown, x, leftDoorY - 1, dungeon);
                    }

                    // Place floor tile
                    PlaceTile(standardFloorTile, x, leftDoorY, dungeon);

                    xBuffer = x + 1;
                }

                if (height > 0)
                {
                    // Place a corner tile
                    PlaceTile(wallTileTopDown, xBuffer, leftDoorY - 1, dungeon);
                    // Place two walls to complete the lower-right corner
                    PlaceTile(wallTileTopDown, xBuffer + 1, leftDoorY, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer + 1, leftDoorY - 1, dungeon);
                    
                    for (var y = leftDoorY; y < rightDoorY; y++)
                    {
                        if (width > 1 && y > leftDoorY)
                        {
                            if (xBuffer > leftDoorX)
                            {
                                PlaceTile(wallTileTopDown, xBuffer - 1, y, dungeon);
                            }

                            if (xBuffer < rightDoorX)
                            {
                                PlaceTile(wallTileTopDown, xBuffer + 1, y, dungeon);
                            }
                        }

                        PlaceTile(standardFloorTile, xBuffer, y, dungeon);
                    }
                    
                    // Place seven walls to complete the upper-left corner
                    PlaceTile(wallTileTopDown, xBuffer - 1, rightDoorY, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer - 1, rightDoorY + 1, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer - 1, rightDoorY + 2, dungeon);
                    
                    PlaceTile(wallTileTopDown, xBuffer, rightDoorY + 2, dungeon);
                    PlaceTile(wallTile, xBuffer, rightDoorY + 1, dungeon);
                    
                    PlaceTile(wallTileTopDown, xBuffer + 1, rightDoorY + 2, dungeon);
                    PlaceTile(wallTile, xBuffer + 1, rightDoorY + 1, dungeon);
                }
                
                else if (height < 0)
                {
                    // Place walls to complete the upper-right corner
                    PlaceTile(wallTileTopDown, xBuffer - 1, leftDoorY + 2, dungeon);
                    PlaceTile(wallTile, xBuffer - 1, leftDoorY + 1, dungeon);
                    
                    PlaceTile(wallTileTopDown, xBuffer, leftDoorY + 2, dungeon);
                    PlaceTile(wallTile, xBuffer, leftDoorY + 1, dungeon);
                    
                    PlaceTile(wallTileTopDown, xBuffer + 1, leftDoorY + 2, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer + 1, leftDoorY + 1, dungeon);
                    
                    // Place a corner tile
                    PlaceTile(wallTileTopDown, xBuffer + 1, leftDoorY, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer + 1, leftDoorY - 1, dungeon);
                    
                    for (var y = leftDoorY; y > rightDoorY; y--)
                    {
                        if (width > 1 && y > rightDoorY + 1)
                        {
                            if (xBuffer > leftDoorX)
                            {
                                PlaceTile(wallTileTopDown, xBuffer - 1, y, dungeon);
                            }

                            if (xBuffer < rightDoorX)
                            {
                                PlaceTile(wallTileTopDown, xBuffer + 1, y, dungeon);
                            }
                        }

                        PlaceTile(standardFloorTile, xBuffer, y, dungeon);
                    }
                    
                    // Place walls to complete the lower-left corner
                    PlaceTile(wallTileTopDown, xBuffer - 1, rightDoorY + 1, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer - 1, rightDoorY, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer - 1, rightDoorY - 1, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer, rightDoorY - 1, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer + 1, rightDoorY - 1, dungeon);
                    PlaceTile(wallTile, xBuffer + 1, rightDoorY + 1, dungeon);
                }
                
                else if (height == 0)
                {
                    // Place walls to complete a straight corridor
                    PlaceTile(wallTileTopDown, xBuffer - 1, leftDoorY + 2, dungeon);
                    PlaceTile(wallTile, xBuffer - 1, leftDoorY + 1, dungeon);
                    
                    PlaceTile(wallTileTopDown, xBuffer, leftDoorY + 2, dungeon);
                    PlaceTile(wallTile, xBuffer, leftDoorY + 1, dungeon);
                    
                    PlaceTile(wallTileTopDown, xBuffer + 1, leftDoorY + 2, dungeon);
                    PlaceTile(wallTile, xBuffer + 1, leftDoorY + 1, dungeon);
                    
                    PlaceTile(wallTileTopDown, xBuffer, leftDoorY - 1, dungeon);
                    PlaceTile(wallTileTopDown, xBuffer + 1, leftDoorY - 1, dungeon);
                }
                
                // Get to the y of the right door
                PlaceTile(standardFloorTile, xBuffer, rightDoorY, dungeon);
                PlaceTile(standardFloorTile, xBuffer + 1, rightDoorY, dungeon);
                
                for (var x = xBuffer + 2; x < rightDoorX; x++)
                {
                    if (width > 1)
                    {
                        if (x > middle - 1)
                        {
                            // Place a wall directly above the corridor
                            PlaceTile(wallTile, x, rightDoorY + 1, dungeon);
                            // Place a dark tile above that
                            PlaceTile(wallTileTopDown, x, rightDoorY + 2, dungeon);
                        }

                        // Place a dark tile directly below the corridor
                        PlaceTile(wallTileTopDown, x, rightDoorY - 1, dungeon);
                    }

                    PlaceTile(standardFloorTile, x, rightDoorY, dungeon);

                    xBuffer += 1;
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
            GameObject spawner = GameObject.Find("RoomSpawner");
            Destroy(GameObject.Find("RoomSpawner(Clone)"));
            
            GameObject player = GameObject.Find("Player(Clone)");
            Destroy(GameObject.Find("Player"));

            if (player != null)
            {
                Destroy(player);
            }

            player = (GameObject) Instantiate(Resources.Load("Player/Player"),
                new Vector3(_rooms[0].x + 1, _rooms[0].y + 1, 0), Quaternion.identity, gameObject.transform);

            return player;
        }

        private GameObject GetRandomTile(IReadOnlyList<GameObject> tileCollection)
        {
            var random = Random.Range((stdLikelihood + 1) - 100, stdLikelihood);

            return random > 0 ? standardFloorTile : tileCollection[Random.Range(0, tileCollection.Count)];
        }
    }
}