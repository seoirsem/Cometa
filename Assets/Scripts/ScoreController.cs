using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;


public class ScoreController : MonoBehaviour
{

    public float scorePixelRatio = 100f;
    public float totalScore;
    public float timeElapsed;
    string highScoresPath = "Assets/Resources/SavedFiles/HighScores.bin";


    public float startTime;
    void Start()
    {
        this.totalScore = 0f;
        startTime = Time.time;
    }

    void Update()
    {
        timeElapsed = Time.time - startTime;
    }

    public void IncrementScore(float increment, Vector2 contact)
    {
        float deltaScore = increment*scorePixelRatio;

        this.totalScore += deltaScore;
        Reference.hudController.UpdateOnScreenScore(this.totalScore);
        Reference.soundController.ScorePoints();
        Reference.hudController.ScoreText(contact, deltaScore, new Color(255,215,0));

    }

    public void SaveHighScore()
    {
        ///// This score
        HighScoreEntry highScoreEntry = new HighScoreEntry();
        string dateAchieved = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day;
        highScoreEntry.dateAchieved = dateAchieved;
        highScoreEntry.score = Reference.scoreController.totalScore;
        highScoreEntry.timeElapsed = Reference.scoreController.timeElapsed;
        /////
        BinaryFormatter formatter;
        FileStream stream;

        List<HighScoreEntry> highScoreEntryList= new List<HighScoreEntry>();
        if (File.Exists(highScoresPath))
        {
            formatter = new BinaryFormatter();
            stream = new FileStream(highScoresPath, FileMode.Open);
            highScoreEntryList = formatter.Deserialize(stream) as List<HighScoreEntry>;
//            Debug.Log(highScoreEntryList[0].dateAchieved);
            stream.Close();
        }

        highScoreEntryList = MakeHighScoresList(highScoreEntryList,highScoreEntry);
        formatter = new BinaryFormatter();
        stream = new FileStream(highScoresPath, FileMode.Create);
        formatter.Serialize(stream, highScoreEntryList);
        stream.Close();
//        Debug.Log("Saved high score to file");
    }


    List<HighScoreEntry> MakeHighScoresList(List<HighScoreEntry> highScoreEntryList, HighScoreEntry newEntry)
    {
        List<HighScoreEntry> newList = new List<HighScoreEntry>();

        if(highScoreEntryList.Count < 10)
        {
            newList = highScoreEntryList;
            newList.Add(newEntry);
        }
        else
        {
            newList = highScoreEntryList;
            float newScore = newEntry.score;

            int index = minimumScoreIndex(highScoreEntryList);
            float minimumScore = highScoreEntryList[index].score;

            if(newScore > minimumScore)
            {
                newList[index] = newEntry;
            }

        }

        return newList;
    }

    int minimumScoreIndex(List<HighScoreEntry> highScoreEntryList)
    {
        float lowestScore = -1;
        int lowestIndex = -1;
        for(int i = 0; i < highScoreEntryList.Count; i++)
        {
            if(lowestScore == -1)
            {
                lowestScore = highScoreEntryList[i].score;
                lowestIndex = i;
            }
            else
            {
                if(highScoreEntryList[i].score < lowestScore)
                {
                    lowestScore = highScoreEntryList[i].score;
                    lowestIndex = i;
                }
            }
        }


        return lowestIndex;
    }

}
