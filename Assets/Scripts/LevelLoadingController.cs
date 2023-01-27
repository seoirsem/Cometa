using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoadingController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadNewScene());
    }

    IEnumerator LoadNewScene()
    {
            bool isLoading = true;
 
            AsyncOperation async = SceneManager.LoadSceneAsync(OptionsParameters.sceneToLoad);
            async.allowSceneActivation = false;
            while(!async.isDone) {
                if(Mathf.Approximately(async.progress, 0.9f)) {
                    async.allowSceneActivation = true;
                    //FadeScreenFromBlack(1.0f);
                }
                yield return null;
            }
 
            yield return new WaitForEndOfFrame();
            isLoading = false;
            yield return null;
    }
    


}
