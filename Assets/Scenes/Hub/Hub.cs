using Core;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utility;


namespace Scenes.Hub
{
    [BurstCompile]
    public class Hub : MonoBehaviour
    {
        private void Start()
        {
            GameObject player = GameManager.GetGameManager_Static().GetPlayer();
            
            if (Camera.main != null && player != null)
            {
                var playerLight = player.transform.Find("PlayerLight").GetComponent<Light2D>();
                playerLight.intensity = 0;
                
                var volume = Camera.main.GetComponent<Volume>();

                ChromaticAberration chromaticAberration;
                LensDistortion lensDistortion;

                volume.profile.TryGet(out chromaticAberration);
                volume.profile.TryGet(out lensDistortion);

                chromaticAberration.intensity.value = 0;
                lensDistortion.intensity.value = 0;

                player.transform.Find("PlayerLight").GetComponent<LightFlickerEffect>().enabled = false;
            }
        }
    }
}
