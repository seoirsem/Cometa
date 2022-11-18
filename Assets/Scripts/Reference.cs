using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reference
{
    public static GameObject backgroundgo;
    public static GameObject playergo;
    public static GameObject hud;

    public static AsteroidController asteroidController;
    public static AnimationController animationController;
    public static HUDController hudcontroller;
    // public static PlayerEdgeVisualsController playerEdgeVisualsController;
    public static PlayerInputController playerInputController;
    public static PlayerSpriteController playerSpriteController;
    public static ProjectileController projectileController;
    public static ScoreController scoreController;
    public static WorldController worldController;
    public static ShipShields shipShields;
    // public static ShipShieldVisuals;
    
    
    // Start is called before the first frame update
    public static void CreateReferences()
    {

        backgroundgo = GameObject.Find("Background");
        playergo = GameObject.Find("Player");
        hud = GameObject.Find("HUD");

        asteroidController = GameObject.Find("AsteroidController").GetComponent<AsteroidController>();
        animationController = GameObject.Find("AnimationController").GetComponent<AnimationController>();
        hudcontroller = GameObject.Find("HUDController").GetComponent<HUDController>();
        // playerEdgeVisualsController = GameObject.Find("PlayerInputController").GetComponent<PlayerEdgeVisualsController>();
        playerInputController = GameObject.Find("PlayerInputController").GetComponent<PlayerInputController>();
        playerSpriteController = GameObject.Find("Player").GetComponent<PlayerSpriteController>();
        projectileController = GameObject.Find("ProjectileController").GetComponent<ProjectileController>();
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
        shipShields = GameObject.Find("ShipShields").GetComponent<ShipShields>();

    }


}
