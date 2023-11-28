using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchToStartController : MonoBehaviour
{
    System.DateTime time0;
    float xChange;
    float yChange;
    float angle;
    float randomRange = 0.25f;
    float alphaChange = 1.2f;
    float timeChange;
    Vector3 spawnPosition;
    Vector3 worldPosition;
    Color color0 = new Color(255, 0, 0);
    Color color;
    Text text;
    float scorePixelRatio;

    float tweenScale;
    float pulseCooldown = 0.1f;
    float lastPulse;
    float timeAwake;

    float originalScaleX;
    float originalScaleY;
    // Start is called before the first frame update
    void Awake()
    {
        originalScaleX = this.gameObject.transform.localScale.x;
        originalScaleY = this.gameObject.transform.localScale.y;
        timeAwake = Time.unscaledTime;
        text = this.GetComponent<Text>();
        tweenScale = 1.1f;
        lastPulse = timeAwake - pulseCooldown;
        color = Color.red;
        // Debug.Log(Color.red);

    }

    void Start()
    {
        scorePixelRatio = Reference.scoreController.scorePixelRatio;
    }
    // Update is called once per frame
    void Update()
    {

        text.color = color;
        timeChange = (float)(System.DateTime.Now - time0).TotalSeconds;
        time0 = System.DateTime.Now;

        this.gameObject.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
        
        //color.a = color.a - alphaChange * timeChange;

        PulseText();
    }

    void PulseText()
    {
        if (Time.unscaledTime - lastPulse > pulseCooldown)
        {
            Debug.Log("Pulsing");
            lastPulse = Time.unscaledTime;
            LeanTween.scaleX(this.gameObject, originalScaleX*1f/tweenScale, pulseCooldown/2f).setIgnoreTimeScale(true);
            LeanTween.scaleX(this.gameObject, originalScaleX*tweenScale, pulseCooldown/2f).setDelay(pulseCooldown/2f).setIgnoreTimeScale(true);
            LeanTween.scaleY(this.gameObject, originalScaleY*1f/tweenScale, pulseCooldown/2f).setIgnoreTimeScale(true);
            LeanTween.scaleY(this.gameObject, originalScaleY*tweenScale, pulseCooldown/2f).setDelay(pulseCooldown/2f).setIgnoreTimeScale(true);
        }
    }


}
