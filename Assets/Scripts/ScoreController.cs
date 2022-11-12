using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{

    public float totalScore;

    void Start()
    {
        this.totalScore = 0f;
    }

    void Update()
    {
        
    }

    public void IncrementScore(float increment)
    {
        this.totalScore += increment;
        Reference.hudcontroller.UpdateOnScreenScore(this.totalScore);
    }
}
