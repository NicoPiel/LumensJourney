using System;
using Core;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.LightBar.Scripts
{
    [BurstCompile]
    public class LightBarScript : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text tmpText;
        
        
        private void Start()
        {
            tmpText.text = "100%";
            slider.value = 1000;
            
            GameManager.GetEventHandler().onPlayerLightLevelChanged.AddListener(ChangeProgress);
        }
        
        private void ChangeProgress()
        {
            var playerMaxLightValue = GameManager.GetPlayer().GetPlayerMaxLightValue();
            var playerCurrentLightValue  = GameManager.GetPlayer().GetPlayerCurrentLightValue();
            slider.maxValue = playerMaxLightValue;
            slider.value = playerCurrentLightValue;
            var temp = ((float) playerCurrentLightValue / playerMaxLightValue); // 980/1000
            
            
            tmpText.text = Math.Ceiling(temp * 100d) + "%";
        }
    
    }
}
