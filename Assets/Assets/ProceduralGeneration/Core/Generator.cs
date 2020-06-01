using System;
using System.Collections.Generic;
using Core;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.ProceduralGeneration.Core
{
    [BurstCompile]
    public class Generator : MonoBehaviour
    {
        // Events

        public UnityEvent onDungeonGenerated;
        public UnityEvent onDungeonChanged;
    
        [Header("Room Settings")] 
        [Range(1, 30)] public int RoomNumber;

        [Space] 
        [SerializeField]
        [Range(5, 30)] private int minWidth;
        [SerializeField]
        [Range(5, 30)] private int maxWidth;
        
        [Space] 
        [SerializeField]
        [Range(5, 30)] private int minHeight;
        [SerializeField]
        [Range(5, 30)] private int maxHeight;

        [Space] 
        [SerializeField]
        [Range(4, 40)] private int minSpacingX;
        [SerializeField]
        [Range(4, 40)] private int maxSpacingX;
        
        [Space] 
        [SerializeField]
        [Range(-80, 0)] private int minSpacingY;
        [SerializeField]
        [Range(0, 80)] private int maxSpacingY;

        [Header("Tileset")]
        [Tooltip("Determines the likelihood with which the standard floor tile will spawn.")]
        [SerializeField]
        [Range(0, 100)] private int stdLikelihood;

        [SerializeField] private GameObject standardFloorTile;
        [SerializeField] private List<GameObject> otherFloorTiles;
        [SerializeField] private GameObject door;

        [Space]

        // Wall tiles
        [SerializeField] private GameObject wallTile;
        [SerializeField] private GameObject wallTileTopDown;

        [Space] 
        [Header("Doors")]
        private List<int[]> _leftDoors;

        private List<int[]> _rightDoors;
        private Dictionary<int[], int[]> _doorRelations;
        [Space] 
        private List<Rect> _rooms;

        [SerializeField] private GameObject dungeonObject;

        public bool Generated { get; set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            onDungeonGenerated = new UnityEvent();
            onDungeonChanged = new UnityEvent();
        }

        private void Start()
        {
            onDungeonGenerated.AddListener(OnDungeonGenerated);
            onDungeonChanged.AddListener(OnDungeonChanged);
        }

        public bool Generate(int numberOfRooms)
        {
            Generated = false;
            
            if ((minWidth > maxWidth)
                || (minHeight > maxHeight)
                || (minSpacingX > maxSpacingX)
                || (minSpacingY > maxSpacingY))
            {
                Debug.LogError(
                    "Parameters were misconfigured. Check, if all your 'min..' values are less than or equal to your 'max..' values");
                return false;
            }

            dungeonObject = CreateDungeonObject();

            RoomNumber = numberOfRooms;
            GenerateRooms(RoomNumber);

            PlacePlayer();
            PlaceTeleporter();

            onDungeonGenerated?.Invoke();
            return true;
        }

        private void GenerateRooms(int numberOfRooms)
        {
            Debug.Log("Generating rooms..");

            GameObject dungeon = this.dungeonObject;
            GameObject spawner = CreateRoomSpawner();

            _rooms = new List<Rect>();
            _rightDoors = new List<int[]>();
            _leftDoors = new List<int[]>();
            _doorRelations = new Dictionary<int[], int[]>();

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
                                    _leftDoors.Add(new[] {xMin, doorLeft});
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
                                    _rightDoors.Add(new[] {xMax, doorRight});
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

            for (var i = 0; i < _rightDoors.Count; i++)
            {
                _doorRelations.Add(_rightDoors[i], _leftDoors[i]);
            }

            GenerateCorridors(dungeon);
            GenerateDoors(dungeon);
        }

        private void GenerateCorridors(GameObject dungeon)
        {
            Debug.Log("Generating corridors..");

            foreach (var doorRelation in _doorRelations)
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
                    
                    
                    PlaceTile(standardFloorTile, xBuffer, leftDoorY, dungeon);

                    for (var y = leftDoorY + 1; y < rightDoorY; y++)
                    {
                        if (width > 1 && y < rightDoorY)
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
                    
                    PlaceTile(standardFloorTile, xBuffer, leftDoorY, dungeon);

                    for (var y = leftDoorY - 1; y > rightDoorY; y--)
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

        private void GenerateDoors(GameObject dungeon)
        {
            foreach (var leftDoor in _leftDoors)
            {
                PlaceTile(door, leftDoor[0], leftDoor[1], dungeon);
            }
            
            foreach (var rightDoor in _rightDoors)
            {
                PlaceTile(door, rightDoor[0], rightDoor[1], dungeon);
            }
        }

        private void OnDungeonGenerated()
        {
            Generated = true;
        }

        private void OnDungeonChanged()
        {
            
        }

        private static GameObject CreateDungeonObject()
        {
            GameObject dungeon = GameObject.Find("Dungeon(Clone)");
            Destroy(GameObject.Find("Dungeon"));

            if (dungeon != null) Destroy(dungeon);

            dungeon = UnityEngine.Resources.Load<GameObject>("DungeonParent");

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
            //_tilesSurfaces.Add(tile.GetComponent<NavMeshSurface>());
            return Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, parent.transform);
        }

        private void PlacePlayer()
        {
            Rect firstRoom = _rooms[0];
            GameManager.GetPlayer().transform.position = new Vector3(firstRoom.x + 2, firstRoom.y + 2, 0);
        }

        private void PlaceTeleporter()
        {
            Rect lastRoom = _rooms[_rooms.Count - 1];
            
            Instantiate(UnityEngine.Resources.Load("Tiles/Teleporter"), 
                new Vector3(lastRoom.x + Random.Range(0, (int) lastRoom.width), lastRoom.y + Random.Range(0, (int) lastRoom.height), 0), 
                Quaternion.identity, 
                dungeonObject.transform);
        }

        private GameObject GetRandomTile(IReadOnlyList<GameObject> tileCollection)
        {
            var random = Random.Range((stdLikelihood + 1) - 100, stdLikelihood);
            return random > 0 ? standardFloorTile : tileCollection[Random.Range(0, tileCollection.Count)];
        }

        public GameObject GetParent()
        {
            return dungeonObject;
        }

        public List<Rect> GetRooms()
        {
            return this._rooms;
        }
    }
}