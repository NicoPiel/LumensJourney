using System;
using ProceduralGeneration.Core;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }

        public static void NewGame_Static()
        {
            _instance.NewGame();
        }

        private void NewGame()
        {
            StartCoroutine(Utility.Methods.LoadYourAsyncScene("PCGTestScene"));
            Generator.Generate_Static();
        }
        
        
    }
}
