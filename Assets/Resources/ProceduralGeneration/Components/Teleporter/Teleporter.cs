using Core;
using Unity.Burst;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Resources.ProceduralGeneration.Components.Teleporter
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
                    if (GameManager.Generator != null && scene.name == "Dungeon") GameManager.Generator.Generate(Random.Range(10, 21));
                };
            }
        }
    }
}
