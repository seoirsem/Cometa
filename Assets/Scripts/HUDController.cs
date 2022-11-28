using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    public GameObject scorego;
    GameObject shieldBar;

    void Start()
    {
        this.scorego = Reference.hud.transform.Find("Score").gameObject;
        this.shieldBar = Reference.hud.transform.Find("ShieldBar").gameObject;
    }

    void Update()
    {
        
    }

    public void UpdateOnScreenScore(float scoreValue)
    {
        int score = Mathf.RoundToInt(scoreValue);
        this.scorego.GetComponent<Text>().text = score.ToString();
        LeanTween.colorText(scorego.GetComponent<RectTransform>(), Color.red, 0.1f);
        LeanTween.scale(scorego, new Vector3(1.2f, 1.2f, 1f), 0.3f);
        LeanTween.colorText(scorego.GetComponent<RectTransform>(), Color.white, 0.1f).setDelay(0.1f);
        LeanTween.scale(scorego, new Vector3(0.8f, 0.8f, 1f), 0.3f).setDelay(0.1f);;
    }
}
