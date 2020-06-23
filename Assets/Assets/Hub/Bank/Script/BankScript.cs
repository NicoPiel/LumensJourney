using System;
using Core;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Utility.Tooltip;

namespace Assets.Hub.Bank.Script
{
    public class BankScript : MonoBehaviour
    {
        private BoxCollider2D _interactCollider2D;
        private SpriteRenderer _spriteRenderer;
        private AudioSource _audioSource;
        private bool _entered;
        private bool _menuOpen;
        private int _storedLightShards;

        public Sprite openChestSprite;
        public Sprite closedChestSprite;
        public AudioClip openChestSound;
        public AudioClip closeChestSound;

        public UnityEvent onLightShardsStoredInBank;
        public UnityEvent onChestClosed;
        public UnityEvent onChestOpened;

        // Start is called before the first frame update
        private void OnEnable()
        {
            onChestClosed = new UnityEvent();
            onChestOpened = new UnityEvent();
            onLightShardsStoredInBank = new UnityEvent();
        }

        void Start()
        {
        
            _interactCollider2D = transform.GetComponentInChildren<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();
            _entered = false;
            _menuOpen = false;
        
            _storedLightShards = GameManager.GetSaveSystem().BankedShards;
            
            onChestClosed.AddListener(OnChestClosed);
            onChestOpened.AddListener(OnChestOpened);
            
            
            
            
        }

        public void Update()
        {
            if (_entered && Input.GetKeyDown(KeyCode.E))
            {
                if (_menuOpen)
                {
                    Tooltip.ShowTooltip_Static("Press E");
                    onChestClosed.Invoke();
                    
                }
                else
                {
                    onChestOpened.Invoke();
                }
            }

        }

        private void OnChestClosed()
        {
            GameManager.GetMenuManagerScript().UnloadCurrentMenu();
            _spriteRenderer.sprite = closedChestSprite;
            if(_menuOpen)
                _audioSource.Play();
            _menuOpen = false;

        }

        private void OnChestOpened()
        {
            Tooltip.HideTooltip_Static();
            _menuOpen = true;
            _spriteRenderer.sprite = openChestSprite;
            _audioSource.clip = openChestSound;
            _audioSource.Play();
            GameManager.GetMenuManagerScript().LoadMenu("BankMenu");
        }

        // Update is called once per frame
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Tooltip.ShowTooltip_Static("Press E");
                _entered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Tooltip.HideTooltip_Static();
                onChestClosed.Invoke();
                _entered = false;
            }
        }

        public void StoreAllLightShardsInBank()
        {
            _storedLightShards += GameManager.GetPlayerScript().GetLightShardAmount();
            GameManager.GetPlayerScript().PlayerSetLightShards(0);
            GameManager.GetSaveSystem().BankedShards = _storedLightShards;
        
            onLightShardsStoredInBank.Invoke();
        
            GameManager.GetSaveSystem().CreateSave();
        }

        public int GetStoredLightShards()
        {
            return _storedLightShards;
        }
    }
}