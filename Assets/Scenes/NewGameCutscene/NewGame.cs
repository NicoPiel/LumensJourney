using Core;
using UnityEngine;
using UnityEngine.Playables;

namespace Scenes.NewGameCutscene
{
    public class NewGame : MonoBehaviour
    {
        private PlayableDirector _timeline;

        private void Start()
        {
            _timeline = GetComponent<PlayableDirector>();

            _timeline.stopped += director => StartNewGame();
        }

        private void Update()
        {
            // Skip cutscene
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _timeline.Stop();
            }
        }

        private void StartNewGame()
        {
            GameManager.NewGame_Static();
        }
    }
}
