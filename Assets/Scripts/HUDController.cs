using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    string pauseMenuPath;
    string optionsButtonsPath;
    string gameOverPath;

    public GameObject scorego;
    GameObject shieldBar;
    GameObject pauseMenu;  
    GameObject pauseMenuPrefab;
    GameObject optionsButtons;
    GameObject optionsButtonsPrefab;
    GameObject gameOverScreen;
    GameObject gameOverScreenPrefab;

    SoundController soundController;

    /// Options Buttons

    GameObject masterVolumeSwitchgo;
    GameObject masterVolumeTogglego;
    GameObject musicVolumeSwitchgo;
    GameObject musicVolumeTogglego;
    GameObject optionsReturngo;

    Button masterVolumeToggle;
    Button masterVolumeSwitch;
    Button musicVolumeToggle;
    Button musicVolumeSwitch;
    Button optionsReturn;

    Button resume;
    Button options;
    Button exitToMenu;


    void Start()
    {
        this.scorego = Reference.hud.transform.Find("Score").gameObject;
        this.shieldBar = Reference.hud.transform.Find("ShieldBar").gameObject;
        
        //this.score.transform.position = 

        pauseMenuPath = "Prefabs/PauseMenu";
        pauseMenuPrefab = Resources.Load(pauseMenuPath) as GameObject;

        optionsButtonsPath = "Prefabs/OptionsButtons";
        optionsButtonsPrefab = Resources.Load(optionsButtonsPath) as GameObject;

        gameOverPath = "Prefabs/GameOverScreen";
        gameOverScreenPrefab = Resources.Load(gameOverPath) as GameObject;
        
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();

    }

    void LoadPauseMenu()
    {   
        pauseMenu = GameObject.Instantiate(pauseMenuPrefab);
        pauseMenu.transform.SetParent(this.transform);
        pauseMenu.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        pauseMenu.GetComponent<RectTransform>().localPosition = new Vector3(0,0,-3);

        resume = GameObject.Find("Resume").GetComponent<Button>();
        options = GameObject.Find("Options").GetComponent<Button>();
        exitToMenu = GameObject.Find("ExitToMenu").GetComponent<Button>();

        GameObject.Find("Resume").GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();
        GameObject.Find("Options").GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();
        GameObject.Find("ExitToMenu").GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();


        resume.onClick.AddListener(ResumeButton);
        options.onClick.AddListener(Options);
        exitToMenu.onClick.AddListener(EscapeToMenu);        
    }
    void UnloadPauseMenu()
    {
        Destroy(pauseMenu);
    }
    public void GameOverUI(float finalScore)
    {
        gameOverScreen = GameObject.Instantiate(gameOverScreenPrefab);
        gameOverScreen.transform.SetParent(this.transform);
        gameOverScreen.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        gameOverScreen.GetComponent<RectTransform>().localPosition = new Vector3(0,0,-3);

        gameOverScreen.GetComponent<FinalScoreFadeIn>().SetScore(finalScore);


    }


    void LoadOptionsMenu()
    {
        optionsButtons = GameObject.Instantiate(optionsButtonsPrefab);
        optionsButtons.transform.SetParent(this.transform);
        optionsButtons.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        optionsButtons.GetComponent<RectTransform>().localPosition = new Vector3(0,0,-3);

        masterVolumeTogglego = GameObject.Find("MasterVolumeToggle");
        masterVolumeSwitchgo = GameObject.Find("SoundSwitch");
        musicVolumeTogglego = GameObject.Find("MusicVolumeToggle");
        musicVolumeSwitchgo = GameObject.Find("MusicSwitch");
        optionsReturngo = GameObject.Find("OptionsReturn");

        masterVolumeTogglego.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();
        masterVolumeSwitchgo.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();
        musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();
        musicVolumeTogglego.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();
        optionsReturngo.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = Reference.soundController.gameObject.GetComponent<AudioSource>();

        masterVolumeToggle = masterVolumeTogglego.GetComponent<Button>();
        masterVolumeSwitch = masterVolumeSwitchgo.GetComponent<Button>();
        musicVolumeSwitch = musicVolumeSwitchgo.GetComponent<Button>();
        musicVolumeToggle = musicVolumeTogglego.GetComponent<Button>();
        optionsReturn = optionsReturngo.GetComponent<Button>();

        optionsReturn.onClick.AddListener(OptionsReturnButtonPressed);
        musicVolumeToggle.onClick.AddListener(MusicVolumeTogglePressed);
        musicVolumeSwitch.onClick.AddListener(MusicVolumeSwitchPressed);
        masterVolumeToggle.onClick.AddListener(MasterVolumeTogglePressed);
        masterVolumeSwitch.onClick.AddListener(MasterVolumeSwitchPressed);

        float masterVolumeSetting = OptionsParameters.MasterVolume;
        float musicVolumeSetting = OptionsParameters.MusicVolume;

        if(masterVolumeSetting == 1f)
        {
            masterVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().isOn = true;
            masterVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().AnimateSwitch();
            masterVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().AnimateSwitch();
        }
        if(musicVolumeSetting == 1f)
        {
            musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().isOn = true;
            musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().AnimateSwitch();
            musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().AnimateSwitch();
        }

    }
    void UnloadOptionsMenu()
    {
        Destroy(optionsButtons);
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
        LoadPauseMenu();
        //also deactivate other HUD options
    }

    public void DisablePauseMenu()
    {
        UnloadOptionsMenu();
        UnloadPauseMenu();
    }

    void ResumeButton()
    {
        Reference.worldController.UnPauseGame();
    }

    void Options()
    {
        UnloadPauseMenu();
        LoadOptionsMenu();
    }

    void EscapeToMenu()
    {
        //first save score perhaps? Maybe save your game state?
        
        Reference.worldController.QuitToMainMenu();
    }

     void OptionsReturnButtonPressed()
    {
        UnloadOptionsMenu();
        LoadPauseMenu();
    }

    void MasterVolumeTogglePressed()
    {
        masterVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().AnimateSwitch();
        MasterVolumeSwitchPressed();
    }

    void MasterVolumeSwitchPressed()
    {
        bool masterSwitch = masterVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().isOn;  
        bool musicSwitch = musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().isOn;        
        
        if(masterSwitch)
        {
            soundController.SetMasterVolume(1);
            if(!musicSwitch)
            {
                MusicVolumeTogglePressed();
            }
        }
        else
        {
            soundController.SetMasterVolume(0);
            if(musicSwitch)
            {
                MusicVolumeTogglePressed();
            }
        }
        
    }

    void MusicVolumeTogglePressed()
    {
        musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().AnimateSwitch();
        MusicVolumeSwitchPressed();
    }

    void MusicVolumeSwitchPressed()
    {
        bool musicSwitch = musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().isOn;        
        if(musicSwitch)
        {
            soundController.SetMusicVolume(1);
        }
        else
        {
            soundController.SetMusicVolume(0);
        }
    }
}
