using UnityEngine;

namespace Assets.LightSphere.Scripts
{
    public class LightSphere : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Teleporter"))
            {
                Destroy(gameObject);
            }
        }
    }
}
