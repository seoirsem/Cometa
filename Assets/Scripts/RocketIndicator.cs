using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    Sprite rocket;
    Sprite rocketGray;
    Image image;
    bool ImageGray;
    PlayerSpriteController playerSpriteController;

    float tweenScale = 1.1f;
    float pulseCooldown = 0.2f;

    void Start()
    {
        ImageGray = false;
        rocket = Resources.Load<Sprite>("Sprites/rocket") as Sprite;
        rocketGray = Resources.Load<Sprite>("Sprites/rocketGray") as Sprite;
        Debug.Log(rocket);
        Debug.Log(rocketGray);
        playerSpriteController = Reference.playerSpriteController;
        image = this.gameObject.GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
     if(playerSpriteController.RocketOnCooldown && !ImageGray)
     {
        ImageGray = true;
        image.sprite = rocketGray;
     }   
     if(!playerSpriteController.RocketOnCooldown && ImageGray)
     {
        ImageGray = false;
        image.sprite = rocket;
        LeanTween.scaleX(this.gameObject, 1f/tweenScale, pulseCooldown/2f);
        LeanTween.scaleX(this.gameObject, tweenScale, pulseCooldown/2f).setDelay(pulseCooldown/2f);
        LeanTween.scaleY(this.gameObject, 1f/tweenScale, pulseCooldown/2f);
        LeanTween.scaleY(this.gameObject, tweenScale, pulseCooldown/2f).setDelay(pulseCooldown/2f);
     }
    }
}
