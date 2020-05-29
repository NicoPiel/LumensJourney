using System;
using Assets.Player.Script;
using Core;
using Unity.Burst;
using UnityEngine;

namespace Utility
{
    [BurstCompile]
    public class Door : MonoBehaviour
    {
        private PlayerScript _player;
        private AudioSource _doorSound;
        
        private bool _entered;
        private bool _canBeDestroyed;

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

                    _canBeDestroyed = true;
                    
                    _player.PlayerChangeLightLevel(-30);
                }
            }
            
            if (_canBeDestroyed && !_doorSound.isPlaying) gameObject.SetActive(false);
        }

        private void PlayOpenSound()
        {
            _doorSound.Play();
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
