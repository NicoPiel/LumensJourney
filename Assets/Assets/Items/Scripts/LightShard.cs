using System;
using System.Collections;
using Core;
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
            _player = GameManager.GetPlayer();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void FixedUpdate()
        {
            Vector2 shardPosition = transform.position;
            Vector2 playerPosition = _player.transform.position + (Vector3) GameManager.GetPlayerScript().GetCollider().offset;
            Vector2 playerDirection = playerPosition - shardPosition;
            
            _rigidbody.MovePosition(shardPosition + (playerDirection * speed * Time.fixedDeltaTime));
        }

        private IEnumerator OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GameManager.GetPlayerScript().PlayerChangeLightShards(1);
                _particleSystem.Play();
                yield return new WaitForSeconds(0.1f);
                Destroy(gameObject);
            }
        }
    }
}
