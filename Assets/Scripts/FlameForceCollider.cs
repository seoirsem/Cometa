using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameForceCollider : MonoBehaviour
{
    public List<Collider2D> forceTriggerList;
    PolygonCollider2D flameForceCollider;


    void Awake()
    {
        forceTriggerList = new List<Collider2D>();
        flameForceCollider = gameObject.GetComponent<PolygonCollider2D>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

            //called when something enters the trigger
    void OnTriggerEnter2D(Collider2D collider)
    {

        //if the object is not already in the list
        if(!forceTriggerList.Contains(collider))
        {
            //add the object to the list
            forceTriggerList.Add(collider);
        }
    }
    
    //called when something exits the trigger
    void OnTriggerExit2D(Collider2D collider)
    {
        //if the object is in the list
        if(forceTriggerList.Contains(collider))
        {
            //remove it from the list
            forceTriggerList.Remove(collider);
        }
    }
}
