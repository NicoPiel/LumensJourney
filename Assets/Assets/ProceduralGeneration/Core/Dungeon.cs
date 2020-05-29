using UnityEngine;
using UnityEngine.AI;

namespace Assets.ProceduralGeneration.Core
{
    public class Dungeon : MonoBehaviour
    {
        private NavMeshSurface2d _navMeshSurface;
        
        // Start is called before the first frame update
        private void Start()
        {
            _navMeshSurface = GetComponent<NavMeshSurface2d>();
        }

        public void BakeNavMesh()
        {
            _navMeshSurface.BuildNavMesh();
        }
    }
}
