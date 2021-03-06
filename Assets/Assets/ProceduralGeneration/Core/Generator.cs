﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.ProceduralGeneration.Core
{
    public class Generator : MonoBehaviour
    {
        #region Events

        // Events

        /// <summary>
        /// Triggers when a dungeon level has been fully generated.
        /// </summary>
        

        /// <summary>
        /// Triggers when something in an existing dungeon changes (e.g. a door opens).
        /// </summary>
        

        #endregion

        #region Editor variables

        [Header("Room Settings")] 
        [Range(1, 30)] public int minRooms;
        [Range(1, 30)] public int maxRooms;

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

        [Space]

        // Wall tiles
        [SerializeField]
        private GameObject wallTile;

        [SerializeField] private GameObject wallTileTopDown;

        #endregion

        #region Class-specific variables

        private List<Rect> _rooms;
        private List<Rect> _spawnedRooms;

        [SerializeField] private GameObject dungeonObject;

        /// <summary>
        /// True, if the dungeon has finished generating.
        /// </summary>
        public bool Generated { get; set; }

        public GameObject teleporter;

        #endregion

        #region UnityEvent functions

        private void Start()
        {
            GameManager.GetEventHandler().onDungeonGenerated.AddListener(OnDungeonGenerated);
            GameManager.GetEventHandler().onDungeonChanged.AddListener(OnDungeonChanged);

            teleporter = null;
        }

        #endregion

        #region Generation

        /// <summary>
        /// Generates a new dungeon with the specified number of rooms.
        /// </summary>
        /// <param name="numberOfRooms">The number of rooms that should spawn.</param>
        /// <returns>True, if everything went smoothly. Otherwise false.</returns>
        public bool Generate(int numberOfRooms)
        {
            Generated = false;
            Time.timeScale = 0f;

            if ((minWidth > maxWidth)
                || (minHeight > maxHeight)
                || (minRooms > maxRooms))
            {
                Debug.LogError(
                    "Parameters were misconfigured. Check, if all your 'min..' values are less than or equal to your 'max..' values");
                return false;
            }

            dungeonObject = CreateDungeonObject();

            maxRooms = numberOfRooms;
            GenerateRooms(maxRooms);

            PlacePlayer();

            GameManager.GetEventHandler().onDungeonGenerated?.Invoke();
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
            _spawnedRooms = new List<Rect>();
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

                if (_spawnedRooms.Count > 0)
                {
                    spawnedRoom = _spawnedRooms[Random.Range(0, _spawnedRooms.Count)];

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

                _spawnedRooms.Add(currentRoom);

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
                                    //Debug.Log($"Moved up from {spawnedRoom.ToString()}");
                                    metadata.Add(new[] {_spawnedRooms.IndexOf(spawnedRoom), i}, "N");
                                    break;
                                case 1:
                                    //Debug.Log($"Moved down from {spawnedRoom.ToString()}");
                                    metadata.Add(new[] {_spawnedRooms.IndexOf(spawnedRoom), i}, "S");
                                    break;
                                case 2:
                                    //Debug.Log($"Moved left from {spawnedRoom.ToString()}");
                                    metadata.Add(new[] {_spawnedRooms.IndexOf(spawnedRoom), i}, "W");
                                    break;
                                case 3:
                                    //Debug.Log($"Moved right from {spawnedRoom.ToString()}");
                                    metadata.Add(new[] {_spawnedRooms.IndexOf(spawnedRoom), i}, "E");
                                    break;
                            }

                            return;
                        }

                        p = p < 3 ? p + 1 : 0;
                    }

                    spawnedRoom = _spawnedRooms[Random.Range(0, _spawnedRooms.Count)];
                    var probability = Random.Range(0, 4);
                    MoveSpawner(probability);
                    //Debug.Log("Generation didn't work, recalibrating.");
                }
                


                bool IsTouchingAnotherRoom(Rect other)
                {
                    return _spawnedRooms.Any(r => r.Overlaps(other));
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

            teleporter = PlaceTeleporter();
            PlaceDoors();

            #region Local functions

            void PlaceDoors()
            {
                foreach (Rect r in _spawnedRooms)
                {
                    // Increase the size of the room
                    var rect = new Rect(new Vector2(r.x - 0.1f, r.y - 0.1f), new Vector2(r.width + 1.2f, r.height + 1.2f));
                    // Look for overlapping rooms
                    var overlaps = _spawnedRooms.Where(r2 => rect.Overlaps(r2)).ToList();

                    //Debug.Log($"{r.ToString()} overlaps {overlaps.Count} rooms.");

                    foreach (Rect overlap in overlaps)
                    {
                        var angleUpDown = Vector2.Angle(overlap.center - r.center, Vector2.up);
                        var angleLeftRight = Vector2.Angle(overlap.center - r.center, Vector2.left);

                        if ((angleUpDown < 60 || angleUpDown > 120) ^ (angleLeftRight < 45 || angleLeftRight > 135))
                        {
                            var results = new RaycastHit2D[100];
                            var size = Physics2D.LinecastNonAlloc(
                                r.center,
                                overlap.center,
                                results,
                                LayerMask.GetMask("Walls"),
                                0,
                                0);
                            //Debug.Log($"Linecast hit {size} objects");

                            if (size > 0)
                            {
                                //Debug.Log("Linecast hit something.");
                                foreach (RaycastHit2D result in results)
                                {
                                    if (result.transform == null) continue;

                                    //Debug.Log("Detected walls.");
                                    GameObject hit = result.transform.gameObject;
                                    if (hit.CompareTag("Wall"))
                                    {
                                        //Debug.Log("Destroyed wall.");
                                        PlaceTile(GetRandomTile(), hit.transform.position, dungeon);
                                        Destroy(hit);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            GameObject PlaceTeleporter()
            {
                Rect firstRoom = _spawnedRooms[0];
                Rect lastRoom = Rect.zero;
                var distance = new Vector2(0, 0);

                foreach (Rect r in _spawnedRooms)
                {
                    if (Vector2.Distance(firstRoom.center, r.center) > distance.magnitude)
                    {
                        lastRoom = r;
                        distance = r.center - firstRoom.center;
                    }
                }

                if (lastRoom != Rect.zero)
                {
                    return (GameObject) Instantiate(UnityEngine.Resources.Load("Tiles/Teleporter"),
                        new Vector3(lastRoom.x + Random.Range(1, (int) lastRoom.width), lastRoom.y + Random.Range(1, (int) lastRoom.height), 0),
                        Quaternion.identity,
                        dungeonObject.transform);
                }

                Debug.LogError("Teleporter couldn't be placed.");

                return null;
            }

            #endregion
        }
        

        #endregion

        #region Event subscriptions

        private void OnDungeonGenerated()
        {
            Debug.Log("Dungeon generated event invoked.");
            Generated = true;
            Time.timeScale = 1f;
        }

        private void OnDungeonChanged()
        {
        }

        #endregion

        #region Object creations

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
        /// Sets the player's position to be in the left corner of the first room.
        /// </summary>
        private void PlacePlayer()
        {
            Rect firstRoom = _rooms[0];
            GameManager.GetUnityPlayerObject().transform.position = new Vector3(firstRoom.x + 2, firstRoom.y + 2, 0);
        }

        #endregion

        #region Utility

        /// <summary>
        /// Places a tile at the specified location and parent.
        /// </summary>
        /// <param name="tile">A tile resource.</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="parent">The parent object</param>
        /// <returns>The placed Tile</returns>
        private static GameObject PlaceTile(GameObject tile, int x, int y, GameObject parent)
        {
            return Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, parent.transform);
        }

        /// <summary>
        /// Places a tile at the specified location and parent.
        /// </summary>
        /// <param name="tile">A tile resource.</param>
        /// <param name="pos">Position vector</param>
        /// <param name="parent">The parent object</param>
        /// <returns>The placed Tile</returns>
        private static GameObject PlaceTile(GameObject tile, Vector2 pos, GameObject parent)
        {
            return Instantiate(tile, new Vector3(pos.x, pos.y, 0), Quaternion.identity, parent.transform);
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

        public IEnumerable<Rect> GetSpawnedRooms()
        {
            return _spawnedRooms;
        }

        public void DestroyDungeon()
        {
            if (dungeonObject != null) Destroy(dungeonObject);
        }

        #endregion
    }
}