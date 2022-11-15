using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFlameFunction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TransitionToFullJet()
    {
        Debug.Log("fullFlame");
    }


    public void DestroyAnimationGO()
    {
        Destroy(this.gameObject);
    }
}
