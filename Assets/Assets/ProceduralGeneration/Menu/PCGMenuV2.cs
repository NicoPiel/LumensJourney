using System.Diagnostics;
using Assets.ProceduralGeneration.Core;
using Unity.Burst;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.ProceduralGeneration.Menu
{
    [BurstCompile]
    [CustomEditor(typeof(GeneratorV2))]
    public class PcgMenuV2 : Editor
    {
        // OnInspector GUI
        public override void OnInspectorGUI()
        {
            var stopwatch = new Stopwatch();
            DrawDefaultInspector();
            
            if (GUILayout.Button("Generate") && Application.isPlaying)
            {
                var generator = GameObject.FindWithTag("Generator").GetComponent<GeneratorV2>();
                
                stopwatch.Restart();
                if (generator.Generate(generator.roomNumber))
                {
                    stopwatch.Stop();
                    Debug.Log($"Generated dungeon in {stopwatch.ElapsedMilliseconds}ms.");
                }
                stopwatch.Stop();
            }
        }
    }
}
