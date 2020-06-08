using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            onDungeonGenerated?.Invoke();
            return true;
        }

        
        // TODO: Generate doors
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
            var spawnedRooms = new List<Rect>();
            var metadata = new Dictionary<int[], string>();

            for (var i = 0; i < numberOfRooms; i++)
            {
                var randomRoomWidth = Random.Range(minWidth, maxWidth);
                var randomRoomHeight = Random.Range(minHeight, maxHeight);

                var room = new Rect(new Vector2(0, 0), new Vector2(randomRoomWidth, randomRoomHeight));

                _rooms.Add(room);
            }

            for (var i = 0; i < _rooms.Count; i++)
            {
                Rect currentRoom = _rooms[i];
                Rect spawnedRoom;

                Vector3 position = spawner.transform.position;

                if (spawnedRooms.Count > 0)
                {
                    spawnedRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count)];

                    var probability = Random.Range(0, 4);
                    MoveSpawner(probability);
                }

                var xMin = (int) math.floor(position.x);
                var xMax = (int) math.ceil(position.x + _rooms[i].width);
                var yMin = (int) math.floor(position.y);
                var yMax = (int) math.ceil(position.y + _rooms[i].height);

                GameObject roomObject = GenerateRoomObject();

                #region Generate tiles

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
                            PlaceTile(wallTileTopDown, x, y, roomObject);
                        }
                        else if (x == xMax && y > yMin && y < yMax)
                        {
                            // Generate top wall, and make room for a door
                            PlaceTile(wallTileTopDown, x, y, roomObject);
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

                #endregion

                spawnedRooms.Add(currentRoom);

                #region Local functions

                // Local function moving the spawner according to the rules.
                void MoveSpawner(int p)
                {
                    for (var u = 0; u < 50; u++)
                    {
                        switch (p)
                        {
                            case 0:
                                // Move spawner up
                                position = new Vector2(spawnedRoom.x, spawnedRoom.yMax + 1);
                                break;
                            case 1:
                                // Move spawner down
                                position = new Vector2(spawnedRoom.x, spawnedRoom.y - currentRoom.height - 1);
                                break;
                            case 2:
                                // Move spawner left
                                position = new Vector2(spawnedRoom.x - currentRoom.width - 1, spawnedRoom.y);
                                break;
                            case 3:
                                // Move spawner right
                                position = new Vector2(spawnedRoom.xMax + 1, spawnedRoom.y);
                                break;
                        }

                        currentRoom.position = position;

                        if (!IsTouchingAnotherRoom(currentRoom))
                        {
                            switch (p)
                            {
                                case 0:
                                    Debug.Log($"Moved up from {spawnedRoom.ToString()}");
                                    metadata.Add(new [] {spawnedRooms.IndexOf(spawnedRoom), i}, "N");
                                    break;
                                case 1:
                                    Debug.Log($"Moved down from {spawnedRoom.ToString()}");
                                    metadata.Add(new [] {spawnedRooms.IndexOf(spawnedRoom), i}, "S");
                                    break;
                                case 2:
                                    Debug.Log($"Moved left from {spawnedRoom.ToString()}");
                                    metadata.Add(new [] {spawnedRooms.IndexOf(spawnedRoom), i}, "W");
                                    break;
                                case 3:
                                    Debug.Log($"Moved right from {spawnedRoom.ToString()}");
                                    metadata.Add(new [] {spawnedRooms.IndexOf(spawnedRoom), i}, "E");
                                    break;
                            }

                            return;
                        }

                        p = p < 3 ? p + 1 : 0;
                    }

                    spawnedRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count)];
                    var probability = Random.Range(0, 4);
                    MoveSpawner(probability);
                    Debug.Log("Generation didn't work, recalibrating.");
                }
                
                // Places a teleporter object, which upon activation generates a new dungeon.
                

                bool IsTouchingAnotherRoom(Rect other)
                {
                    return spawnedRooms.Any(r => r.Overlaps(other));
                }

                GameObject GenerateRoomObject()
                {
                    var newObject = new GameObject
                    {
                        name = $"Room_{i}: {currentRoom.width}x{currentRoom.height}",
                        tag = "Room",
                        layer = LayerMask.NameToLayer("Rooms")
                    };
                    newObject.transform.SetParent(dungeon.transform);
                    newObject.transform.rotation = Quaternion.identity;
                    newObject.transform.position = spawner.transform.position;

                    return newObject;
                }

                #endregion
            }
            
            PlaceTeleporter();
            
            void PlaceTeleporter()
            {
                Rect firstRoom = spawnedRooms[0];
                Rect lastRoom = Rect.zero;
                var distance = new Vector2(0,0);

                foreach (Rect r in spawnedRooms)
                {
                    if ((r.center - firstRoom.center).magnitude > distance.magnitude)
                    {
                        lastRoom = r;
                        distance = r.center - firstRoom.center;
                    }
                }

                if (lastRoom != Rect.zero)
                {
                    Instantiate(UnityEngine.Resources.Load("Tiles/Teleporter"),
                        new Vector3(lastRoom.x + Random.Range(1, (int) lastRoom.width), lastRoom.y + Random.Range(1, (int) lastRoom.height), 0),
                        Quaternion.identity,
                        dungeonObject.transform);
                }
                else
                {
                    Debug.LogError("Teleporter couldn't be placed.");
                }
            }
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

        

        #region Utility
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
        #endregion
    }
}