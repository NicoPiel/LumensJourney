using Assets.Player.Script;
using Core;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Enemies.Scripts
{
    public class InnerRadius : MonoBehaviour
    {
        public UnityEvent onPlayerWalkIntoRange;
        public UnityEvent onHit;
        
        private Enemy _parent;

        private bool _inInner = false;

        private void Awake()
        {
            onPlayerWalkIntoRange = new UnityEvent();
            onHit = new UnityEvent();
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
            
            if (other.Equals(GameManager.GetPlayerScript().hitCollider))
            {
                PlayerScript playerScipt = GameManager.GetPlayerScript();

                _parent.TakeDamage(playerScipt.GetPlayerDamage());
                
                Vector3 moveDirection =  _parent.GetRigidBody().transform.position - playerScipt.transform.position;
                _parent.GetRigidBody().AddForce( moveDirection.normalized * -500f);

                Debug.Log($"{_parent.gameObject.name} was hit by the player.");
                
                onHit.Invoke();
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
