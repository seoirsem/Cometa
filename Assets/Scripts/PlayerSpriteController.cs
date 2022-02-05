using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    Player player;
    GameObject playergo;
    PlayerInputController playerInputController;
    float rotationRate = 1; //radians/s
    Reference reference;

    void Start()
    {
        playerInputController = Reference.playerInputController;
        playergo = Reference.playergo;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerMotion();
    }

    void UpdatePlayerMotion()
    {

    }



}
