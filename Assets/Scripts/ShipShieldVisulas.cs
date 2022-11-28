using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipShieldVisulas : MonoBehaviour
{

    ShipShields shipShields;
    float maxShieldStrength;
    float shieldStrength;
    Rigidbody2D rigid_body;
    SpriteRenderer spriteRenderer;
    public List<Collider2D> TriggerList;

    void Start()
    {
        shipShields = Reference.shipShields;
        maxShieldStrength = shipShields.maxShieldStrength;
        shieldStrength = shipShields.shieldStrength;

        rigid_body = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        TriggerList  = new List<Collider2D>();
    }

    void Update()
    {
        UpdateFromPlayerShields();
        UpdateVisuals();
        UpdateCollider();
    }

    void UpdateVisuals()
    {   
        CopyTransformFields(Reference.shipShields.gameObject);
    }

    void CopyTransformFields(GameObject from)
    {
        this.gameObject.transform.rotation = from.transform.rotation;
        this.gameObject.transform.localScale = from.transform.localScale;
        this.spriteRenderer.color = from.GetComponent<SpriteRenderer>().color;
    }

    void UpdateFromPlayerShields()
    {
        maxShieldStrength = shipShields.maxShieldStrength;
        shieldStrength = shipShields.shieldStrength;
    }


    void UpdateCollider()
    {
        foreach (Collider2D hitCollider in TriggerList)
        {
            if(hitCollider.gameObject.GetComponent<MainAsteroid>() != null || hitCollider.gameObject.GetComponent<Projectile>() != null)
            {

                float projectileLife = 2f;
                if(hitCollider.gameObject.GetComponent<Projectile>() != null)
                {
                    projectileLife = Time.time - hitCollider.gameObject.GetComponent<Projectile>().timeFired;
                }

                if(shieldStrength > 0 && projectileLife>=1f && hitCollider.isTrigger == false && hitCollider.gameObject.active)
                {//exclude the player itself!


                    Rigidbody2D hitRigidBody = hitCollider.gameObject.GetComponent<Rigidbody2D>();
                    //Rigidbody2D playerRigidBody = playerGameObject.GetComponent<Rigidbody2D>();

                    Vector2 relativeVelocity = hitRigidBody.velocity - rigid_body.velocity;
                    Vector2 relativePosition = hitRigidBody.position - rigid_body.position;

                    Vector3 closestPoint = hitCollider.ClosestPoint(gameObject.transform.position);
                    float distance =  (closestPoint - gameObject.transform.position).magnitude;
                    if (distance < 0.2f){distance = 0.2f;} // this stops the applied force getting too high
                    //float mass = hitCollider.gameObject.GetComponent<Rigidbody2D>().mass;
                    //float playerMass = playergo.GetComponent<Rigidbody2D>().mass;
                
                    shipShields.CollisionCalculation(distance, relativeVelocity, relativePosition, hitRigidBody);
                }
            }
        }
    }
    
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
