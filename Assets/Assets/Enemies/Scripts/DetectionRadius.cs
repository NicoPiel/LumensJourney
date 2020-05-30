using UnityEngine;
using UnityEngine.Events;

namespace Assets.Enemies.Scripts
{
    public class DetectionRadius : MonoBehaviour
    {
        public UnityEvent onDetected;
        public UnityEvent onLostSight;

        private bool _detected;

        private void Awake()
        {
            onDetected = new UnityEvent();
            onLostSight = new UnityEvent();
        }

        private void Start()
        {
            onDetected.AddListener(OnDetected);
            onLostSight.AddListener(OnLostSight);
        }

        private void OnDetected()
        {
            _detected = true;
        }

        private void OnLostSight()
        {
            _detected = false;
        }

            private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) onDetected.Invoke();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player")) onDetected.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) onLostSight.Invoke();
        }

        public bool IsDetected()
        {
            return _detected;
        }
    }
}
