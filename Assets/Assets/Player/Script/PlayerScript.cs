using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Healthbar.Scripts;
using Assets.Items.Scripts;
using Assets.PlayerUI.Scripts;
using Core;
using Resources.LightBar.Scripts;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.Player.Script
{
    [BurstCompile]
    public class PlayerScript : MonoBehaviour
    {
        public int lightLoss;
        public float speed;
        private CapsuleCollider2D _playerCollider;
        private Rigidbody2D _playerRigidbody2D;
        private Vector3 _change;
        private AudioSource _audioSource;
        private Animator _animator;
        private Player _player = new Player("Pacolos");
        public HealthbarScript healthBarScript;
        public LightBarScript lightBar;
        public LightShardDisplayScript lightShardScript;
        public BoxCollider2D hitCollider;
        private static readonly int StateExit = Animator.StringToHash("StateExit");
        private static readonly int IdleDown = Animator.StringToHash("IdleDown");

        #region UnityEvents

        public UnityEvent onPlayerTakeDamage;
        public UnityEvent onItemAddedToPlayerInventory;
        public UnityEvent onPlayerLightLevelChanged;
        public UnityEvent onPlayerLightShardsChanged;

        #endregion

        public Dictionary<string, AudioClip> audioClips;

        [SerializeField] private int playerDamage;


        // Start is called before the first frame update
        private void Awake()
        {
            SetUpEvents();
        }

        private void Start()
        {
            healthBarScript = PlayerUiScript.GetPlayerUiScript().GetHealthBarScript();
            lightBar = PlayerUiScript.GetPlayerUiScript().GetLightBarScript();
            lightShardScript = PlayerUiScript.GetPlayerUiScript().GetLightShardDisplayScript();
            _playerCollider = GetComponent<CapsuleCollider2D>();
            _playerRigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            audioClips = new Dictionary<string, AudioClip>();
            healthBarScript.ChangeHealthBar(5, 5);
            hitCollider = transform.Find("HitCollider").GetComponent<BoxCollider2D>();
            hitCollider.gameObject.SetActive(false);
            _animator.SetBool(StateExit, false);

            _audioSource = GetComponent<AudioSource>();

            AddAudioClips();

            GameManager.GetGenerator().onDungeonGenerated.AddListener(() => { StartCoroutine(LoseLightPerSecond(lightLoss)); });
        }


        private void SetUpEvents()
        {
            onPlayerTakeDamage = new UnityEvent();
            onItemAddedToPlayerInventory = new UnityEvent();
            onPlayerLightLevelChanged = new UnityEvent();
            onPlayerLightShardsChanged = new UnityEvent();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                PlayerChangeLightShards(50);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                PlayerChangeLightShards(-50);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                HitInDirection(180, new Vector2(2, 1), new Vector2(0, -1f), "SwingUp");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                HitInDirection(90, new Vector2(1, 1), new Vector2(0.5f, -1f), "SwingRight");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                HitInDirection(0, new Vector2(2, 1), new Vector2(0, -0.5f), "SwingDown");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                HitInDirection(270, new Vector2(1, 1), new Vector2(-0.5f, -1f), "SwingLeft");
            }
            else
            {
                _change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
                _change.Normalize();
            }
        }

        private void HitInDirection(float rotation, Vector2 size, Vector2 offset, string stateName)
        {
            hitCollider.transform.eulerAngles = new Vector3(0, 0, rotation);
            hitCollider.size = size;
            hitCollider.offset = offset;
            hitCollider.gameObject.SetActive(true);

            _animator.SetBool(StateExit, true);
            _animator.Play(stateName);
        }

        private void SetStateExit()
        {
            if (!_animator.GetBool(StateExit)) return;
            
            hitCollider.gameObject.SetActive(false);
            var layerIndex = _animator.GetLayerIndex("Player");
            _animator.SetBool(StateExit, false);
        }

        public void PlayFootsteps()
        {
            var rnd = Random.Range(0, 3);
            _audioSource.clip = audioClips["footstep0" + rnd];
            _audioSource.Play();
        }

        public void PlaySwordWoosh()
        {
            var rnd = Random.Range(1, 8);
            _audioSource.clip = audioClips["woosh" + rnd];
            _audioSource.Play();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                
            }
        }

        public void AddToInventory(GameItem item)
        {
            _player.Inventory.AddItem(item);
        }

        private void FixedUpdate()
        {
            MoveCharacter();
        }

        private void MoveCharacter()
        {
            _animator.SetFloat("Horizontal", _change.x);
            _animator.SetFloat("Vertical", _change.y);
            _animator.SetFloat("Speed", _change.magnitude);
            _playerRigidbody2D.MovePosition(transform.position + (_change * speed * Time.fixedDeltaTime));
        }

        private void AddAudioClips()
        {
            for (int i = 1; i <= 8; i++)
            {
                audioClips.Add("woosh" + i, UnityEngine.Resources.Load<AudioClip>("Wooshes/woosh" + i));
            }

            for (int i = 0; i <= 2; i++)
            {
                audioClips.Add("footstep0" + i, UnityEngine.Resources.Load<AudioClip>("Footsteps/footstep0" + i));
            }
        }

        public void PlayerChangeLightLevel(int lightlevel)
        {
            _player.playerstats["CurrentLightLevel"] += lightlevel;
            if (_player.playerstats["CurrentLightLevel"] < 0) _player.playerstats["CurrentLightLevel"] = 0;
            lightBar.ChangeProgress(_player.playerstats["CurrentLightLevel"], _player.playerstats["MaxLightLevel"]);
            onPlayerLightLevelChanged.Invoke();
        }

        public int GetPlayerDamage()
        {
            return playerDamage;
        }

        public void PlayerTakeDamage(int damage)
        {
            var remainingHealth = _player.playerstats["CurrentHealth"] -= damage;

            if (remainingHealth >= 0)
            {
                _player.playerstats["CurrentHealth"] = remainingHealth;
                healthBarScript.ChangeHealthBar(_player.playerstats["CurrentHealth"], _player.playerstats["MaxHealth"]);
                PlayerChangeLightLevel(-damage*10);
                onPlayerTakeDamage.Invoke();
            }
            else
            {
                KillPlayer();
            }
        }

        private void KillPlayer()
        {
            Debug.Log("The player died.");
        }

        public Collider2D GetCollider()
        {
            return _playerCollider;
        }

        public Rigidbody2D GetRigidbody()
        {
            return _playerRigidbody2D;
        }

        public float GetPlayerLightLevel()
        {
            return (float) _player.playerstats["CurrentLightLevel"] / (float) _player.playerstats["MaxLightLevel"];
        }

        public int GetLightShardAmount()
        {
            return _player.Inventory.Lightshard;
        }

        public void PlayerChangeLightShards(int lightShards)
        {
            _player.Inventory.Lightshard += lightShards;
            PlayerUiScript.GetPlayerUiScript().GetLightShardDisplayScript().UpdateLightShardDisplay(_player.Inventory.Lightshard);
            onPlayerLightLevelChanged.Invoke();
        }

        public void PlayerSetLightShards(int lightShards)
        {
            _player.Inventory.Lightshard = lightShards;
            PlayerUiScript.GetPlayerUiScript().GetLightShardDisplayScript().UpdateLightShardDisplay(_player.Inventory.Lightshard);
        }

        private IEnumerator LoseLightPerSecond(int lightLoss)
        {
            int maxLightLevelLoss = 300;
            while (maxLightLevelLoss > 0)
            {
                maxLightLevelLoss -= lightLoss;
                PlayerChangeLightLevel(-lightLoss);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}