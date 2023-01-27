using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreFadeIn : MonoBehaviour
{
    GameObject gameOver;
    Text gameOverText;
    GameObject finalScore;
    Text finalScoreText;

    GameObject replay;
    Button replayButton;
    GameObject mainMenu;
    Button mainMenuButton;

    float startTime;

    float fadeTimeScore = 8f;
    float fadeTimeGame = 5f;
    bool buttonsActive = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameOver = GameObject.Find("GameOverText");
        gameOverText = gameOver.GetComponent<Text>();
        finalScore = GameObject.Find("FinalScoreText");
        finalScoreText = finalScore.GetComponent<Text>();
        startTime = Time.unscaledTime;

        replay = GameObject.Find("Replay");
        replayButton = replay.GetComponent<Button>();

        mainMenu = GameObject.Find("MainMenu");
        mainMenuButton = mainMenu.GetComponent<Button>();

        replayButton.onClick.AddListener(ReplayButtonClicked);
        mainMenuButton.onClick.AddListener(MainMenuClicked);

        replay.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();
        mainMenu.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();

        replay.SetActive(false);
        mainMenu.SetActive(false);
        //gameOverText.text
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.unscaledTime - startTime < fadeTimeGame)
        {
            SetAlpha((Time.unscaledTime - startTime)/fadeTimeGame, gameOverText);
        }
        if(Time.unscaledTime - startTime < fadeTimeScore)
        {
            SetAlpha((Time.unscaledTime - startTime)/fadeTimeScore, finalScoreText);
        }
        if(!buttonsActive && Time.unscaledTime - startTime > 4f)
        {
            replay.SetActive(true);
            mainMenu.SetActive(true);
        }
    }

    public void SetScore(float score)
    {
        finalScoreText.text = "Final Score: " + score.ToString("F1");
    }

    void SetAlpha(float alpha, Text text)
    {
        Color tempColor = text.color;
        tempColor.a = alpha;
        text.color = tempColor;
    }

    void ReplayButtonClicked()
    {
        Reference.worldController.ReplayLevel();
    }

    void MainMenuClicked()
    {
        Reference.worldController.QuitToMainMenu();
    }
}
