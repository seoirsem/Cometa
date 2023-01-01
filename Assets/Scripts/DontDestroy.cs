using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Object.DontDestroyOnLoad example.
//

public class DontDestroy : MonoBehaviour
{
    float loadedTime;
    bool loadedTimer = false;
    void Awake()
    {
        // GameObject[] objs = GameObject.FindGameObjectsWithTag("music");

        // if (objs.Length > 1)
        // {
        //     Destroy(this.gameObject);
        // }

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if(GameObject.Find("LevelLoadingController") == null && !loadedTimer)
        {
            loadedTimer = true;
            loadedTime = Time.time;
        }
        if(loadedTimer && Time.time - loadedTime >0.01f)
        {
            Destroy(this.gameObject);
        }
    }
}