using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    public class Methods : MonoBehaviour
    {
        public static IEnumerator LoadYourAsyncScene(string sceneName)
        {
            Debug.Log($"Loading scene {sceneName}");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        
        public static IEnumerator LoadYourAsyncScene(int sceneNumber)
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
