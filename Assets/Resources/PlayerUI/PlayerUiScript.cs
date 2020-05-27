using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUiScript : MonoBehaviour
{
    public GameObject healthbar;

    public GameObject itembar;
    // Start is called before the first frame update
    void Start()
    {
        var hb = Instantiate(healthbar, transform);
        hb.GetComponent<RectTransform>().anchoredPosition = new Vector3(260, -37, 0);

        var ib = Instantiate(itembar, transform);
        ib.GetComponent<RectTransform>().anchoredPosition = new Vector3(210, -115, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
