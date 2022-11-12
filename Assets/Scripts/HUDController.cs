using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    public GameObject scorego;

    void Start()
    {
        this.scorego = Reference.hud.transform.Find("Score").gameObject;
    }

    void Update()
    {
        
    }

    public void UpdateOnScreenScore(float scoreValue)
    {
        this.scorego.GetComponent<Text>().text = scoreValue.ToString();
    }
}
