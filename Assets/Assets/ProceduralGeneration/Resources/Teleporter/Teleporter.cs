using Assets.ProceduralGeneration.Core;
using Core;
using Unity.Burst;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Assets.ProceduralGeneration.Resources.Teleporter
{
    [BurstCompile]
    public class Teleporter : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Methods.LoadYourSceneAsync("Dungeon"));

                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    Generator generator = GameManager.GetGenerator();
                    
                    if (generator != null && scene.name == "Dungeon") generator.Generate(Random.Range(10, 21));
                };
            }
        }
    }
}
