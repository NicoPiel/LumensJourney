using Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Scenes.Hub
{
    public class Hub : MonoBehaviour
    {
        private void Start()
        {
            if (Camera.main != null && GameManager.Player != null)
            {
                var playerLight = GameManager.Player.transform.Find("PlayerLight").GetComponent<Light2D>();
                playerLight.intensity = 0;
                
                var volume = Camera.main.GetComponent<Volume>();

                ChromaticAberration chromaticAberration;
                LensDistortion lensDistortion;

                volume.profile.TryGet(out chromaticAberration);
                volume.profile.TryGet(out lensDistortion);

                chromaticAberration.intensity.value = 0;
                lensDistortion.intensity.value = 0;
            }
        }
    }
}
