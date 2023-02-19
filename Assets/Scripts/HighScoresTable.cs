using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;



public class HighScoresTable : MonoBehaviour
{
    MenuSoundController menuSoundController;

    string highScoresPath;// = "Assets/Resources/SavedFiles/HighScores.bin";
    string returnButtonPath = "Prefabs/highScoreReturn";
    string prefabPath = "Prefabs/HighScore";
    GameObject highScorePrefab;
    GameObject returnButtonPrefab;
    MainUIController mainUIController;

    // Start is called before the first frame update
    void Start()
    {
        highScoresPath = Application.persistentDataPath + "HighScores.bin";

        highScorePrefab = Resources.Load(prefabPath) as GameObject;
        returnButtonPrefab = Resources.Load(returnButtonPath) as GameObject;
        mainUIController = GameObject.Find("UI").GetComponent<MainUIController>();
        menuSoundController = GameObject.Find("MenuSoundController").GetComponent<MenuSoundController>();

    }

    public void MakeScoreMenu()
    {
        List<HighScoreEntry> highScores = LoadHighScores();
        
        int numberShown = 5;
        if(numberShown > highScores.Count)
        {
            numberShown = highScores.Count;
        }

        highScores.Sort((p1,p2)=>p1.score.CompareTo(p2.score));
        List<HighScoreEntry> highScoreSorted = new List<HighScoreEntry>();
        for(int i=highScores.Count-1; i>=0; i--)
        {
            highScoreSorted.Add(highScores[i]);
        }


        for(int i=0; i<numberShown; i++)
        {   
            HighScoreEntry score = highScoreSorted[i];
            GameObject highScore = GameObject.Instantiate(highScorePrefab);

            highScore.transform.SetParent(this.transform);
            highScore.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            //highScore.GetComponent<RectTransform>().localPosition = new Vector3(highScore.GetComponent<RectTransform>().localPosition.x,highScore.GetComponent<RectTransform>().localPosition.y,-3);

            Michsky.UI.Shift.SpotlightButton spotlight = highScore.GetComponent<Michsky.UI.Shift.SpotlightButton>();
            spotlight.buttonTitle = "Score: " + score.score.ToString("F1") + " -- Time " + score.timeElapsed.ToString("F2") + "s";
            spotlight.buttonDescription = score.dateAchieved;
        }
        GameObject returnButton = GameObject.Instantiate(returnButtonPrefab);
        returnButton.transform.SetParent(this.transform);
        returnButton.GetComponent<Michsky.UI.Shift.UIElementSound>().audioObject = menuSoundController.gameObject.GetComponent<AudioSource>();
        returnButton.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        returnButton.GetComponent<Button>().onClick.AddListener(CloseScoreMenu);


    }

    public void CloseScoreMenu()
    {
        foreach (Transform transform in this.transform)
        {
            GameObject.Destroy(transform.gameObject);
        }
        mainUIController.LoadMainButtons();
    }

    List<HighScoreEntry> LoadHighScores()
    {
        BinaryFormatter formatter;
        FileStream stream;
        List<HighScoreEntry> highScoreEntryList= new List<HighScoreEntry>();
        if (File.Exists(highScoresPath))
        {
            formatter = new BinaryFormatter();
            stream = new FileStream(highScoresPath, FileMode.Open);
            highScoreEntryList = formatter.Deserialize(stream) as List<HighScoreEntry>;
            Debug.Log(highScoreEntryList[0].dateAchieved);
            stream.Close();
        }

        return highScoreEntryList;
    }

}
