using System;
using UnityEngine;

namespace Assets.Enemies.Scripts
{
    public class InnerRadius : MonoBehaviour
    {
        private bool _inInner = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) _inInner = true;
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
