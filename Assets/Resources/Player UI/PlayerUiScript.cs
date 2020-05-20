using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUiScript : MonoBehaviour
{
    public GameObject healthbar;
    // Start is called before the first frame update
    void Start()
    {
        var hb = Instantiate(healthbar, transform);
        hb.GetComponent<RectTransform>().anchoredPosition = new Vector3(260, -37, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
