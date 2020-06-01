using System.Diagnostics;
using Assets.ProceduralGeneration.Core;
using Unity.Burst;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.ProceduralGeneration.Menu
{
    [BurstCompile]
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
                var generator = GameObject.FindWithTag("Generator").GetComponent<Generator>();
                
                stopwatch.Restart();
                if (generator.Generate(generator.RoomNumber))
                {
                    stopwatch.Stop();
                    Debug.Log($"Generated dungeon in {stopwatch.ElapsedMilliseconds}ms.");
                }
                stopwatch.Stop();
            }
        }
    }
}