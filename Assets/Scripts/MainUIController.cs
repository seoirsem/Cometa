using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{

    MenuSoundController menuSoundController;

    string mainButtonsPath;
    string optionsButtonsPath;
    GameObject mainButtonsPrefab;
    GameObject optionsButtonsPrefab;
    /// Main Buttons

    GameObject mainButtons;
    GameObject play;
    GameObject options;
    GameObject highScores;
    GameObject exit;

    Button playButton;
    Button optionsButton;
    Button highScoresButton;
    Button exitButton;

    /// Options Buttons

    GameObject optionsButtons;
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

    GameObject highScoresMenu;
    HighScoresTable highScoresTable;


    // Start is called before the first frame update
    void Start()
    {

        mainButtonsPath = "Prefabs/MainButtons";
        mainButtonsPrefab = Resources.Load(mainButtonsPath) as GameObject;
        optionsButtonsPath = "Prefabs/OptionsButtons";
        optionsButtonsPrefab = Resources.Load(optionsButtonsPath) as GameObject;

        highScoresMenu = GameObject.Find("HighScoresMenu");
        highScoresTable = highScoresMenu.GetComponent<HighScoresTable>();

        menuSoundController = GameObject.Find("MenuSoundController").GetComponent<MenuSoundController>();
        ///// Initial startup
        LoadMainButtons();

    }

    void Update()
    {
        // if(optionsButtons != null)
        // {
        //    Debug.Log(musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.SwitchManager>().isOn);
        // }
    }

    public void LoadMainButtons()
    {


        ///////// Main buttons

        mainButtons = GameObject.Instantiate(mainButtonsPrefab);
        mainButtons.transform.SetParent(this.transform);
        mainButtons.GetComponent<RectTransform>().localScale = new Vector3(3,3,1);
        mainButtons.GetComponent<RectTransform>().localPosition = new Vector3(mainButtons.GetComponent<RectTransform>().localPosition.x,mainButtons.GetComponent<RectTransform>().localPosition.y,-3);


        play = GameObject.Find("Play");
        highScores = GameObject.Find("HighScores");
        options = GameObject.Find("Options");
        exit = GameObject.Find("Exit");

        play.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();
        highScores.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();
        options.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();
        exit.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();


        playButton = play.GetComponent<Button>();
        highScoresButton = highScores.GetComponent<Button>();
        optionsButton = options.GetComponent<Button>();
        exitButton = exit.GetComponent<Button>();

        playButton.onClick.AddListener(PlayGame);
        highScoresButton.onClick.AddListener(LoadHighScores);
        optionsButton.onClick.AddListener(OptionsButtonPressed);
        exitButton.onClick.AddListener(ExitOptions);

        //mainButtons.SetActive(true);
        //optionsButtons.SetActive(false);
        //ResetButtons();
    }
    void UnloadMainButtons()
    {
        Destroy(mainButtons);
    }

    void LoadHighScores()
    {
        UnloadMainButtons();
        highScoresTable.MakeScoreMenu();
    }

    void LoadOptions()
    {        
        ///////// Options buttons

        optionsButtons = GameObject.Instantiate(optionsButtonsPrefab);
        optionsButtons.transform.SetParent(this.transform);
        optionsButtons.GetComponent<RectTransform>().localScale = new Vector3(3,3,1);
        optionsButtons.GetComponent<RectTransform>().localPosition = new Vector3(mainButtons.GetComponent<RectTransform>().localPosition.x,mainButtons.GetComponent<RectTransform>().localPosition.y,-3);

        masterVolumeTogglego = GameObject.Find("MasterVolumeToggle");
        masterVolumeSwitchgo = GameObject.Find("SoundSwitch");
        musicVolumeTogglego = GameObject.Find("MusicVolumeToggle");
        musicVolumeSwitchgo = GameObject.Find("MusicSwitch");
        optionsReturngo = GameObject.Find("OptionsReturn");

        masterVolumeTogglego.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();
        masterVolumeSwitchgo.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();
        musicVolumeTogglego.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();
        musicVolumeSwitchgo.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();
        optionsReturngo.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();

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
    void UnloadOptions()
    {
        Destroy(optionsButtons);
    }


    void ExitOptions()
    {
        Debug.Log("Exit");
        // for quitting a built game:
        Application.Quit();
        //for working in the unity editor:
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    void PlayGame()
    {
        Debug.Log("Play Game");
        GameObject.Find("GameController").GetComponent<GameController>().PlayGame();
    }

    void OptionsButtonPressed()
    {
        UnloadMainButtons();
        LoadOptions();
    }

    void OptionsReturnButtonPressed()
    {
        UnloadOptions();
        LoadMainButtons();
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
            menuSoundController.SetMasterVolume(1);
            if(!musicSwitch)
            {
                MusicVolumeTogglePressed();
            }
        }
        else
        {
            menuSoundController.SetMasterVolume(0);
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
            menuSoundController.SetMusicVolume(1);
        }
        else
        {
            menuSoundController.SetMusicVolume(0);
        }
    }
}
