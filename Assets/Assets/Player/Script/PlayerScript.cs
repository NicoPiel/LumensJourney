using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Items.Scripts;
using Core;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.Player.Script
{
    [BurstCompile]
    public class PlayerScript : MonoBehaviour
    {
        #region Inspector variables

        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private float invunerabilityTime;
        [SerializeField] private int playerDamage;
        [SerializeField] private GameObject lightSphereGameObject;
        [SerializeField] private float lightSphereSpeed;
        [SerializeField] private float lightSphereCooldown;

        public int lightLoss;
        public float speed;
        public BoxCollider2D hitCollider;

        #endregion

        #region Public variables

        #endregion

        #region Private variables

        private CapsuleCollider2D _playerCollider;
        private Rigidbody2D _playerRigidbody2D;
        private Vector3 _change;
        private AudioSource _audioSource;
        private Animator _animator;
        private Player _player = new Player("Pacolos");

        private Dictionary<string, AudioClip> _audioClips;
        private bool _invulnerable;
        private bool _canAttack;
        private bool _canSendLightSphere;
        private bool _frozen;

        private static readonly int StateExit = Animator.StringToHash("StateExit");
        private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
        private static readonly int LastVertical = Animator.StringToHash("LastVertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        #region UnityEvents

        public UnityEvent onPlayerTakeDamage;
        public UnityEvent onPlayerTakeHeal;
        public UnityEvent onItemAddedToPlayerInventory;
        public UnityEvent onPlayerLightLevelChanged;
        public UnityEvent onPlayerLightShardsChanged;
        public UnityEvent onPlayerLifeChanged;
        public UnityEvent onPlayerDied;

        #endregion

        #region UnityEvent functions

        // Start is called before the first frame update
        private void Awake()
        {
            SetUpEvents();
        }

        private void Start()
        {
            _playerRigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _audioClips = new Dictionary<string, AudioClip>();
            _playerCollider = GetComponent<CapsuleCollider2D>();
            hitCollider = transform.Find("HitCollider").GetComponent<BoxCollider2D>();
            _audioSource = GetComponent<AudioSource>();

            AddAudioClips();
            GameManager.GetGenerator().onDungeonGenerated.AddListener(() => { StartCoroutine(LoseLightPerSecond()); });

            _canAttack = true;
            hitCollider.gameObject.SetActive(false);
            _canSendLightSphere = true;
        }

        #endregion

        #region Update

        // Update is called once per frame
        private void Update()
        {
            if (!_frozen)
            {
                #region Controls

                if (Input.GetKeyDown(KeyCode.J))
                {
                    PlayerChangeLightShards(50);
                }

                if (Input.GetKeyDown(KeyCode.K))
                {
                    PlayerChangeLightShards(-50);
                }

                if (Input.GetKeyDown(KeyCode.V))
                {
                    PlayerTakeDamage(1);
                }

                if (Input.GetKeyDown(KeyCode.B))
                {
                    PlayerTakeHeal(1);
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    GameObject teleporter = GameManager.GetGenerator().teleporter;
                    if (teleporter != null) SendLightSphereFromPlayer(teleporter.transform.position);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    HitInDirection(180, new Vector2(2, 1), new Vector2(0, -1f), "SwingUp");
                    _animator.SetFloat(LastHorizontal, 0);
                    _animator.SetFloat(LastVertical, 1);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    HitInDirection(90, new Vector2(1, 1), new Vector2(0.5f, -1f), "SwingRight");
                    _animator.SetFloat(LastHorizontal, 1);
                    _animator.SetFloat(LastVertical, 0);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    HitInDirection(0, new Vector2(2, 1), new Vector2(0, -0.5f), "SwingDown");
                    _animator.SetFloat(LastHorizontal, 0);
                    _animator.SetFloat(LastVertical, -1);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    HitInDirection(270, new Vector2(1, 1), new Vector2(-0.5f, -1f), "SwingLeft");
                    _animator.SetFloat(LastHorizontal, -1);
                    _animator.SetFloat(LastVertical, 0);
                }
                else
                {
                    _change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
                    _change.Normalize();
                    if (_change.x < 0 || _change.x > 0)
                    {
                        _animator.SetFloat(LastHorizontal, _change.x);
                        _animator.SetFloat(LastVertical, 0);
                    }

                    if (_change.y < 0 || _change.y > 0)
                    {
                        _animator.SetFloat(LastVertical, _change.y);
                        _animator.SetFloat(LastHorizontal, 0);
                    }
                }

                #endregion
            }
        }

        private void FixedUpdate()
        {
            MoveCharacter();
        }

        #endregion
        
        private void SetUpEvents()
        {
            onPlayerTakeDamage = new UnityEvent();
            onItemAddedToPlayerInventory = new UnityEvent();
            onPlayerLightLevelChanged = new UnityEvent();
            onPlayerLightShardsChanged = new UnityEvent();
            onPlayerTakeHeal = new UnityEvent();
            onPlayerLifeChanged = new UnityEvent();
            onPlayerDied = new UnityEvent();
        }

        private void HitInDirection(float rotation, Vector2 size, Vector2 offset, string stateName)
        {
            if (_canAttack)
            {
                hitCollider.transform.eulerAngles = new Vector3(0, 0, rotation);
                hitCollider.size = size;
                hitCollider.offset = offset;

                _animator.Play(stateName);
                StartCoroutine(CanAttack());
            }
        }

        private GameObject SendLightSphereFromPlayer(Vector2 targetPosition)
        {
            if (!_canSendLightSphere) return null;

            GameObject lightSphere = GameObject.FindWithTag("LightSphere");
            if (lightSphere != null) Destroy(lightSphere);

            Vector2 playerPosition = transform.position;
            Vector2 vectorToTarget = (targetPosition - playerPosition).normalized;

            lightSphere = Instantiate(lightSphereGameObject, playerPosition, Quaternion.identity, gameObject.transform);
            lightSphere.name = "LightSphere";
            lightSphere.GetComponent<Rigidbody2D>().velocity = vectorToTarget * lightSphereSpeed;

            StartCoroutine(LightSphereCooldown());

            return lightSphere;
        }

        private IEnumerator LightSphereCooldown()
        {
            _canSendLightSphere = false;
            yield return new WaitForSeconds(lightSphereCooldown);
            _canSendLightSphere = true;
        }

        private void SetStateEnter()
        {
            if (_animator.GetBool(StateExit)) return;

            Debug.Log("State enter.");

            hitCollider.gameObject.SetActive(true);
            _animator.SetBool(StateExit, true);
        }

        private void SetStateExit()
        {
            if (!_animator.GetBool(StateExit)) return;
            
            Debug.Log("State exit.");

            hitCollider.gameObject.SetActive(false);
            _animator.SetBool(StateExit, false);
        }

        private void MoveCharacter()
        {
            _animator.SetFloat(Horizontal, _change.x);
            _animator.SetFloat(Vertical, _change.y);
            _animator.SetFloat(Speed, _change.magnitude);
            _playerRigidbody2D.MovePosition(transform.position + (_change * speed * Time.fixedDeltaTime));
        }

        public void AddToInventory(GameItem item)
        {
            _player.Inventory.AddItem(item);
        }

        public Collider2D GetCollider()
        {
            return _playerCollider;
        }

        public Rigidbody2D GetRigidbody()
        {
            return _playerRigidbody2D;
        }

        #region PlayerDamage

        public void PlayerTakeDamage(int damage)
        {
            if (_invulnerable) return;

            var remainingHealth = _player.PlayerStats["CurrentHealth"] -= damage;

            if (remainingHealth > 0)
            {
                _player.PlayerStats["CurrentHealth"] = remainingHealth;
                PlayerChangeLightLevel(-damage * 10);
                onPlayerLifeChanged.Invoke();
                onPlayerTakeDamage.Invoke();
            }
            else
            {
                _player.PlayerStats["CurrentHealth"] = 0;
                KillPlayer();
            }

            StartCoroutine(Invulnerable());
        }

        private void KillPlayer()
        {
            onPlayerDied.Invoke();
            Debug.Log("The player died.");
        }

        public void PlayerTakeHeal(int heal)
        {
            _player.PlayerStats["CurrentHealth"] += heal;
            onPlayerLifeChanged.Invoke();
        }

        public int GetPlayerCurrentHealth()
        {
            return _player.PlayerStats["CurrentHealth"];
        }

        public int GetPlayerMaxHealth()
        {
            return _player.PlayerStats["MaxHealth"];
        }

        public int GetPlayerDamage()
        {
            return playerDamage;
        }

        #endregion

        #region Sounds

        public void PlayFootsteps()
        {
            int rnd = Random.Range(0, 3);
            _audioSource.clip = _audioClips["footstep0" + rnd];
            _audioSource.Play();
        }

        public void PlaySwordWoosh()
        {
            int rnd = Random.Range(1, 8);
            _audioSource.clip = _audioClips["woosh" + rnd];
            _audioSource.Play();
        }

        private void AddAudioClips()
        {
            for (int i = 1; i <= 8; i++)
            {
                _audioClips.Add("woosh" + i, Resources.Load<AudioClip>("Wooshes/woosh" + i));
            }

            for (int i = 0; i <= 2; i++)
            {
                _audioClips.Add("footstep0" + i, Resources.Load<AudioClip>("Footsteps/footstep0" + i));
            }
        }

        #endregion

        #region Getter/Setter and Timer for LightLevel

        //Setter
        public void PlayerChangeLightLevel(int lightlevel)
        {
            _player.PlayerStats["CurrentLightLevel"] += lightlevel;
            if (_player.PlayerStats["CurrentLightLevel"] < 0) _player.PlayerStats["CurrentLightLevel"] = 0;
            onPlayerLightLevelChanged.Invoke();
        }

        //Getter
        public float GetPlayerLightLevel()
        {
            return (float) _player.PlayerStats["CurrentLightLevel"] / (float) _player.PlayerStats["MaxLightLevel"];
        }

        public int GetPlayerCurrentLightValue()
        {
            return _player.PlayerStats["CurrentLightLevel"];
        }

        public int GetPlayerMaxLightValue()
        {
            return _player.PlayerStats["MaxLightLevel"];
        }

        //Timer
        private IEnumerator LoseLightPerSecond()
        {
            int maxLightLevelLoss = 300;
            while (maxLightLevelLoss > 0)
            {
                maxLightLevelLoss -= lightLoss;
                PlayerChangeLightLevel(-lightLoss);
                yield return new WaitForSeconds(1f);
            }
        }

        #endregion

        #region Getter/Setter for LightShards

        //Setter
        public void PlayerChangeLightShards(int lightShards)
        {
            _player.Inventory.Lightshard += lightShards;
            onPlayerLightShardsChanged.Invoke();
        }

        public void PlayerSetLightShards(int lightShards)
        {
            _player.Inventory.Lightshard = lightShards;
            onPlayerLightShardsChanged.Invoke();
        }

        //Getter
        public int GetLightShardAmount()
        {
            return _player.Inventory.Lightshard;
        }

        #endregion

        private IEnumerator CanAttack()
        {
            _canAttack = false;
            yield return new WaitForSeconds(timeBetweenAttacks);
            _canAttack = true;
        }

        private IEnumerator Invulnerable()
        {
            _invulnerable = true;
            yield return new WaitForSeconds(invunerabilityTime);
            _invulnerable = false;
        }

        private void StopPlayer()
        {
            _change = new Vector3(0,0,0);
        }

        public void FreezeControls()
        {
            StopPlayer();
            _frozen = true;
        }

        public void UnfreezeControls()
        {
            _frozen = false;
        }

        public float GetTimeBetweenAttacks()
        {
            return timeBetweenAttacks;
        }
    }
}