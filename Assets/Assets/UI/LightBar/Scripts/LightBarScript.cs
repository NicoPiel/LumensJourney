using System;
using Core;
using TMPro;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.LightBar.Scripts
{
    [BurstCompile]
    public class LightBarScript : MonoBehaviour
    {
        // Start is called before the first frame update
        private Slider _slider;
        private TMP_Text _tmpText;
        
        
        void Start()
        {
            _tmpText = gameObject.GetComponentInChildren<TMP_Text>();
            _tmpText.text = "100%";
            _slider = gameObject.GetComponent<Slider>();
            _slider.value = 1000;
            
            GameManager.GetPlayerScript().onPlayerLightLevelChanged.AddListener(ChangeProgress);
        }
        
        private void ChangeProgress()
        {
            var playerMaxLightValue = GameManager.GetPlayerScript().GetPlayerMaxLightValue();
            var playerCurrentLightValue  = GameManager.GetPlayerScript().GetPlayerCurrentLightValue();
            _slider.maxValue = playerMaxLightValue;
            _slider.value = playerCurrentLightValue;
            var temp = ((float) playerCurrentLightValue / playerMaxLightValue); // 980/1000
            
            
            _tmpText.text = Math.Ceiling(temp * 100d) + "%";
        }
    
    }
}
