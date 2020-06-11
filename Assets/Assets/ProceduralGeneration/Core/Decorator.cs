﻿using System.Collections.Generic;
using System.Linq;
using Core;
using Unity.Burst;
using UnityEngine;

namespace Assets.ProceduralGeneration.Core
{
    /// <summary>
    /// The PCG's
    /// </summary>
    [BurstCompile]
    public class Decorator : MonoBehaviour
    {
        public float spawnRateInPercent;

        public List<GameObject> enemies;
        
        private GeneratorV2 _generator;

        private void Start()
        {
            _generator = GameManager.GetGenerator();
            _generator.onDungeonGenerated.AddListener(Decorate);
        }
        
        private void Decorate()
        {
            Debug.Log("Decorating..");
            GameObject enemyDecorator = CreateEnemyDecorator();

            var spawnedRooms = _generator.GetSpawnedRooms();
            
            if (spawnedRooms == null) return;
            
            foreach (Rect room in spawnedRooms)
            {
                Debug.Log($"Spawning enemies in {room.ToString()}");
                SpawnRandomly(GetRandomEnemy(), room, enemyDecorator.transform);
                
                var numberOfEnemies = room.width * room.height / 10;
                
                for (var i = 0; i < numberOfEnemies; i++)
                {
                    if (GetProbability(spawnRateInPercent))
                    {
                        SpawnRandomly(GetRandomEnemy(), room, enemyDecorator.transform);
                    }
                }
            }
            
            Debug.Log("Dungeon decorated.");
        }

        private GameObject Spawn (string resourcesPath, Vector2 position, Transform parent)
        {
            return (GameObject) Instantiate(UnityEngine.Resources.Load(resourcesPath), 
                new Vector3(position.x, position.y, 0),
                Quaternion.identity,
                parent);
        }
        
        private GameObject Spawn (GameObject prefab, Vector2 position, Transform parent)
        {
            return Instantiate(prefab, 
                new Vector3(position.x, position.y, 0),
                Quaternion.identity,
                parent);
        }
        
        private GameObject SpawnRandomly (string resourcesPath, Rect room, Transform parent)
        {
            return (GameObject) Instantiate(UnityEngine.Resources.Load(resourcesPath),
                GetRandomPosition((int) room.x+1, (int) room.xMax, (int) room.y+1, (int) room.yMax),
                Quaternion.identity,
                parent);
        }
        
        private GameObject SpawnRandomly (GameObject prefab, Rect room, Transform parent)
        {
            return Instantiate(prefab,
                GetRandomPosition((int) room.x+1, (int) room.xMax, (int) room.y+1, (int) room.yMax),
                Quaternion.identity,
                parent);
        }

        private GameObject GetRandomEnemy()
        {
            return enemies?[Random.Range(0, enemies.Count)];
        }

        private Vector2 GetRandomPosition(int leftXBoundary, int rightXBoundary, int leftYBoundary, int rightYBoundary) 
        {
            return new Vector2(Random.Range(leftXBoundary, rightXBoundary), Random.Range(leftYBoundary, rightYBoundary));
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

        private bool GetProbability(float pInPercent)
        {
            return Random.Range(0f, 1f) <= pInPercent / 100;
        }
    }
}