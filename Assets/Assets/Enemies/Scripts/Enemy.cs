using System;
using Assets.Player.Script;
using Cinemachine;
using Core;
using UnityEngine;

namespace Assets.Enemies.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [Header("AI settings")] 
        public float detectionRadius;
        public float innerRadius;
        public float speed;
        [Space] [Header("Combat settings")] public int health;
        public int damage;
        [Space]
        [Header("Other settings")]
        [SerializeField] private float screenShakeMagnitude;

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
            
            _innerRadius.onPlayerWalkIntoRange.AddListener(OnPlayerWalkIntoRange);

            _detectionRadius.GetComponent<CircleCollider2D>().radius = detectionRadius;
            _innerRadius.GetComponent<CircleCollider2D>().radius = innerRadius;
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void MoveToPlayer()
        {
            Vector3 enemyPosition = transform.position;
            Vector3 playerPosition = GameManager.GetPlayer().transform.position;
            Vector2 vectorToPlayer = playerPosition - enemyPosition;
            Vector2 playerDirection = (playerPosition - enemyPosition).normalized;
            var distanceToPlayer = vectorToPlayer.magnitude;

            RaycastHit2D raycastHitInfo = Physics2D.Raycast(enemyPosition, vectorToPlayer, distanceToPlayer, LayerMask.GetMask("PlayerObject"));
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

        private void OnPlayerWalkIntoRange()
        {
            Debug.Log($"Player walked into {gameObject.name}");

            PlayerScript playerScipt = GameManager.GetPlayerScript();
            Rigidbody2D playerRigidbody = playerScipt.GetRigidbody();
                
            playerScipt.PlayerTakeDamage(damage);
                
            Vector3 moveDirection = playerRigidbody.transform.position - this.transform.position;
            playerRigidbody.AddForce( moveDirection.normalized * -500f, ForceMode2D.Force);
                
            GetComponent<CinemachineImpulseSource>().GenerateImpulse(new Vector2(screenShakeMagnitude,screenShakeMagnitude));
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.Equals(GameManager.GetPlayerScript().hitCollider))
            {
                PlayerScript playerScipt = GameManager.GetPlayerScript();

                TakeDamage(playerScipt.GetPlayerDamage());
                
                Vector3 moveDirection =  _rigidbody.transform.position - playerScipt.transform.position;
                _rigidbody.AddForce( moveDirection.normalized * -500f);
                
                Debug.Log($"{gameObject.name} was hit by the player.");
            }
        }

        public void TakeDamage(int damageTaken)
        {
            health -= damageTaken;
            if (health < 0) health = 0;

            Die();
        }

        private void Die()
        {
            if (health > 0) return;
            
            Debug.Log($"{gameObject.name} died.");
            Destroy(gameObject);
        }

        private void StopMoving()
        {
            _rigidbody.velocity = new Vector2(0, 0);
        }
    }
}