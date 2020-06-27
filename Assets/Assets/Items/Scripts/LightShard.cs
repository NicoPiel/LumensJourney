using System;
using System.Collections;
using Core;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Items.Scripts
{
    public class LightShard : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private GameObject _player;
        private ParticleSystem _particleSystem;

        [SerializeField] private float speed;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _player = GameManager.GetUnityPlayerObject();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void FixedUpdate()
        {
            Vector2 shardPosition = transform.position;
            Vector2 playerPosition = _player.transform.position + (Vector3) GameManager.GetPlayerScript().GetCollider().offset;
            Vector2 playerDirection = playerPosition - shardPosition;
            var playerDistance = playerDirection.magnitude;

            Vector2 movePosition = shardPosition + playerDirection * 1 / playerDistance * (speed * Time.fixedDeltaTime);
            
            _rigidbody.MovePosition(movePosition);
        }

        private IEnumerator OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GameManager.GetPlayerScript().GetPlayerInventory().PlayerChangeLightShards(1);
                _particleSystem.Play();
                yield return new WaitForSeconds(0.1f);
                Destroy(gameObject);
            }
        }
    }
}
