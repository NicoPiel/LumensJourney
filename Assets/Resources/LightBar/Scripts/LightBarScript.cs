using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBarScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Slider slider;

    private int _targetValue;
    private int speed;
    void Start()
    {
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
    }
    
}
