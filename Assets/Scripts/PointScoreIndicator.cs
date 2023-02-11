using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointScoreIndicator : MonoBehaviour
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
        timeAwake = Time.time;
        text = this.GetComponent<Text>();
        tweenScale = 1.1f;
        lastPulse = timeAwake - pulseCooldown;

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

        worldPosition = new Vector3(worldPosition.x + timeChange * xChange, worldPosition.y + timeChange * yChange, worldPosition.z);
        this.gameObject.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
        
        color.a = color.a - alphaChange * timeChange;
        if(color.a <= 0)
        {
            SimplePool.Despawn(this.gameObject);
        }
        PulseText();
    }

    public void SpawnText(float points, Vector3 position, Color colour)//, Vector3 position)
    {
        angle = Random.Range(-Mathf.PI, Mathf.PI);
        xChange = 2 * Mathf.Cos(angle);
        yChange = 2 * Mathf.Sin(angle);
        //Debug.Log(angle);
        worldPosition = position;
        this.color = colour;
        time0 = System.DateTime.Now;
        int fontSize = 35 + 2*Mathf.RoundToInt(points/scorePixelRatio);
        if(fontSize>115){fontSize = 115;}
        text.fontSize = fontSize;
        this.gameObject.SetActive(true);
        //this.gameObject.transform.position = position;
        text.text = Mathf.Round(points).ToString();
        

    }

    void PulseText()
    {
        if (Time.time - lastPulse > pulseCooldown)
        {
            lastPulse = Time.time;
            LeanTween.scaleX(this.gameObject, originalScaleX*1f/tweenScale, pulseCooldown/2f);
            LeanTween.scaleX(this.gameObject, originalScaleX*tweenScale, pulseCooldown/2f).setDelay(pulseCooldown/2f);
            LeanTween.scaleY(this.gameObject, originalScaleY*1f/tweenScale, pulseCooldown/2f);
            LeanTween.scaleY(this.gameObject, originalScaleY*tweenScale, pulseCooldown/2f).setDelay(pulseCooldown/2f);
        }
    }

}
