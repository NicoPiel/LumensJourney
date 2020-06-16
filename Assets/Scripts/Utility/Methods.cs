using System.Collections;
using Unity.Burst;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    [BurstCompile]
    public class Methods : MonoBehaviour
    {
        public static IEnumerator LoadYourSceneAsync(string sceneName)
        {
            Debug.Log($"Loading scene {sceneName}");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        
        public static IEnumerator LoadYourSceneAsync(int sceneNumber)
        {
            Debug.Log($"Loading scene {sceneNumber}");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(1);
        }
    }
}
