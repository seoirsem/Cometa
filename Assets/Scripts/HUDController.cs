using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    public GameObject scorego;
    GameObject shieldBar;
    GameObject pauseMenu;  

    Button resume;
    Button options;
    Button exitToMenu;

    void Start()
    {
        this.scorego = Reference.hud.transform.Find("Score").gameObject;
        this.shieldBar = Reference.hud.transform.Find("ShieldBar").gameObject;
        pauseMenu = GameObject.Find("PauseMenu");


        resume = GameObject.Find("Resume").GetComponent<Button>();
        options = GameObject.Find("Options").GetComponent<Button>();
        exitToMenu = GameObject.Find("ExitToMenu").GetComponent<Button>();
        resume.onClick.AddListener(ResumeButton);
        options.onClick.AddListener(Options);
        exitToMenu.onClick.AddListener(EscapeToMenu);

        pauseMenu.SetActive(false);
    }

    void Update()
    {
        
    }

    public void UpdateOnScreenScore(float scoreValue)
    {
        int score = Mathf.RoundToInt(scoreValue);
        this.scorego.GetComponent<Text>().text = score.ToString();
        LeanTween.colorText(scorego.GetComponent<RectTransform>(), Color.red, 0.1f);
        LeanTween.scale(scorego, new Vector3(1.2f, 1.2f, 1f), 0.3f);
        LeanTween.colorText(scorego.GetComponent<RectTransform>(), Color.white, 0.1f).setDelay(0.1f);
        LeanTween.scale(scorego, new Vector3(0.8f, 0.8f, 1f), 0.3f).setDelay(0.1f);;
    }

    public void EnablePauseMenu()
    {
        pauseMenu.SetActive(true);
        //also deactivate other HUD options
    }

    public void DisablePauseMenu()
    {
        pauseMenu.SetActive(false);
    }

    void ResumeButton()
    {
        Debug.Log("Resuming...");
        Reference.worldController.UnPauseGame();
    }

    void Options()
    {
        Debug.Log("Options menu to be implemented");
    }

    void EscapeToMenu()
    {
        //first save score perhaps? Maybe save your game state?
        
        Reference.worldController.QuitToMainMenu();
    }
}
