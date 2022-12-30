using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    string gameScene;
    // Start is called before the first frame update
    void Start()
    {
        gameScene = "Scenes/GameScene";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("p"))
        {
               PlayGame();
        }
        if(Input.GetKey("escape"))
        {
            
        }
    }


    public void PlayGame()
    {
        // Unload main menu UI
        // Put up loading screen
        // Pull down loading screen when loaded

        // countdown timer? Then start playing

        //Debug.Log("Playing Game");
        SceneManager.LoadScene(gameScene);

    }

}


