using System.Collections;
using Assets.Player.Script;
using Cinemachine;
using Core;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Enemies.Scripts
{
    public class Enemy : MonoBehaviour
    {
        #region Events

        public UnityEvent onDeath;

        #endregion

        #region Inspector variables

        [Header("AI settings")] 
        [SerializeField] private float detectionRadius;
        [SerializeField] private float innerRadius;
        [SerializeField] private float speed;
        [SerializeField] private int maxShardsOnDrop;
        [Space] [Header("Combat settings")] public int health;
        public int damage;
        [Space]
        [Header("Other settings")]
        [SerializeField] private float screenShakeMagnitude;

        [SerializeField] private GameObject lightShardPrefab;

        #endregion

        #region Private variables

        private DetectionRadius _detectionRadius;
        private InnerRadius _innerRadius;
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;
        private ParticleSystem _particleSystem;
        private CinemachineImpulseSource _impulseSource;
        private Animator _animator;

        private bool _invulnerable;
        
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        #region UnityEvent functions

        private void Awake()
        {
            onDeath = new UnityEvent();
        }

        private void Start()
        {
            _collider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();

            _detectionRadius = GetComponentInChildren<DetectionRadius>();
            _innerRadius = GetComponentInChildren<InnerRadius>();

            _detectionRadius.onDetected.AddListener(MoveToPlayer);
            _detectionRadius.onLostSight.AddListener(StopMoving);
            
            _innerRadius.onPlayerWalkIntoRange.AddListener(OnPlayerWalkIntoRange);
            _innerRadius.onHit.AddListener(OnHit);

            _detectionRadius.GetComponent<CircleCollider2D>().radius = detectionRadius;
            _innerRadius.GetComponent<CircleCollider2D>().radius = innerRadius;
            _rigidbody = GetComponent<Rigidbody2D>();
            _particleSystem = GetComponent<ParticleSystem>();
            _impulseSource = GetComponent<CinemachineImpulseSource>();

            lightShardPrefab = Resources.Load<GameObject>("LightShard");

            onDeath.AddListener(OnDeath);
        }

        #endregion

        #region EventSubscriptions

        // On DetectionRadius.onDetected
        private void MoveToPlayer()
        {
            Vector3 enemyPosition = transform.position;
            Vector3 playerPosition = GameManager.GetPlayer().transform.position + (Vector3) GameManager.GetPlayerScript().GetCollider().offset;
            Vector2 vectorToPlayer = playerPosition - enemyPosition;
            Vector2 playerDirection = (playerPosition - enemyPosition).normalized;
            var distanceToPlayer = vectorToPlayer.magnitude;

            RaycastHit2D raycastHitInfo = Physics2D.Raycast(enemyPosition, vectorToPlayer, distanceToPlayer, LayerMask.GetMask("Player"));
            Debug.DrawRay(enemyPosition, vectorToPlayer);

            if (!_innerRadius.IsInInner() && _detectionRadius.IsDetected())
            {
                if (raycastHitInfo)
                {
                    Debug.Log(raycastHitInfo.transform.gameObject.tag);
                    if (raycastHitInfo.transform.gameObject.CompareTag("Player"))
                    {
                        _animator.SetFloat(Vertical, playerDirection.y);
                        _animator.SetFloat(Horizontal, playerDirection.x);
                        _animator.SetFloat(Speed, playerDirection.magnitude);
                        _rigidbody.velocity = playerDirection * speed * Time.fixedDeltaTime;
                    }
                }
            }
            else
            {
                _rigidbody.velocity = new Vector2(0, 0);
            }
        }
        
        // On DetectionRadius.onLostSight
        private void StopMoving()
        {
            _rigidbody.velocity = new Vector2(0, 0);
        }

        // On InnerRadius.onPlayerWalkIntoRange
        private void OnPlayerWalkIntoRange()
        {
            Debug.Log($"Player walked into {gameObject.name}");

            PlayerScript playerScipt = GameManager.GetPlayerScript();
            Rigidbody2D playerRigidbody = playerScipt.GetRigidbody();
                
            playerScipt.PlayerTakeDamage(damage);
                
            Vector3 moveDirection = playerRigidbody.transform.position - this.transform.position;
            playerRigidbody.AddForce( moveDirection.normalized * 500f, ForceMode2D.Force);
                
            _impulseSource.GenerateImpulse(new Vector2(screenShakeMagnitude,screenShakeMagnitude));
        }

        // On InnerRadius.onHit
        private void OnHit()
        {
            _particleSystem.Play();
            _impulseSource.GenerateImpulse(new Vector2(screenShakeMagnitude/2,screenShakeMagnitude/2));
        }
        
        // On this.onDeath
        private void OnDeath()
        {
            _particleSystem.Play();
            DropShards();
            Destroy(gameObject);
        }

        #endregion

        public void TakeDamage(int damageTaken)
        {
            if (_invulnerable) return;
            
            health -= damageTaken;
            if (health < 0) health = 0;

            Die();
        }

        private void Die()
        {
            if (health > 0) return;
            
            Debug.Log($"{gameObject.name} died.");
            GameManager.GetPlayerScript().PlayerChangeLightLevel(health*10);
            GameManager.GetPlayerScript().PlayerChangeLightShards(health*10);
            onDeath.Invoke();
        }

        private void DropShards()
        {
            Drop(lightShardPrefab);
            
            for (var i = 1; i < maxShardsOnDrop; i++)
            {
                Drop(lightShardPrefab);
            }
        }

        private GameObject Drop(GameObject prefab)
        {
            Transform dropPosition = transform;
            
            GameObject drop = Instantiate(prefab, dropPosition.position, Quaternion.identity, dropPosition);
            drop.GetComponent<Rigidbody2D>().AddForce(GetRandomDirection(), ForceMode2D.Impulse);

            return drop;
        }

        private Vector2 GetRandomDirection()
        {
            return new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }

        private IEnumerator Invulnerable()
        {
            _invulnerable = true;
            yield return new WaitForSeconds(0.1f);
            _invulnerable = false;
        }

        public Rigidbody2D GetRigidBody()
        {
            return _rigidbody;
        }
    }
}