using System;
using System.Collections;
using System.Collections.Generic;
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
        private Rigidbody2D _playerRigidbody2D;
        private Vector3 _change;
        private AudioSource _audioSource;
        private Animator _animator;
        private Player _player = new Player("Pacolos");
        public BoxCollider2D hitCollider;
        private static readonly int StateExit = Animator.StringToHash("StateExit");

        #region UnityEvents

        public UnityEvent onPlayerTakeDamage;
        public UnityEvent onPlayerTakeHeal;
        public UnityEvent onItemAddedToPlayerInventory;
        public UnityEvent onPlayerLightLevelChanged;
        public UnityEvent onPlayerLightShardsChanged;
        public UnityEvent onPlayerLifeChanged;

        #endregion

        public Dictionary<string, AudioClip> _audioClips;


        // Start is called before the first frame update
        private void Awake()
        {
            SetUpEvents();
        }

        private void Start()
        {
            _playerRigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _audioClips = new Dictionary<String, AudioClip>();
            hitCollider = transform.Find("HitCollider").GetComponent<BoxCollider2D>();
            hitCollider.gameObject.SetActive(false);
            _animator.SetBool(StateExit, false);
            _audioSource = GetComponent<AudioSource>();

            AddAudioClips();
            GameManager.GetGenerator().onDungeonGenerated.AddListener(() => { StartCoroutine(LoseLightPerSecond()); });
        }


        private void SetUpEvents()
        {
            onPlayerTakeDamage = new UnityEvent();
            onItemAddedToPlayerInventory = new UnityEvent();
            onPlayerLightLevelChanged = new UnityEvent();
            onPlayerLightShardsChanged = new UnityEvent();
            onPlayerTakeHeal = new UnityEvent();
            onPlayerLifeChanged = new UnityEvent();
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

            if (Input.GetKeyDown(KeyCode.I))
            {
                PlayerTakeDamage(1);
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                PlayerTakeHeal(1);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                hitCollider.transform.eulerAngles = new Vector3(0, 0, 180);
                hitCollider.size = new Vector2(2, 1);
                hitCollider.offset = new Vector2(0, -1);
                hitCollider.gameObject.SetActive(true);
                _animator.SetBool(StateExit, false);
                _animator.Play("SwingUp");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                hitCollider.transform.eulerAngles = new Vector3(0, 0, 90);
                hitCollider.size = new Vector2(1, 1.5f);
                hitCollider.offset = new Vector2(0, -1);
                hitCollider.gameObject.SetActive(true);
                _animator.SetBool(StateExit, false);
                _animator.Play("SwingRight");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                hitCollider.transform.eulerAngles = new Vector3(0, 0, 0);
                hitCollider.size = new Vector2(2, 1);
                hitCollider.offset = new Vector2(0, -0.5f);
                hitCollider.gameObject.SetActive(true);
                _animator.SetBool(StateExit, false);
                _animator.Play("SwingDown");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                hitCollider.transform.eulerAngles = new Vector3(0, 0, 270);
                hitCollider.size = new Vector2(1, 1.5f);
                hitCollider.offset = new Vector2(0, -1);
                hitCollider.gameObject.SetActive(true);
                _animator.SetBool(StateExit, false);
                _animator.Play("SwingLeft");
            }
            else
            {
                _change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
                _change.Normalize();
            }

            if (_animator.GetBool(StateExit))
            {
                hitCollider.gameObject.SetActive(false);
                _animator.SetBool(StateExit, false);
                Debug.Log("State Exit");
            }
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


        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Collider hit.");
        }

        public void AddToInventory(GameItem item)
        {
            _player.Inventory.AddItem(item);
        }

        #region PlayerDamage
        public void PlayerTakeDamage(int damage)
        {
            _player.playerstats["CurrentHealth"] -= damage;
            onPlayerLifeChanged.Invoke();
        }

        public void PlayerTakeHeal(int heal)
        {
            _player.playerstats["CurrentHealth"] += heal;
            onPlayerLifeChanged.Invoke();
        }

        public int GetPlayerCurrentHealth()
        {
            return _player.playerstats["CurrentHealth"];
        }

        public int GetPlayerMaxHealth()
        {
            return _player.playerstats["MaxHealth"];
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
                _audioClips.Add("woosh" + i, UnityEngine.Resources.Load<AudioClip>("Wooshes/woosh" + i));
            }

            for (int i = 0; i <= 2; i++)
            {
                _audioClips.Add("footstep0" + i, UnityEngine.Resources.Load<AudioClip>("Footsteps/footstep0" + i));
            }
        }

        #endregion

        #region Getter/Setter and Timer for LightLevel

        //Setter
        public void PlayerChangeLightLevel(int lightlevel)
        {
            _player.playerstats["CurrentLightLevel"] += lightlevel;
            if (_player.playerstats["CurrentLightLevel"] < 0) _player.playerstats["CurrentLightLevel"] = 0;
            onPlayerLightLevelChanged.Invoke();
        }


        //Getter
        public float GetPlayerLightLevel()
        {
            return (float) _player.playerstats["CurrentLightLevel"] / (float) _player.playerstats["MaxLightLevel"];
        }

        public int GetPlayerCurrentLightValue()
        {
            return _player.playerstats["CurrentLightLevel"];
        }

        public int GetPlayerMaxLightValue()
        {
            return _player.playerstats["MaxLightLevel"];
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
    }
}