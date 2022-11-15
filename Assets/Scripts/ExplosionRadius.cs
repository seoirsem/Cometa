using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    public List<Collider2D> TriggerList;
    // Start is called before the first frame update
    void Start()
    {
        TriggerList  = new List<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
    //The list of colliders currently inside the trigger
    
    //called when something enters the trigger
    void OnTriggerEnter2D(Collider2D collider)
    {

        //if the object is not already in the list
        if(!TriggerList.Contains(collider))
        {
            //add the object to the list
            TriggerList.Add(collider);
        }
    }
    
    //called when something exits the trigger
    void OnTriggerExit2D(Collider2D collider)
    {
        //if the object is in the list
        if(TriggerList.Contains(collider))
        {
            //remove it from the list
            TriggerList.Remove(collider);
        }
    }
}
