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
        private GeneratorV2 _generator;

        private void Start()
        {
            _generator = GameManager.GetGenerator();
            _generator.onDungeonGenerated.AddListener(Decorate);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Decorate()
        {
            GameObject enemyDecorator = CreateEnemyDecorator();
            
            foreach (Rect room in _generator.GetRooms())
            {
                Spawn("Slime", 
                    GetRandomPosition((int) room.x+1, (int) room.xMax, (int) room.y+1, (int) room.yMax), 
                    enemyDecorator.transform);

                var numberOfEnemies = room.width * room.height / 20;

                for (int i = 0; i < numberOfEnemies; i++)
                {
                    if (GetProbability(10))
                    {
                        Spawn("Slime", 
                            GetRandomPosition((int) room.x+1, (int) room.xMax, (int) room.y+1, (int) room.yMax), 
                            enemyDecorator.transform);
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