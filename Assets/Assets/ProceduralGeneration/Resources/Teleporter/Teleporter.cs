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
        [SerializeField] private int numberOfRooms;

        private void OnTriggerEnter2D(Collider2D other)
        {
            numberOfRooms = Random.Range(15, 30);
            Generator generator = GameManager.GetGenerator();
            
            
            if (other.CompareTag("Player"))
            {
                if (SceneManager.GetActiveScene().name != "Dungeon")
                {
                    StartCoroutine(Methods.LoadYourSceneAsync("Dungeon"));
                    SceneManager.sceneLoaded += (scene, mode) =>
                    {
                        if (scene.name == "Dungeon")
                        {
                            generator.Generate(numberOfRooms);
                        }
                    };
                }
                else
                {
                    generator.Generate(numberOfRooms);
                }
            }
        }
    }
}
