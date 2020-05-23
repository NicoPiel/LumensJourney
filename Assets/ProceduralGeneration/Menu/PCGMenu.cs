using System.Diagnostics;
using ProceduralGeneration.Core;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ProceduralGeneration.Menu
{
    [CustomEditor(typeof(Generator))]
    public class PcgMenu : Editor
    {
        // OnInspector GUI
        public override void OnInspectorGUI()
        {
            var stopwatch = new Stopwatch();
            DrawDefaultInspector();
            
            if (GUILayout.Button("Generate") && Application.isPlaying)
            {
                var generator = FindObjectOfType<Generator>();
                
                stopwatch.Restart();
                if (generator.Generate())
                {
                    stopwatch.Stop();
                    Debug.Log($"Generated dungeon in {stopwatch.ElapsedMilliseconds}ms.");
                }
                stopwatch.Stop();
            }
            
            if (GUILayout.Button("Decorate") && Application.isPlaying)
            {
                var decorator = FindObjectOfType<Decorator>();
                
                stopwatch.Restart();
                if (decorator.Decorate())
                {
                    stopwatch.Stop();
                    Debug.Log($"Decorated dungeon in {stopwatch.ElapsedMilliseconds}ms.");
                }
                stopwatch.Stop();
            }
        }
    }
}