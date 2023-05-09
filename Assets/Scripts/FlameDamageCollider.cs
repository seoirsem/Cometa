using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDamageCollider : MonoBehaviour
{
        
    CapsuleCollider2D flameDamageCollider;
    float radiusOfDestruction = 4f;

    public List<Collider2D> damageTriggerList;
    public List<Collider2D> forceTriggerList;
    FlameForceCollider flameForceCollider;

    float damageCooldown = 0.5f;
    float timeLastDamaged;

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
        timeLastDamaged = Time.time - damageCooldown;
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
                if(collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    Rigidbody2D hitRigidBody = collider.gameObject.GetComponent<Rigidbody2D>();
                    FlameForceCalculation(distance, relativePosition, hitRigidBody);   
                }
            }
        }

        if(damageTriggerList.Count > 0 && Time.time - timeLastDamaged > damageCooldown)
        {
            timeLastDamaged = Time.time;
            foreach(Collider2D collider in damageTriggerList)
            {
                if (collider.gameObject.GetComponent<Asteroid>() != null)
                    {
                        Vector2 position = (Vector2)(this.gameObject.transform.position + new Vector3(-0.05f,-0.25f,0));
                        // this is janky repeating code which can probably be fixed by using derived classes better oh well
                        // we will also want to damage nearby asteroids in the radius too
                        if(collider.gameObject.GetComponent<MainAsteroid>() != null)
                        {
                            MainAsteroid asteroid = collider.gameObject.GetComponent<MainAsteroid>();
                            asteroid.ApplyFlames(position, radiusOfDestruction, this.gameObject);
                            //Debug.Log("Main Asteroid in explosion radius");
                        }
                        else
                        {
                            DerivedAsteroid asteroid = collider.gameObject.GetComponent<DerivedAsteroid>();
                            asteroid.ApplyFlames(position, radiusOfDestruction, this.gameObject);
                        }
                    }
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
