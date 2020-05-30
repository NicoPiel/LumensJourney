﻿using System;
using System.Threading.Tasks;
using Assets.Player.Script;
using Core;
using Unity.Burst;
using UnityEngine;

namespace Utility
{
    [BurstCompile]
    public class Door : MonoBehaviour
    {
        [SerializeField] private int lightlevelChange;
        
        private PlayerScript _player;
        private AudioSource _doorSound;

        private bool _entered;

        private void Start()
        {
            _player = GameManager.GetPlayer().GetComponent<PlayerScript>();
            _doorSound = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_entered)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayOpenSound();
                    
                }
            }
        }

        private async void PlayOpenSound()
        {
            _doorSound.Play();
            _player.PlayerChangeLightLevel(lightlevelChange);
            
            while (_doorSound.isPlaying)
            {
                await Task.Yield();
            }
            
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.ShowTooltip_Static("Press E");
                _entered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.HideTooltip_Static();
                _entered = false;
            }
        }

        private void OnDisable()
        {
            GameManager.GetGenerator().onDungeonChanged.Invoke();
        }
    }
}