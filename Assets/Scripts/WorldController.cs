using System.Collections;
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
    float asteroidSpawnInterval = 100f;
    float time;

    BoxCollider2D rightEdgeCollider;
    BoxCollider2D leftEdgeCollider;
    BoxCollider2D topEdgeCollider;
    BoxCollider2D bottomEdgeCollider;

    float colliderThickness = 0.01f;

    void Awake()
    {

        time = Time.time;
        worldSize = new Vector2(10f, 8f); //(1300/90, 800/90)
        Reference.CreateReferences();
        windowing = GameObject.Find("Windowing");

        //worldSize = new Vector2(Screen.width)
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
            // Reference.asteroidController.SpawnNewAsteroid();
            time = Time.time;
        }

        if(!isPaused && !escPressed && Reference.playerInputController.escape)
        {
            PauseGame();

        }
        if(escPressed && !Reference.playerInputController.escape)
        {
            //This means escape has been de-pressed
            escPressed = false;
        }
        
        if(isPaused && !escPressed && Reference.playerInputController.escape)
        {
            UnPauseGame();
            escPressed = true;

        }

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

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        //maybe save game state so you can resume it later?
        OptionsParameters.sceneToLoad = mainMenuScene;
        SceneManager.LoadScene(loadingScene);

    }
}
