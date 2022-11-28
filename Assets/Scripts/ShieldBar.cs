using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBar : MonoBehaviour
{
    GameObject activeBar;
    GameObject background;
    RectTransform activeRect;
    ShipShields shipShields;
    float maxShieldStrength;
    float shieldStrength;

    void Start()
    {
        activeBar = transform.Find("Active").gameObject;
        activeRect = activeBar.GetComponent<RectTransform>();
        shipShields = Reference.shipShields;
    }

    // Update is called once per frame
    void Update()
    {
        if(shieldStrength != shipShields.shieldStrength)
        {
            maxShieldStrength = shipShields.maxShieldStrength;
            shieldStrength = shipShields.shieldStrength;
            float proportion = shieldStrength/maxShieldStrength;
            activeRect.localPosition = new Vector2(-125f * proportion/2f +62.5f,0);
            activeRect.sizeDelta = new Vector2 (proportion * 125f,15f);
        }
    }
}
