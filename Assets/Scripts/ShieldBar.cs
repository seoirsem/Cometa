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

    float tweenDelay = 0.1f;
    float lastTween;

    void Start()
    {
        lastTween = Time.time;
        activeBar = transform.Find("Active").gameObject;
        activeRect = activeBar.GetComponent<RectTransform>();
        shipShields = Reference.shipShields;

        Debug.Log(activeRect);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"Strength: {shipShields.shieldStrength}, old strength: {shieldStrength}");
        if(shieldStrength != shipShields.shieldStrength)
        {
            maxShieldStrength = shipShields.maxShieldStrength;
            shieldStrength = shipShields.shieldStrength;
            float proportion = shieldStrength/maxShieldStrength;
            activeRect.localPosition = new Vector2(-125f * proportion/2f +62.5f,0);
            activeRect.sizeDelta = new Vector2 (proportion * 125f,15f);

            if(Time.time - lastTween > tweenDelay)
            {
                lastTween = Time.time;
                //LeanTween.colorText(activeRect, Color.red, 0.1f);
                //LeanTween.scale(activeBar, new Vector3(1.2f, 1.2f, 1f), 0.3f);
                //LeanTween.colorText(activeRect, Color.white, 0.1f).setDelay(0.1f);
                //LeanTween.scale(activeBar, new Vector3(0.8f, 0.8f, 1f), 0.3f).setDelay(0.1f);;
            }
        }
    }
}
