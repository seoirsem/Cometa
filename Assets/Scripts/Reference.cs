using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reference
{
    public static AsteroidController asteroidController;
    public static PlayerEdgeVisualsController playerEdgeVisualsController;
    public static PlayerInputController playerInputController;
    public static PlayerSpriteController playerSpriteController;
    public static ProjectileController projectileController;
    public static WorldController worldController;
    public static GameObject backgroundgo;
    public static GameObject playergo;
    
    // Start is called before the first frame update
    public static void CreateReferences()
    {
        asteroidController = GameObject.Find("PlayerInputController").GetComponent<AsteroidController>();
        playerEdgeVisualsController = GameObject.Find("PlayerInputController").GetComponent<PlayerEdgeVisualsController>();
        playerInputController = GameObject.Find("PlayerInputController").GetComponent<PlayerInputController>();
        playerSpriteController = GameObject.Find("Player").GetComponent<PlayerSpriteController>();
        projectileController = GameObject.Find("ProjectileController").GetComponent<ProjectileController>();
        worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
        backgroundgo = GameObject.Find("Background");
        playergo = GameObject.Find("Player");
    }


}
