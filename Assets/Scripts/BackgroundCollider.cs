using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCollider : MonoBehaviour
{
    BoxCollider2D collider2d;
    public List<GameObject> TriggerList;

    // Start is called before the first frame update
    void Start()
    {
        collider2d = gameObject.GetComponent<BoxCollider2D>();
        collider2d.size = Reference.worldController.worldSize;
        TriggerList  = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


        //called when something enters the trigger
    void OnTriggerEnter2D(Collider2D collider)
    {

        //if the object is not already in the list
        if(!TriggerList.Contains(collider.gameObject))
        {
            //add the object to the list
            TriggerList.Add(collider.gameObject);
        }
    }
    
    //called when something exits the trigger
    void OnTriggerExit2D(Collider2D collider)
    {
        //if the object is in the list
        if(TriggerList.Contains(collider.gameObject))
        {
            //remove it from the list
            TriggerList.Remove(collider.gameObject);
        }
    }
}
