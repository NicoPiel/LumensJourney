using System.Collections;
using Assets.Player.Script;
using Cinemachine;
using Core;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.Enemies.Scripts
{
    public class Enemy : MonoBehaviour
    {
        #region Events

        public UnityEvent onDeath;
        public UnityEvent onDamageTaken;

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

        [Space] [Header("Audio")] 
        [SerializeField] private AudioClip soundOnHit;
        [SerializeField] private AudioClip soundOnDeath;

        #endregion

        #region Private variables

        private DetectionRadius _detectionRadius;
        private InnerRadius _innerRadius;
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;
        private ParticleSystem _particleSystem;
        private CinemachineImpulseSource _impulseSource;
        private Animator _animator;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;

        private bool _invulnerable;
        private float _invulnerabilityTime;
        
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        #region UnityEvent functions

        private void Awake()
        {
            onDeath = new UnityEvent();
            onDamageTaken = new UnityEvent();
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

            _detectionRadius.GetComponent<CircleCollider2D>().radius = detectionRadius;
            _innerRadius.GetComponent<CircleCollider2D>().radius = innerRadius;
            _rigidbody = GetComponent<Rigidbody2D>();
            _particleSystem = GetComponent<ParticleSystem>();
            _impulseSource = GetComponent<CinemachineImpulseSource>();
            _audioSource = GetComponent<AudioSource>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            lightShardPrefab = Resources.Load<GameObject>("LightShard");
            _invulnerabilityTime = GameManager.GetPlayerScript().GetTimeBetweenAttacks() - .5f;
        }

        #endregion

        #region EventSubscriptions

        // On DetectionRadius.onDetected
        private void MoveToPlayer()
        {
            Vector2 enemyPosition = transform.position;
            Vector2 playerPosition = GameManager.GetPlayer().transform.position + (Vector3) GameManager.GetPlayerScript().GetCollider().offset;
            Vector2 vectorToPlayer = playerPosition - enemyPosition;
            Vector2 playerDirection = (playerPosition - enemyPosition).normalized;
            var distanceToPlayer = vectorToPlayer.magnitude;

            RaycastHit2D raycastHitInfo = Physics2D.Raycast(enemyPosition, vectorToPlayer, distanceToPlayer);
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
                        _rigidbody.MovePosition(enemyPosition + (playerDirection * speed * Time.fixedDeltaTime));
                    }
                }
            }
            else
            {
                _rigidbody.MovePosition(transform.position);
            }
        }
        
        // On DetectionRadius.onLostSight
        private void StopMoving()
        {
            _rigidbody.MovePosition(transform.position);
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

        #endregion

        public void TakeDamage (int damageTaken)
        {
            if (_invulnerable) return;
            
            health -= damageTaken;
            
            if (health <= 0)
            {
                health = 0;
                Die();
            }
            else
            {
                _audioSource.PlayOneShot(soundOnHit);
                _particleSystem.Play();
                _impulseSource.GenerateImpulse(new Vector2(screenShakeMagnitude/2,screenShakeMagnitude/2));
                StartCoroutine(Invulnerable());
            }
        }
        
        // On this.onDeath
        private void Die()
        {
            Debug.Log($"{gameObject.name} died.");
            GameManager.GetPlayerScript().PlayerChangeLightLevel(health*10);
            GameManager.GetPlayerScript().PlayerChangeLightShards(health*10);
            
            _particleSystem.Play();
            DropShards();
            _audioSource.PlayOneShot(soundOnDeath);
            onDeath.Invoke();
            Destroy(gameObject, 0.2f);
        }

        private void DropShards ()
        {
            Drop(lightShardPrefab);

            var droppedShards = Random.Range(0, maxShardsOnDrop);
            
            for (var i = 0; i < droppedShards; i++)
            {
                Drop(lightShardPrefab);
            }
        }

        private GameObject Drop (GameObject prefab)
        {
            Vector2 dropPosition = gameObject.transform.position;
            
            GameObject drop = Instantiate(prefab, 
                dropPosition, 
                Quaternion.identity);
            drop.GetComponent<Rigidbody2D>().AddForce(GetRandomDirection(), ForceMode2D.Impulse);

            return drop;
        }

        private Vector2 GetRandomDirection ()
        {
            return new Vector2(math.sin(Random.Range(0f, 2*math.PI)), math.sin(Random.Range(0f, 2*math.PI))).normalized;
        }

        private IEnumerator Invulnerable ()
        {
            _invulnerable = true;
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(_invulnerabilityTime);
            _invulnerable = false;
            yield return new WaitForSeconds(_invulnerabilityTime*4);
            _spriteRenderer.color = Color.white;
        }

        public Rigidbody2D GetRigidbody ()
        {
            return _rigidbody;
        }
    }
}