using Core;
using UnityEngine;

namespace Assets.Enemies.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [Header("AI settings")]
        public float detectionRadius;
        public float speed;
        [Space] 
        [Header("Combat settings")] 
        public int damage;

        private DetectionRadius _detectionRadius;
        private InnerRadius _innerRadius;
        private Rigidbody2D _rigidbody;

        private void Start()
        {
            _detectionRadius = GetComponentInChildren<DetectionRadius>();
            _innerRadius = GetComponentInChildren<InnerRadius>();
            
            _detectionRadius.onDetected.AddListener(MoveToPlayer);
            _detectionRadius.onLostSight.AddListener(StopMoving);
            
            _detectionRadius.GetComponent<CircleCollider2D>().radius = detectionRadius;
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void MoveToPlayer()
        {
            if (!_innerRadius.IsInInner() && _detectionRadius.IsDetected())
            {
                _rigidbody.velocity = (GameManager.GetPlayer().transform.position - transform.position).normalized * speed * Time.deltaTime;
                Debug.Log("Moving to player..");
            }
            else
            {
                _rigidbody.velocity = new Vector2(0,0);
            }
        }

        private void StopMoving()
        {
            _rigidbody.velocity = new Vector2(0,0);
        }
    }
}