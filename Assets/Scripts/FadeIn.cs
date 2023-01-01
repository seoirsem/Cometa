using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    float startTime;
    Image image;
    float fadeTime = 10f;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.unscaledTime;
        image = GetComponent<Image>();
        SetAlpha(0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.unscaledTime - startTime < fadeTime)
        {
            
            SetAlpha((Time.unscaledTime - startTime)/fadeTime);
        }
    }

    void SetAlpha(float alpha)
    {
        Color tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }
}
