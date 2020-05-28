using System;
using UnityEngine;

namespace Assets.Enemies.Scripts
{
    public class Enemy : MonoBehaviour
    {
        public float detectionRadius;
        private DetectionRadius _detectionRadius;
        
        private void Start()
        {
            _detectionRadius = transform.Find("DetectionRadius").gameObject.GetComponent<DetectionRadius>();
            //_detectionRadius.onDetected.AddListener();
            _detectionRadius.GetComponent<CircleCollider2D>().radius = detectionRadius;
        }
        
        
    }
}
