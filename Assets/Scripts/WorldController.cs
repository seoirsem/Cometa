using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    GameObject playergo;
    public Player player;
    Reference reference;
    // Start is called before the first frame update
    public Vector2 worldSize = new Vector2(10, 8); //(1300/90, 800/90)
    GameObject windowing;

    void Awake()
    {
        Reference.CreateReferences();
        playergo = Reference.playergo;
        player = new Player(playergo);
        windowing = GameObject.Find("Windowing");

        //worldSize = new Vector2(Screen.width)
        windowing.transform.localScale = new Vector3(2*worldSize.x, 2*worldSize.y, 1);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
