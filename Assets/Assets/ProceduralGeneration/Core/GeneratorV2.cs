using System;
using System.Collections.Generic;
using Core;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.ProceduralGeneration.Core
{
    public class GeneratorV2 : MonoBehaviour
    {
        #region Events

        // Events

        /// <summary>
        /// Triggers when a dungeon level has been fully generated.
        /// </summary>
        public UnityEvent onDungeonGenerated;

        /// <summary>
        /// Triggers when something in an existing dungeon changes (e.g. a door opens).
        /// </summary>
        public UnityEvent onDungeonChanged;

        #endregion

        #region Editor variables

        [Header("Room Settings")] [Range(1, 30)]
        public int roomNumber;

        [Space] [SerializeField] [Range(5, 30)]
        private int minWidth;

        [SerializeField] [Range(5, 30)] private int maxWidth;

        [Space] [SerializeField] [Range(5, 30)]
        private int minHeight;

        [SerializeField] [Range(5, 30)] private int maxHeight;

        [Header("Tileset")] [Tooltip("Determines the likelihood with which the standard floor tile will spawn.")] [SerializeField] [Range(0, 100)]
        private int stdLikelihood;

        [SerializeField] private GameObject standardFloorTile;
        [SerializeField] private List<GameObject> otherFloorTiles;
        [SerializeField] private GameObject door;

        [Space]

        // Wall tiles
        [SerializeField]
        private GameObject wallTile;

        [SerializeField] private GameObject wallTileTopDown;

        [Space] [Header("Doors")] private List<int[]> _leftDoors;

        #endregion

        private List<int[]> _rightDoors;
        private Dictionary<int[], int[]> _doorRelations;
        private List<Rect> _rooms;

        [SerializeField] private GameObject dungeonObject;

        /// <summary>
        /// True, if the dungeon has finished generating.
        /// </summary>
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

        /// <summary>
        /// Generates a new dungeon with the specified number of rooms.
        /// </summary>
        /// <param name="numberOfRooms">The number of rooms that should spawn.</param>
        /// <returns>True, if everything went smoothly. Otherwise false.</returns>
        public bool Generate(int numberOfRooms)
        {
            Generated = false;

            if ((minWidth > maxWidth)
                || (minHeight > maxHeight))
            {
                Debug.LogError(
                    "Parameters were misconfigured. Check, if all your 'min..' values are less than or equal to your 'max..' values");
                return false;
            }

            dungeonObject = CreateDungeonObject();

            roomNumber = numberOfRooms;
            GenerateRooms(roomNumber);

            PlacePlayer();
            PlaceTeleporter();

            onDungeonGenerated?.Invoke();
            return true;
        }

        /// <summary>
        /// Generates the specified number of rooms (only). 
        /// </summary>
        /// <param name="numberOfRooms">The number of rooms that should spawn.</param>
        private void GenerateRooms(int numberOfRooms)
        {
            Debug.Log("Generating rooms..");

            GameObject dungeon = dungeonObject;
            GameObject spawner = CreateRoomSpawner();

            _rooms = new List<Rect>();
            _rightDoors = new List<int[]>();
            _leftDoors = new List<int[]>();
            _doorRelations = new Dictionary<int[], int[]>();

            var spawnerBacktrace = new List<Vector2>(); 
            var lastIterationDirection = string.Empty;

            for (var i = 0; i < numberOfRooms; i++)
            {
                var randomRoomWidth = Random.Range(minWidth, maxWidth);
                var randomRoomHeight = Random.Range(minHeight, maxHeight);

                var room = new Rect(new Vector2(0, 0), new Vector2(randomRoomWidth, randomRoomHeight));

                _rooms.Add(room);
            }


            for (var i = 0; i < _rooms.Count; i++)
            {
                Rect room = _rooms[i];

                Vector3 position = spawner.transform.position;
                spawnerBacktrace.Add(position);
                room.position = position;

                var xMin = (int) math.floor(position.x);
                var xMax = (int) math.ceil(position.x + _rooms[i].width);
                var yMin = (int) math.floor(position.y);
                var yMax = (int) math.ceil(position.y + _rooms[i].height);

                // Make random position for the doors
                var doorLeft = Random.Range(yMin + 1, yMax - 1);
                var doorRight = Random.Range(yMin + 1, yMax - 1);

                var roomObject = new GameObject
                {
                    name = $"Room_{i}"
                };
                roomObject.transform.SetParent(dungeon.transform);
                roomObject.transform.rotation = Quaternion.identity;
                roomObject.transform.position = spawner.transform.position;

                // Generate all room tiles
                for (var x = xMin; x <= xMax; x++)
                {
                    for (var y = yMin; y <= yMax; y++)
                    {
                        // Generate floor tiles
                        if (x > xMin && x < xMax && y > yMin && y < yMax - 1)
                        {
                            PlaceTile(GetRandomTile(), x, y, roomObject);
                        }

                        else if (x != xMin && x != xMax && y == yMax - 1)
                        {
                            PlaceTile(wallTile, x, y, roomObject);
                        }

                        // Erect vertical walls
                        else if (x == xMin && y > yMin && y < yMax)
                        {
                            // Generate bottom wall, and make room for a door
                            if (y != doorLeft)
                            {
                                PlaceTile(wallTileTopDown, x, y, roomObject);
                            }
                            else if (y == doorLeft)
                            {
                                if (_rooms.Count > 1)
                                {
                                    PlaceTile(standardFloorTile, x, y, roomObject);
                                    _leftDoors.Add(new[] {xMin, doorLeft});
                                }
                                else
                                {
                                    PlaceTile(wallTileTopDown, x, y, roomObject);
                                }
                            }
                        }
                        else if (x == xMax && y > yMin && y < yMax)
                        {
                            // Generate top wall, and make room for a door
                            if (y != doorRight)
                            {
                                PlaceTile(wallTileTopDown, x, y, roomObject);
                            }
                            else if (y == doorRight)
                            {
                                if (_rooms.Count < numberOfRooms)
                                {
                                    PlaceTile(standardFloorTile, x, y, roomObject);
                                    _rightDoors.Add(new[] {xMax, doorRight});
                                }
                                else
                                {
                                    PlaceTile(wallTileTopDown, x, y, roomObject);
                                }
                            }
                        }

                        // Erect horizontal walls
                        else if (y == yMin)
                        {
                            PlaceTile(wallTileTopDown, x, y, roomObject);
                        }
                        else if (y == yMax)
                        {
                            PlaceTile(wallTileTopDown, x, y, roomObject);
                        }
                    }
                }

                var probability = Random.Range(0f, 1f);
                
                MoveSpawner(probability);

                // Local function moving the spawner according to the rules.
                void MoveSpawner(float p)
                {
                    if (p <= 0.25)
                    {
                        if (lastIterationDirection != "down")
                        {
                            // Move spawner up
                            spawner.transform.position = (i < (numberOfRooms - 1))
                                ? new Vector2(position.x, position.y + room.height + _rooms[i + 1].height)
                                : new Vector2(position.x, position.y + room.height * 2);
                            lastIterationDirection = "up";
                            Debug.Log("Moved up");
                        }
                        else MoveSpawner(p + 0.25f);
                    }

                    else if (p <= 0.5 && p > 0.25)
                    {
                        if (lastIterationDirection != "up")
                        {
                            // Move spawner down
                            spawner.transform.position = (i < (numberOfRooms - 1))
                                ? new Vector2(position.x, position.y - room.height - _rooms[i + 1].height)
                                : new Vector2(position.x, position.y - room.height * 2);
                            lastIterationDirection = "down";
                            Debug.Log("Moved down");
                        }
                        else MoveSpawner(p + 0.25f);
                    }

                    else if (p <= 0.75 && p > 0.5)
                    {
                        if (lastIterationDirection != "right")
                        {
                            // Move spawner left
                            spawner.transform.position = (i < (numberOfRooms - 1))
                                ? new Vector2(position.x - room.width - _rooms[i + 1].width, position.y)
                                : new Vector2(position.x - room.width * 2, position.y);
                            lastIterationDirection = "left";
                            Debug.Log("Moved left");
                        }
                        else MoveSpawner(p + 0.25f);
                    }

                    else if (p <= 1 && p > 0.75)
                    {
                        if (lastIterationDirection != "left")
                        {
                            // Move spawner right
                            spawner.transform.position = (i < (numberOfRooms - 1))
                                ? new Vector2(position.x + room.width + _rooms[i + 1].width, position.y)
                                : new Vector2(position.x + room.width * 2, position.y);
                            lastIterationDirection = "right";
                            Debug.Log("Moved right");
                        }
                        else MoveSpawner(p + 0.25f);
                    }
                }

                // TODO: Local function looking for adjacent former spawner positions.
                bool SpawnerBacktrace()
                {
                    foreach (Vector2 backtracePosition in spawnerBacktrace)
                    {
                        
                    }
                    return false;
                }
            }

            for (var i = 0; i < _rightDoors.Count; i++)
            {
                _doorRelations.Add(_rightDoors[i], _leftDoors[i]);
            }

            //GenerateCorridors(dungeon);
            //GenerateDoors(dungeon);
        }

        private void OnDungeonGenerated()
        {
            Generated = true;
        }

        private void OnDungeonChanged()
        {
        }

        /// <summary>
        /// Spawns the parent Dungeon object.
        /// </summary>
        /// <returns>The DungeonParent object</returns>
        private static GameObject CreateDungeonObject()
        {
            GameObject dungeon = GameObject.Find("DungeonParent(Clone)");
            Destroy(GameObject.Find("DungeonParent"));

            if (dungeon != null) Destroy(dungeon);

            dungeon = UnityEngine.Resources.Load<GameObject>("DungeonParent");

            return Instantiate(dungeon, new Vector3(0, 0, 0), Quaternion.identity);
        }

        /// <summary>
        /// Creates the invisible object, which rooms are built around.
        /// </summary>
        /// <returns>The Spawner object.</returns>
        private static GameObject CreateRoomSpawner()
        {
            GameObject spawner = GameObject.Find("RoomSpawner");
            Destroy(GameObject.Find("RoomSpawner(Clone)"));

            if (spawner != null) Destroy(spawner);

            spawner = new GameObject {name = "RoomSpawner"};
            spawner.transform.position = new Vector2(0, 0);

            return Instantiate(spawner);
        }

        /// <summary>
        /// Places a tile at the specified location and parent.
        /// </summary>
        /// <param name="tile">A tile resource.</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="parent">The parent object</param>
        /// <returns>The placed Tile</returns>
        public static GameObject PlaceTile(GameObject tile, int x, int y, GameObject parent)
        {
            return Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, parent.transform);
        }

        /// <summary>
        /// Sets the player's position to be in the left corner of the first room.
        /// </summary>
        private void PlacePlayer()
        {
            Rect firstRoom = _rooms[0];
            //GameManager.GetPlayer().transform.position = new Vector3(firstRoom.x + 2, firstRoom.y + 2, 0);
        }

        /// <summary>
        /// Places a teleporter object, which upon activation generates a new dungeon.
        /// </summary>
        private void PlaceTeleporter()
        {
            Rect lastRoom = _rooms[_rooms.Count - 1];

            Instantiate(UnityEngine.Resources.Load("Tiles/Teleporter"),
                new Vector3(lastRoom.x + Random.Range(0, (int) lastRoom.width), lastRoom.y + Random.Range(0, (int) lastRoom.height), 0),
                Quaternion.identity,
                dungeonObject.transform);
        }

        /// <summary>
        /// Gets a random GameObject from a pool of GameObjects using the Generator's stdLikelihood value.
        /// </summary>
        /// <returns></returns>
        public GameObject GetRandomTile()
        {
            var random = Random.Range((stdLikelihood + 1) - 100, stdLikelihood);
            return random > 0 ? standardFloorTile : otherFloorTiles[Random.Range(0, otherFloorTiles.Count)];
        }

        /// <summary>
        /// Returns the DungeonParent object
        /// </summary>
        /// <returns>The DungeonParent object</returns>
        public GameObject GetDungeonParent()
        {
            return dungeonObject;
        }

        /// <summary>
        /// Returns a list of rooms inside a level.
        /// </summary>
        /// <returns>A list of rooms.</returns>
        public IEnumerable<Rect> GetRooms()
        {
            return this._rooms;
        }
    }
}