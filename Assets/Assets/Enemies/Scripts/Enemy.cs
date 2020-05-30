using System;
using Core;
using UnityEngine;

namespace Assets.Enemies.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [Header("AI settings")] public float detectionRadius;
        public float speed;
        [Space] [Header("Combat settings")] public int damage;

        private DetectionRadius _detectionRadius;
        private InnerRadius _innerRadius;
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;

        private void Start()
        {
            _collider = GetComponent<BoxCollider2D>();

            _detectionRadius = GetComponentInChildren<DetectionRadius>();
            _innerRadius = GetComponentInChildren<InnerRadius>();

            _detectionRadius.onDetected.AddListener(MoveToPlayer);
            _detectionRadius.onLostSight.AddListener(StopMoving);

            _detectionRadius.GetComponent<CircleCollider2D>().radius = detectionRadius;
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void MoveToPlayer()
        {
            Vector3 enemyPosition = transform.position;
            Vector3 playerPosition = GameManager.GetPlayer().transform.position;
            Vector2 vectorToPlayer = playerPosition - enemyPosition;
            Vector2 playerDirection = (playerPosition - enemyPosition).normalized;
            var distanceToPlayer = vectorToPlayer.magnitude;

            var filter = new ContactFilter2D()
            {
                layerMask = LayerMask.GetMask("Player"),
                useLayerMask = true,
                useTriggers = false,
                
            };
            
            RaycastHit2D raycastHitInfo = Physics2D.Raycast(enemyPosition, vectorToPlayer, distanceToPlayer, LayerMask.GetMask("Player"));
            Debug.DrawRay(enemyPosition, vectorToPlayer);

            if (!_innerRadius.IsInInner() && _detectionRadius.IsDetected())
            {
                Debug.Log("Detected player..");
                
                if (raycastHitInfo)
                {
                    Debug.Log("Can see something..");
                    Debug.Log(raycastHitInfo.transform.gameObject.tag);
                    
                    if (raycastHitInfo.transform.gameObject.CompareTag("Player"))
                    {
                        Debug.Log("Can see Player..");
                        _rigidbody.velocity = playerDirection * speed * Time.deltaTime;
                        Debug.Log("Moving to player..");
                    }
                }
            }
            else
            {
                _rigidbody.velocity = new Vector2(0, 0);
            }
        }

        private void StopMoving()
        {
            _rigidbody.velocity = new Vector2(0, 0);
        }
    }
}