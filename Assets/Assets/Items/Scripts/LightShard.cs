using System;
using Core;
using UnityEngine;

namespace Assets.Items.Scripts
{
    public class LightShard : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private GameObject _player;

        [SerializeField] private float speed;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _player = GameManager.GetPlayer();
        }

        private void FixedUpdate()
        {
            Vector2 vectorToPlayer = _player.transform.position - transform.position;
            
            _rigidbody.velocity = vectorToPlayer * speed * Time.fixedDeltaTime;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                GameManager.GetPlayerScript().PlayerChangeLightShards(1);
            }
        }
    }
}
