using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDamageCollider : MonoBehaviour
{
        
    CapsuleCollider2D flameDamageCollider;

    public List<Collider2D> damageTriggerList;
    public List<Collider2D> forceTriggerList;
    FlameForceCollider flameForceCollider;

    GameObject blueFlame;
    // Start is called before the first frame update
    void Awake()
    {
        blueFlame = GameObject.Find("Blue_Flame");
        flameDamageCollider = this.gameObject.GetComponent<CapsuleCollider2D>();
        flameForceCollider = GameObject.Find("FlameForceCollider").GetComponent<FlameForceCollider>();
        damageTriggerList = new List<Collider2D>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(blueFlame.active)
        {
        forceTriggerList = flameForceCollider.forceTriggerList;
        CalculateForceAndDamage();
        }
    }


    void CalculateForceAndDamage()
    {
        if(forceTriggerList.Count > 0)
        {
            foreach(Collider2D collider in forceTriggerList)
            {
                float distance = collider.ClosestPoint(this.gameObject.transform.position + new Vector3(-0.05f,-0.25f,0)).magnitude;
                Vector2 relativePosition = collider.ClosestPoint(this.gameObject.transform.position + new Vector3(-0.05f,-0.25f,0)) - (Vector2)(this.gameObject.transform.position + new Vector3(-0.05f,-0.25f,0));
                Rigidbody2D hitRigidBody = collider.gameObject.GetComponent<Rigidbody2D>();

                FlameForceCalculation(distance, relativePosition, hitRigidBody);
                
            }

        }

    }

    void FlameForceCalculation(float distance, Vector2 relativePosition, Rigidbody2D hitRigidBody)
    {
        // Debug.Log(hitRigidBody.gameObject.name);
        float shieldAppliedForce = (1/distance) * Time.deltaTime * 100;

        Vector2 shieldForceVector = shieldAppliedForce * relativePosition;
        hitRigidBody.AddForce(shieldForceVector, ForceMode2D.Impulse);

    }





    
        //called when something enters the trigger
    void OnTriggerEnter2D(Collider2D collider)
    {

        //if the object is not already in the list
        if(!damageTriggerList.Contains(collider))
        {
            //add the object to the list
            damageTriggerList.Add(collider);
        }
    }
    
    //called when something exits the trigger
    void OnTriggerExit2D(Collider2D collider)
    {
        //if the object is in the list
        if(damageTriggerList.Contains(collider))
        {
            //remove it from the list
            damageTriggerList.Remove(collider);
        }
    }
}
