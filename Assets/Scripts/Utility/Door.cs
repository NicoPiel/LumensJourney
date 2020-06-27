using System;
using System.Threading.Tasks;
using Assets.Player.Script;
using Cinemachine;
using Core;
using Unity.Burst;
using UnityEngine;

namespace Utility
{
    [BurstCompile]
    public class Door : MonoBehaviour
    {
        [SerializeField] private float screenShakeMagnitude;
        [SerializeField] private int lightlevelChange;

        private CinemachineImpulseSource _impulseSource;
        
        private Player _player;
        private AudioSource _doorSound;

        private bool _entered;

        private void Start()
        {
            _player = GameManager.GetPlayer();
            _doorSound = GetComponent<AudioSource>();
            _impulseSource = GetComponent<CinemachineImpulseSource>();
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

            var clipLength = _doorSound.clip.length / 2;

            _impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = clipLength/4;
            _impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = clipLength/2;
            _impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = clipLength/4;
            _impulseSource.GenerateImpulse(new Vector2(screenShakeMagnitude,screenShakeMagnitude));

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
                Tooltip.Tooltip.ShowTooltip_Static("Press E");
                _entered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tooltip.Tooltip.HideTooltip_Static();
                _entered = false;
            }
        }

        private void OnDisable()
        {
            GameManager.GetEventHandler().onDungeonChanged.Invoke();
        }
    }
}
