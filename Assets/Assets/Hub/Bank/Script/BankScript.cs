﻿using System;
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

        [SerializeField] private Sprite openChestSprite;
        [SerializeField] private Sprite closedChestSprite;
        [SerializeField] private AudioClip openChestSound;
        [SerializeField] private AudioClip closeChestSound;

        
        

        void Start()
        {
            _interactCollider2D = transform.GetComponentInChildren<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();
            _entered = false;
            _menuOpen = false;

            _storedLightShards = GameManager.GetSaveSystem().BankedShards;

            GameManager.GetEventHandler().onChestClosed.AddListener(OnChestClosed);
            GameManager.GetEventHandler().onChestOpened.AddListener(OnChestOpened);
        }

        public void Update()
        {
            if (_entered && GameManager.isPressingInteractButton)
            {
                if (_menuOpen)
                {
                    Tooltip.ShowTooltip_Static("Press E");
                    GameManager.GetEventHandler().onChestClosed.Invoke();
                }
                else
                {
                    GameManager.GetEventHandler().onChestOpened.Invoke();
                }
            }
        }

        private void OnChestClosed()
        {
            GameManager.GetMenuManagerScript().UnloadCurrentMenu();
            _spriteRenderer.sprite = closedChestSprite;
            _audioSource.clip = closeChestSound;
            if (_menuOpen)
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
                GameManager.GetEventHandler().onChestClosed.Invoke();
                _entered = false;
            }
        }

        public void StoreAllLightShardsInBank()
        {
            _storedLightShards += GameManager.GetPlayerScript().GetPlayerInventory().GetLightShardAmount();
            GameManager.GetPlayerScript().GetPlayerInventory().PlayerSetLightShards(0);
            GameManager.GetSaveSystem().BankedShards = _storedLightShards;

            GameManager.GetEventHandler().onLightShardsStoredInBank.Invoke();

            GameManager.GetSaveSystem().CreateSave();
        }

        public int GetStoredLightShards()
        {
            return _storedLightShards;
        }
    }
}