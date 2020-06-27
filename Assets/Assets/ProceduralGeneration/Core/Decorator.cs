using System;
using System.Collections.Generic;
using Assets.Items.Scripts;
using Assets.PickUps.Scripts;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.ProceduralGeneration.Core
{
    /// <summary>
    /// The PCG's secondary main script. It brings life to the dungeon.
    /// </summary>
    public class Decorator : MonoBehaviour
    {
        #region Public variables

        [Header("Enemy settings")]
        [SerializeField] private float enemySpawnRateInPercent;
        [SerializeField] private float roomPerEnemy;
        [SerializeField] private List<GameObject> enemies;

        [Space]
        [Header("Item settings")]
        [SerializeField] private float itemSpawnRateInPercent;
        [SerializeField] private int maxItemsPerRoom;
        [Utility.ReadOnly] public int globalItemCount;
        [SerializeField] private List<string> items;
        

        [Space]
        [Header("Storystone settings")]
        [SerializeField] private GameObject storyStone;
        [SerializeField] private int storyStonesPerLevel;
        [Utility.ReadOnly] public int storyStonesInLevel;
        [SerializeField] private float storyStonesProbability;

        [Space]
        [Header("Other settings")]
        [SerializeField] private float minimumDistanceToPlayer;

        #endregion

        #region Private variables

        private Generator _generator;

        private int _levelItemCount;

        #endregion

        #region UnityEvent functions

        private void Start()
        {
            _generator = GameManager.GetGenerator();
            GameManager.GetEventHandler().onDungeonGenerated.AddListener(Decorate);

            items = GameItem.GetItemNames();

            globalItemCount = 0;
        }

        #endregion

        #region Decoration

        /// <summary>
        /// Uses data from the generator to bring life to the dungeon.
        /// </summary>
        private void Decorate()
        {
            Debug.Log("Decorating..");
            GameObject enemyDecorator = CreateEnemyDecorator();
            GameObject itemDecorator = CreateItemDecorator();
            _levelItemCount = 0;

            var spawnedRooms = _generator.GetSpawnedRooms();

            if (spawnedRooms == null) return;

            foreach (Rect room in spawnedRooms)
            {
                SpawnEnemies(room, enemyDecorator.transform);
                SpawnItems(room, itemDecorator.transform);
                SpawnStoryStones(room);
            }

            Debug.Log("Dungeon decorated.");
        }

        private void SpawnEnemies(Rect room, [NotNull] Transform enemyDecorator)
        {
            if (enemyDecorator == null) throw new ArgumentNullException(nameof(enemyDecorator));

            //Debug.Log($"Spawning enemies in {room.ToString()}");
            SpawnRandomly(GetRandomEnemy(), room, enemyDecorator);

            var numberOfEnemies = room.width * room.height / roomPerEnemy;

            for (var i = 1; i < numberOfEnemies; i++)
            {
                if (GetProbability(enemySpawnRateInPercent))
                {
                    SpawnRandomlyAwayFromPlayer(GetRandomEnemy(), room, enemyDecorator);
                }
            }
        }

        private void SpawnItems(Rect room, [NotNull] Transform itemDecorator)
        {
            for (var i = 0; i < maxItemsPerRoom; i++)
            {
                if (GetProbability(itemSpawnRateInPercent))
                {
                    GameObject pickUp = SpawnRandomlyAwayFromPlayer("PickUp", room, itemDecorator);
                    pickUp.GetComponent<PickUpScript>().SetPickUpItem(GetRandomItemName());

                    _levelItemCount++;
                    globalItemCount++;
                }
            }
        }

        private void SpawnStoryStones(Rect room)
        {
            var count = 0;
            
            for (var i = storyStonesInLevel; i < storyStonesPerLevel; i++)
            {
                if (GetProbability(storyStonesProbability))
                {
                    SpawnRandomlyAwayFromPlayer(storyStone, room);
                    count++;
                }
            }

            storyStonesInLevel += count;
        }

        #endregion

        #region Utility

        /// <summary>
        /// Spawns a new enemy at the given position
        /// </summary>
        /// <param name="resourcesPath">Path of the enemy prefab</param>
        /// <param name="position">Position to spawn at</param>
        /// <param name="parent">Parent object</param>
        /// <returns></returns>
        private GameObject Spawn(string resourcesPath, Vector2 position, Transform parent = null)
        {
            return (GameObject) Instantiate(UnityEngine.Resources.Load(resourcesPath),
                new Vector3(position.x, position.y, 0),
                Quaternion.identity,
                parent);
        }

        private GameObject Spawn(GameObject prefab, Vector2 position, Transform parent = null)
        {
            return Instantiate(prefab,
                new Vector3(position.x, position.y, 0),
                Quaternion.identity,
                parent);
        }

        private GameObject SpawnRandomly(string resourcesPath, Rect room, Transform parent = null)
        {
            return (GameObject) Instantiate(UnityEngine.Resources.Load(resourcesPath),
                GetRandomPosition((int) room.x + 1, (int) room.xMax, (int) room.y + 1, (int) room.yMax),
                Quaternion.identity,
                parent);
        }

        private GameObject SpawnRandomly(GameObject prefab, Rect room, Transform parent = null)
        {
            return Instantiate(prefab,
                GetRandomPosition((int) room.x + 1, (int) room.xMax, (int) room.y + 1, (int) room.yMax),
                Quaternion.identity,
                parent);
        }

        private GameObject SpawnRandomlyAwayFromPlayer(string resourcesPath, Rect room, Transform parent = null)
        {
            return (GameObject) Instantiate(UnityEngine.Resources.Load(resourcesPath),
                GetRandomPositionAwayFromPlayer((int) room.x + 1, (int) room.xMax, (int) room.y + 1, (int) room.yMax),
                Quaternion.identity,
                parent);
        }

        private GameObject SpawnRandomlyAwayFromPlayer(GameObject prefab, Rect room, Transform parent = null)
        {
            return Instantiate(prefab,
                GetRandomPositionAwayFromPlayer((int) room.x + 1, (int) room.xMax, (int) room.y + 1, (int) room.yMax),
                Quaternion.identity,
                parent);
        }

        private GameObject GetRandomEnemy()
        {
            return enemies?[Random.Range(0, enemies.Count)];
        }

        private string GetRandomItemName()
        {
            return items?[Random.Range(0, items.Count)];
        }

        private Vector2 GetRandomPosition(int leftXBoundary, int rightXBoundary, int leftYBoundary, int rightYBoundary)
        {
            return new Vector2(Random.Range(leftXBoundary, rightXBoundary), Random.Range(leftYBoundary, rightYBoundary));
        }

        private Vector2 GetRandomPositionAwayFromPlayer(int leftXBoundary, int rightXBoundary, int leftYBoundary, int rightYBoundary)
        {
            Vector2 playerPosition = GameManager.GetUnityPlayerObject().transform.position;
            var spawnPosition = new Vector2(Random.Range(leftXBoundary, rightXBoundary), Random.Range(leftYBoundary, rightYBoundary));
            var distanceToPlayer = (playerPosition - spawnPosition).magnitude;

            while (distanceToPlayer < minimumDistanceToPlayer)
            {
                spawnPosition = new Vector2(Random.Range(leftXBoundary, rightXBoundary), Random.Range(leftYBoundary, rightYBoundary));
                distanceToPlayer = (playerPosition - spawnPosition).magnitude;
            }

            return spawnPosition;
        }

        private GameObject CreateEnemyDecorator()
        {
            GameObject decorator = GameObject.Find("Enemies(Clone)");
            Destroy(GameObject.Find("Enemies"));

            if (decorator != null) Destroy(decorator);

            decorator = new GameObject()
            {
                name = "Enemies",
            };

            return Instantiate(decorator, new Vector3(0, 0, 0), Quaternion.identity);
        }

        private GameObject CreateItemDecorator()
        {
            GameObject decorator = GameObject.Find("Items(Clone)");
            Destroy(GameObject.Find("Items"));

            if (decorator != null) Destroy(decorator);

            decorator = new GameObject()
            {
                name = "Items",
            };

            return Instantiate(decorator, new Vector3(0, 0, 0), Quaternion.identity);
        }

        private bool GetProbability(float pInPercent)
        {
            return Random.Range(0f, 1f) <= pInPercent / 100;
        }

        #endregion
    }
}