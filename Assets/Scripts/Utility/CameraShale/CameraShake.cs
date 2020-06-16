using System.Collections;
using UnityEngine;

namespace Utility
{
    public class CameraShake : MonoBehaviour
    {
        public static IEnumerator Shake (Camera cam, float duration, float magnitude)
        {
            Vector3 originalPosition = cam.transform.position;

            var elapsed = 0.0f;

            while (elapsed < duration)
            {
                var x = Random.Range(-1f, 1f) * magnitude;
                var y = Random.Range(-1f, 1f) * magnitude;
                
                cam.transform.localPosition = new Vector3(x, y,originalPosition.z);

                elapsed += Time.deltaTime;

                yield return null;
            }

            cam.transform.localPosition = originalPosition;
        }
    }
}
