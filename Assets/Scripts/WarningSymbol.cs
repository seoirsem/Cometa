using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningSymbol : MonoBehaviour
{
    SpriteRenderer sr;
    float timeAwake;
    float timeActive;

    float timeRed = 1f;
    float timeYellow;

    bool redWarning;

    float tweenScale;
    float pulseCooldown = 0.5f;
    float lastPulse;

    Color red;
    Color yellow;
    // Start is called before the first frame update
    void Awake()
    {
        redWarning = false;
        sr = gameObject.GetComponent<SpriteRenderer>();
        timeAwake = Time.time;
        red = new Color(255,0,0);
        yellow = new Color(255,255,0);
        sr.color = yellow;
        tweenScale = 1.1f;
        lastPulse = timeAwake - pulseCooldown;
    }

    public void OnSpawn(float timeActive, float timeRed)
    {
        //this.transform.position = location;
        this.timeActive = timeActive;

    }
    // Update is called once per frame
    void Update()
    {
        if(Time.time - timeAwake > timeActive - timeRed)
        {
            sr.color = red;
        }
        if(Time.time - timeAwake > timeActive)
        {
            GameObject.Destroy(gameObject);
        }

        if (Time.time - lastPulse > pulseCooldown)
        {
            lastPulse = Time.time;
            LeanTween.scaleX(this.gameObject, 1f/tweenScale, pulseCooldown/2f);
            LeanTween.scaleX(this.gameObject, tweenScale, pulseCooldown/2f).setDelay(pulseCooldown/2f);
            LeanTween.scaleY(this.gameObject, 1f/tweenScale, pulseCooldown/2f);
            LeanTween.scaleY(this.gameObject, tweenScale, pulseCooldown/2f).setDelay(pulseCooldown/2f);
            // if(sr.color == yellow)
            // {
            //     LeanTween.color(this.gameObject,red,pulseCooldown/2f);//.setDelay(pulseCooldown/4f);
            //     LeanTween.color(this.gameObject,yellow,pulseCooldown/2f).setDelay(pulseCooldown/2f);
            // }
        
        }
    }
}
