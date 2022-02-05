using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reference
{
    public static PlayerInputController playerInputController;
    public static GameObject playergo;
    public static GameObject backgroundgo;
    // Start is called before the first frame update
    public static void CreateReferences()
    {
        playergo = GameObject.Find("Player");
        playerInputController = GameObject.Find("PlayerInputController").GetComponent<PlayerInputController>();
        backgroundgo = GameObject.Find("Background");

    }


}
