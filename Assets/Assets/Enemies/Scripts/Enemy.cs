using System;
using System.Collections.Generic;
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
            Vector2 playerDirection = (playerPosition - enemyPosition);

            var lineOfSight = new Ray(enemyPosition, playerPosition);
            var raycastHitInfo = new RaycastHit2D[1];

            _collider.Raycast(playerDirection.normalized, raycastHitInfo, playerDirection.magnitude);

            if (!_innerRadius.IsInInner() && _detectionRadius.IsDetected())
            {
                Debug.Log("Detected player..");
                
                if (raycastHitInfo[0].collider.gameObject.CompareTag("Player"))
                {
                    _rigidbody.velocity = playerDirection.normalized * speed * Time.deltaTime;
                    Debug.Log("Can see player..");
                    Debug.Log("Moving to player..");
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