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
        private Slider slider;
        private TMP_Text _tmpText;

        private int _targetValue;
        private int speed;
        void Start()
        {
            _tmpText = gameObject.GetComponentInChildren<TMP_Text>();
            _tmpText.text = "100%";
            slider = gameObject.GetComponent<Slider>();
            slider.value = 1000;
            speed = 10;
        }

        // Update is called once per frame
        void Update()
        {
            /*if (slider.value != _targetValue)
        {
            slider.value += (float) speed * Time.deltaTime;
        }*/
        }

        public void ChangeProgress(int lightChange, int maxlvl)
        {
            slider.maxValue = maxlvl;
            slider.value = lightChange;
            float temp = ((float) lightChange / maxlvl); // 980/1000
            
            
            _tmpText.text = Math.Ceiling(temp * 100d) + "%";
        }
    
    }
}
