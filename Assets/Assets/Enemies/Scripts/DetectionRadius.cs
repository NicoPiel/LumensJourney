using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Enemies.Scripts
{
    public class DetectionRadius : MonoBehaviour
    {
        public UnityEvent onDetected;

        public bool detected;

        private void Awake()
        {
            onDetected = new UnityEvent();
        }

        private void Start()
        {
            onDetected.AddListener(OnDetected);
        }

        private void OnDetected()
        {
            detected = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) onDetected.Invoke();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player")) onDetected.Invoke();
        }
    }
}
