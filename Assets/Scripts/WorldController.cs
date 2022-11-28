using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    GameObject playergo;
    public Player player;
    public Vector2 worldSize;
    GameObject windowing;

    float asteroidSpawnInterval = 10f;
    float time;

    void Awake()
    {

        time = Time.time;
        worldSize = new Vector2(10, 8); //(1300/90, 800/90)
        Reference.CreateReferences();
        windowing = GameObject.Find("Windowing");

        //worldSize = new Vector2(Screen.width)
        windowing.transform.localScale = new Vector3(2*worldSize.x, 2*worldSize.y, 1);


    }

    void Start() {
        playergo = Reference.playergo;
        player = new Player(playergo);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - time > asteroidSpawnInterval)
        {
            Reference.asteroidController.SpawnNewAsteroid();
            time = Time.time;
        }   
    }
}
