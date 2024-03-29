﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WorldController : MonoBehaviour
{
    GameObject playergo;
    public Player player;
    public Vector2 worldSize;
    GameObject windowing;
    public bool isPaused = false;
    bool escPressed = false;
    string mainMenuScene = "Scenes/MainMenu";
    string loadingScene = "Scenes/LoadingScene";
    string gameScene = "Scenes/GameScene";
    GameObject warningPrefab;
    float asteroidSpawnInterval = 10f;
    float time;
    bool gameOver = false;

    BoxCollider2D rightEdgeCollider;
    BoxCollider2D leftEdgeCollider;
    BoxCollider2D topEdgeCollider;
    BoxCollider2D bottomEdgeCollider;

    float colliderThickness = 0.01f;

    float freqOfNewAsteroids = 10f;
    float warningLifespan = 3f;
    int newAsteroidDirection;
    Vector3 asteroidPositionOffset;
    public string platform;
    bool warningOff = true;

    public int maxAsteroidSize = 20;
    public int minAsteroidSize = 10;
    float howToTime = 10f;
    float howToStart;

    float spawnCooldown;

    bool how_to_play_active = false;


    void Awake()
    {

        time = Time.time;
         //(1300/90, 800/90)
        Reference.CreateReferences();
        windowing = GameObject.Find("Windowing");

        worldSize = new Vector2(18f, 15f);

        windowing.transform.localScale = new Vector3(2*worldSize.x, 2*worldSize.y, 1);

        rightEdgeCollider = transform.Find("RightEdgeCollider").GetComponent<BoxCollider2D>();
        leftEdgeCollider = transform.Find("LeftEdgeCollider").GetComponent<BoxCollider2D>();
        topEdgeCollider = transform.Find("TopEdgeCollider").GetComponent<BoxCollider2D>();
        bottomEdgeCollider = transform.Find("BottomEdgeCollider").GetComponent<BoxCollider2D>();

        rightEdgeCollider.gameObject.transform.position = new Vector3(Reference.worldController.worldSize.x/2f + colliderThickness, 0f, 0f);
        leftEdgeCollider.gameObject.transform.position = new Vector3(-(Reference.worldController.worldSize.x/2f + colliderThickness), 0f, 0f);
        rightEdgeCollider.size = new Vector2(colliderThickness, Reference.worldController.worldSize.y);
        leftEdgeCollider.size = new Vector2(colliderThickness, Reference.worldController.worldSize.y);

        topEdgeCollider.gameObject.transform.position = new Vector3(0f, Reference.worldController.worldSize.y/2f + colliderThickness, 0f);
        bottomEdgeCollider.gameObject.transform.position = new Vector3(0f, -(Reference.worldController.worldSize.y/2f + colliderThickness), 0f);
        topEdgeCollider.size = new Vector2(Reference.worldController.worldSize.x, colliderThickness);
        bottomEdgeCollider.size = new Vector2(Reference.worldController.worldSize.x, colliderThickness);
        if(Application.platform == RuntimePlatform.WindowsPlayer)
        {
 //           Debug.Log("On PC");
            platform = "Windows";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            //Debug.Log("Android");
            platform = "Android";
        }
        else if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            platform = "Editor";
//            Debug.Log("Unity Editor");
        }
        else
        {
            platform = "Windows";
        }

        
        // Debug.Log("The game is running on the following platform: " + platform);
        // platform = "Android";
        //Debug.Log("Debugging Only");
    }

    void Start() 
    {
        playergo = Reference.playergo;
        player = new Player(playergo);
        spawnCooldown = Time.time;

        int randomSize = Random.Range(minAsteroidSize, maxAsteroidSize);
        //Debug.Log(randomSize);
        warningPrefab = Resources.Load("Prefabs/WarningSymbol") as GameObject;
        Reference.asteroidController.SpawnNewAsteroid(20, 2, new Vector3(0,2.5f,0), new Vector3(0,0,0));
        if(platform == "Android")
        {
            ShowControlIcons();
            howToStart = Time.unscaledTime;
        }
        //Reference.asteroidController.SpawnNewAsteroid(randomSize,2, new Vector3(0,0,0));
        //Reference.asteroidController.SpawnRandomAsteroid(20, new Vector3(0,2.5f,0));
        // Reference.asteroidController.SpawnNewAsteroid(10,0, new Vector3(0,0,0), new Vector3(5f,0,0));
        // Reference.asteroidController.SpawnNewAsteroid(10,1, new Vector3(0,0,0), new Vector3(-5f,0,0));

    }

    // Update is called once per frame
    void Update()
    {
        if(platform == "Android" && how_to_play_active && (Time.unscaledTime - howToStart > howToTime ||
          (Reference.playerInputController.touchAnywhere && Time.unscaledTime - howToStart > 0.8f)))
        {
            HideControlIcons();
        }

        if (Time.time - time > asteroidSpawnInterval)
        {
            // Reference.asteroidController.SpawnNewAsteroid();
            time = Time.time;
        }

        if(!isPaused && !escPressed && Reference.playerInputController.escape && !gameOver)
        {
            PauseGame();

        }
        if(escPressed && !Reference.playerInputController.escape)
        {
            //This means escape has been de-pressed
            escPressed = false;
        }
        
        if(isPaused && !escPressed && Reference.playerInputController.escape && !gameOver) 
        {
            UnPauseGame();
            escPressed = true;

        }
        if(Time.time - spawnCooldown > freqOfNewAsteroids - warningLifespan && warningOff)
        {
            Debug.Log("1");
            warningOff = false;
            
            newAsteroidDirection = Random.Range(0,4); //left, right, up, down

            if(freqOfNewAsteroids > 4f)
            {
                freqOfNewAsteroids -= 0.5f;
            }
            
            int[] numbers = {-1,0,1,0,0,1,0,-1};
            Vector3 spawnPosition = new Vector3(numbers[newAsteroidDirection*2]*worldSize.x/2,numbers[newAsteroidDirection*2+1]*worldSize.y/2,0) + 
                                    new Vector3(numbers[newAsteroidDirection*2]*-0.5f,numbers[newAsteroidDirection*2+1]*-0.5f,0);
            asteroidPositionOffset = GenerateAsteroidPositionOffset(newAsteroidDirection);
            GameObject warningSymbol = Instantiate(warningPrefab,spawnPosition + asteroidPositionOffset,Quaternion.identity);
            warningSymbol.GetComponent<WarningSymbol>().OnSpawn(warningLifespan,1f);
        }
        /// ToDo increase the freuency of asteroid spawns as the player gets further and further
        if(Time.time - spawnCooldown > freqOfNewAsteroids && !isPaused)
        {
            // Debug.Break();
            //// ToDo: Warn the player of incoming a few seconds before spawning    
             //spawnn new asteroid
            spawnCooldown = Time.time;
            warningOff = true;
            // Reference.asteroidController.SpawnNewAsteroid(20, 2, new Vector3(0,2.5f,0), new Vector3(0,0,0));
            Reference.asteroidController.SpawnNewAsteroid(Random.Range(minAsteroidSize, maxAsteroidSize),newAsteroidDirection, asteroidPositionOffset,new Vector3(0,0,0));
        }

        if ((Reference.playerInputController.p && Time.time - spawnCooldown > 2f) && !isPaused)
        {
            // Debug.Break();
            spawnCooldown = Time.time;
            int randomInt = Random.Range(0,3);
            // Reference.asteroidController.SpawnNewAsteroid(Random.Range(minAsteroidSize, maxAsteroidSize), randomInt, GenerateAsteroidPositionOffset(randomInt),new Vector3(0,0,0));   
        }

    }
    Vector3 GenerateAsteroidPositionOffset(int direction)
    {
        if(direction == 0 || direction == 1)
        { //left or right
            return new Vector3(0,Random.Range(-worldSize.y/2.5f,worldSize.y/2.5f),0);
        }
        else
        { // up or down
            return new Vector3(Random.Range(-worldSize.x/2.5f,worldSize.x/2.5f),0,0);
        }
    }


    public void PlayerDead()
    {
        if(!gameOver)
        {
            Reference.soundController.PlayerDeadSound();
            Time.timeScale = 0;
            isPaused = true;

            gameOver = true;
            Reference.hudController.GameOverUI(Reference.scoreController.totalScore);
        }
    }

    public void ShowControlIcons()
    {
        Reference.hudController.DisablePauseMenu();
        Reference.playerInputController.escape = false;
        Debug.Log("Game Paused");
        how_to_play_active = true;
        isPaused = true;
        escPressed = true;
        Time.timeScale = 0;
        howToStart = Time.unscaledTime;
        Reference.hudController.ShowControlDescriptions();
    }
    void HideControlIcons()
    {
        Debug.Log("Game Resuming");
        isPaused = false;
        how_to_play_active = false;
        escPressed = false;
        Time.timeScale = 1;
        Reference.hudController.HideControlDescriptions();
    }

    void PauseGame()
    {
        Debug.Log("Game Paused");
        isPaused = true;
        escPressed = true;
        Time.timeScale = 0;
        Reference.hudController.EnablePauseMenu();
    }

    public void UnPauseGame()
    {
        Debug.Log("Game Resuming");
        isPaused = false;;
        Time.timeScale = 1;
        Reference.hudController.DisablePauseMenu();

    }

    public void ReplayLevel()
    {
        Reference.scoreController.SaveHighScore();
        Time.timeScale = 1;
        //maybe save game state so you can resume it later?
        OptionsParameters.sceneToLoad = gameScene;
        SceneManager.LoadScene(loadingScene);
    }

    public void QuitToMainMenu()
    {
        Reference.scoreController.SaveHighScore();
        Time.timeScale = 1;
        //maybe save game state so you can resume it later?

        OptionsParameters.sceneToLoad = mainMenuScene;
        SceneManager.LoadScene(loadingScene);
    }
}
