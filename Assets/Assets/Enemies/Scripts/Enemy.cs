using System;
using Core;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Enemies.Scripts
{
    public class Enemy : MonoBehaviour
    {
        
        public float detectionRadius;

        private NavMeshAgent _navMeshAgent;
        private DetectionRadius _detectionRadius;
        
        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();

            _navMeshAgent.updatePosition = false;
            _navMeshAgent.updateUpAxis = false;

            _detectionRadius = transform.Find("DetectionRadius").gameObject.GetComponent<DetectionRadius>();
            _detectionRadius.onDetected.AddListener(MoveToPlayer);
            _detectionRadius.GetComponent<CircleCollider2D>().radius = detectionRadius;
        }

        private void MoveToPlayer()
        {
            if (_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.SetDestination(GameManager.GetPlayer().transform.position);
                Debug.Log("Moving..");
            }
            else Debug.Log("No nav mesh..");
        }
    }
}
