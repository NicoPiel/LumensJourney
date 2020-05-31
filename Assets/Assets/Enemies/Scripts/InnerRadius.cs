using Core;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Enemies.Scripts
{
    public class InnerRadius : MonoBehaviour
    {
        public UnityEvent onPlayerWalkIntoRange;
        
        private Enemy _parent;

        private bool _inInner = false;

        private void Awake()
        {
            onPlayerWalkIntoRange = new UnityEvent();
        }

        private void Start()
        {
            _parent = transform.parent.GetComponent<Enemy>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.Equals(GameManager.GetPlayerScript().GetCollider()))
            {
                _inInner = true;
                onPlayerWalkIntoRange.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) _inInner = false;
        }

        public bool IsInInner()
        {
            return _inInner;
        }
    }
}
