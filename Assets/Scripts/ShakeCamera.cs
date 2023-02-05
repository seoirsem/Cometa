using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://www.youtube.com/watch?v=BQGTdRhGmE4&ab_channel=ThomasFriday
public class ShakeCamera : MonoBehaviour
{
    public AnimationCurve animationCurve;
    public bool start = false;
    float duration;
    float strength;

    public void StartShake(float duration, float strength)
    {
        this.duration = duration;
        this.strength = strength;
        start = true;
    }
    void Awake()
    {
        duration = 0.15f;
        strength = 0.2f;
    }

    void Update()
    {
        if(start) {
            start = false;
            StartCoroutine(Shaking());
        }        
    }

    IEnumerator Shaking() 
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = animationCurve.Evaluate(elapsedTime/duration);
            transform.position = startPosition + Random.insideUnitSphere*strength;
            yield return null;
        }
        transform.position = startPosition;
    }


}
