using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    GameObject mainButtons;
    GameObject play;
    GameObject options;
    GameObject highScores;
    GameObject exit;


    Button playButton;
    Button optionsButton;
    Button highScoresButton;
    Button exitButton;


    // Start is called before the first frame update
    void Start()
    {
        mainButtons = GameObject.Find("MainButtons");
        play = GameObject.Find("Play");
        highScores = GameObject.Find("HighScores");
        options = GameObject.Find("Options");
        exit = GameObject.Find("Exit");

        playButton = play.GetComponent<Button>();
        highScoresButton = highScores.GetComponent<Button>();
        optionsButton = options.GetComponent<Button>();
        exitButton = exit.GetComponent<Button>();

        playButton.onClick.AddListener(PlayGame);
        highScoresButton.onClick.AddListener(LoadHighScores);
        optionsButton.onClick.AddListener(LoadOptions);
        exitButton.onClick.AddListener(ExitOptions);



        LoadMainButtons();
    }

    void LoadMainButtons()
    {
        Debug.Log("load main buttons");

    }

    void LoadHighScores()
    {
        Debug.Log("High scores menu to be implemented");
    }

    void LoadOptions()
    {
        Debug.Log("Options menu to be implemented");
    }

    void ExitOptions()
    {
        Debug.Log("Exit");
        // for quitting a built game:
        Application.Quit();
        //for working in the unity editor:
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void PlayGame()
    {
        Debug.Log("Play Game");
        GameObject.Find("GameController").GetComponent<GameController>().PlayGame();


    }
}
