using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLogoController : MonoBehaviour
{
    Button logoButton;
    // Start is called before the first frame update
    void Start()
    {
        logoButton = gameObject.GetComponent<Button>();
        logoButton.onClick.AddListener(LogoClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LogoClicked()
    {
        Debug.Log("Logo clicked, going to HBA website");
        Application.OpenURL("https://www.honeybadgerattitude.com/");
    }
}
