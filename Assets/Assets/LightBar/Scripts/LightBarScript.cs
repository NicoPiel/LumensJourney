using System;
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

        private int _targetValue;
        private int speed;
        
        void Start()
        {
            _tmpText = gameObject.GetComponentInChildren<TMP_Text>();
            _tmpText.text = "100%";
            _slider = gameObject.GetComponent<Slider>();
            _slider.value = 1000;
            speed = 10;
        }
        
        public void ChangeProgress(int lightChange, int maxlvl)
        {
            _slider.maxValue = maxlvl;
            _slider.value = lightChange;
            float temp = ((float) lightChange / maxlvl); // 980/1000
            
            
            _tmpText.text = Math.Ceiling(temp * 100d) + "%";
        }
    
    }
}
