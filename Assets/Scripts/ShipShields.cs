﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipShields : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxShieldStrength = 100f;
    public float shieldStrength;
    Rigidbody2D playerRigidBody;
    GameObject playerGameObject;
    Rigidbody2D rigid_body;
    Collider2D collider;
    public List<Collider2D> TriggerList;
    float lastHit;
    float lastPulse;
    float pulseCooldown = 1.5f;
    float shieldForceMultiplier;
    SpriteRenderer spriteRenderer;

    float shieldRechargeRate = 5f; //per second
    float shieldChargeDelay = 2.5f; //seconds
    float shieldForceRatio = 5f;

    float shieldDamageRadiusToAsteroids = 1.5f;
    float matterDestroyedShieldPenalty = 5f;

    void Start()
    {
        shieldStrength = 100f;
        playerRigidBody = Reference.playergo.GetComponent<Rigidbody2D>();
        TriggerList  = new List<Collider2D>();
        playerGameObject = Reference.playergo;
        rigid_body = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        lastHit = Time.time;
        lastPulse = Time.time;
        shieldForceMultiplier = playerRigidBody.mass;

    }


    
    // Update is called once per frame
    void Update()
    {
        TrackPlayer();
        UpdateCollisions();
        UpdateCharge(Time.deltaTime);

    }

    void TrackPlayer()
    {
        rigid_body.position = playerRigidBody.position;
        rigid_body.rotation = playerRigidBody.rotation;
    }

    void UpdateCollisions()
    {
        foreach (Collider2D hitCollider in TriggerList)
        {

            float projectileLife = 2f;
            if(hitCollider.gameObject.GetComponent<Projectile>() != null)
            {
                projectileLife = Time.time - hitCollider.gameObject.GetComponent<Projectile>().timeFired;
            }

            if(hitCollider.gameObject.GetComponent<PlayerSpriteController>() == null && shieldStrength > 0 &&
                projectileLife>=1f && hitCollider.isTrigger == false && hitCollider.gameObject.active && 
                hitCollider.gameObject.layer != LayerMask.NameToLayer("SpawningAsteroid"))
            {//exclude the player itself!

            Rigidbody2D hitRigidBody = hitCollider.gameObject.GetComponent<Rigidbody2D>();
            //Rigidbody2D playerRigidBody = playerGameObject.GetComponent<Rigidbody2D>();

            Vector2 relativeVelocity = hitRigidBody.velocity - playerRigidBody.velocity;
            Vector2 relativePosition = hitRigidBody.worldCenterOfMass - playerRigidBody.position;

            Vector3 closestPoint = hitCollider.ClosestPoint(playerGameObject.transform.position);
            float distance =  (closestPoint - playerGameObject.transform.position).magnitude;
            if (distance < 0.25f){distance = 0.25f;} // this stops the applied force getting too high
//            Debug.Log(distance);
            //Debug.Break();
            //float mass = hitCollider.gameObject.GetComponent<Rigidbody2D>().mass;
            //float playerMass = playergo.GetComponent<Rigidbody2D>().mass;
           
            CollisionCalculation(distance, relativeVelocity, relativePosition, hitRigidBody);

            if(hitCollider.gameObject.GetComponent<MainAsteroid>() != null)
            {
                hitCollider.gameObject.GetComponent<MainAsteroid>().MainAsteroidHitShields(distance, relativeVelocity, relativePosition, new Vector2(closestPoint.x,closestPoint.y), shieldDamageRadiusToAsteroids);
            }
            else if(hitCollider.gameObject.GetComponent<DerivedAsteroid>() != null)
            {
                hitCollider.gameObject.GetComponent<DerivedAsteroid>().DerivedAsteroidHitShields(distance, relativeVelocity, relativePosition, new Vector2(closestPoint.x,closestPoint.y), shieldDamageRadiusToAsteroids);

            }
            else if(hitCollider.gameObject.GetComponent<Projectile>() != null)
            {
                Projectile projectile = hitCollider.gameObject.GetComponent<Projectile>();
                if(projectile.colliderEnabled)
                {
                    projectile.DestroySelf();
                }
            }

            }
        }
    }
    public void ShieldsDestroyedAsteroidSquares(int numberOfSquaresLost)
    {
        //A shield penalty when the shields destroy asteroid matter
        OnHit(numberOfSquaresLost * matterDestroyedShieldPenalty/shieldForceRatio);
    }
    public void ShieldsInExplosionRadius(float explosionImpulse, Projectile projectile)
    {   
        Debug.Log(explosionImpulse);
        // /Debug.Log(explosionImpulse/(shieldForceRatio*7));
        if(explosionImpulse/(shieldForceRatio*400) > 5f)
        {
            OnHit(explosionImpulse/(shieldForceRatio*400));

        }
    }

    public void CollisionCalculation(float distance, Vector2 relativeVelocity, Vector2 relativePosition, Rigidbody2D hitRigidBody)
    {
        // Debug.Log(hitRigidBody.gameObject.name);
        float shieldAppliedForce = (1/distance) * (1/distance) * relativeVelocity.magnitude * Time.deltaTime * shieldForceMultiplier;
        if(shieldAppliedForce > 50*shieldForceMultiplier){shieldAppliedForce = 50f*shieldForceMultiplier;}

        Vector2 shieldForceVector = shieldAppliedForce * relativePosition;
//        Debug.Log(shieldForceMultiplier);
        playerRigidBody.AddForce(-1 * shieldForceVector, ForceMode2D.Impulse);
        hitRigidBody.AddForce(shieldForceVector, ForceMode2D.Impulse);
        OnHit(shieldAppliedForce/shieldForceMultiplier);
        
    }

    
    void UpdateCharge(float dt)
    {   
        if(Time.time - lastHit > shieldChargeDelay)
        {
            shieldStrength += shieldRechargeRate*dt;
            if(shieldStrength > maxShieldStrength){shieldStrength = maxShieldStrength;}
        }
        if(shieldStrength == 0f)
        {
            //ToDo an animation when shields burst
            Color tmp = spriteRenderer.color;
            tmp.a = 0;
            this.spriteRenderer.color = tmp; 
        }
        // Debug.Log(Time.time - lastShimmer);
        // Debug.Log(Time.time);
        else
            {
            float strengthRange = 1f;
            Color tmp = spriteRenderer.color;
            tmp.a = strengthRange;
            if (Time.time - lastPulse > pulseCooldown)
            {
                this.spriteRenderer.color = tmp;
                float strengthFraction = shieldStrength/maxShieldStrength;
                lastPulse = Time.time;
                float scale = 1.15f;

                LeanTween.scaleX(this.gameObject, 1f/scale, pulseCooldown/2f);
                LeanTween.scaleX(this.gameObject, scale, pulseCooldown/2f).setDelay(pulseCooldown/2f);
                LeanTween.scaleY(this.gameObject, scale, pulseCooldown/2f).setDelay(pulseCooldown*1f/4f);
                LeanTween.scaleY(this.gameObject, 1f/scale, pulseCooldown/2f).setDelay(pulseCooldown*3f/4f);
                LeanTween.rotate(this.gameObject, new Vector3(0f,0f,180f), pulseCooldown/2f);
                LeanTween.rotate(this.gameObject, new Vector3(0f,0f,360f), pulseCooldown/2f).setDelay(pulseCooldown/2f);

                if (strengthFraction < 0.5f)
                {
                    if (strengthFraction < 0.25f){strengthRange = 0.25f;}
                    else{strengthRange = 0.5f;}
                    tmp.a = strengthRange;
                    this.spriteRenderer.color = tmp;
                    float part = 12f;
                    float randFrac = (float)Random.Range(1,(int)part);
                    if (Random.Range(0f, 0.5f) > strengthFraction)
                    {
                        LeanTween.alpha(this.gameObject, strengthFraction, pulseCooldown/part);
                        LeanTween.alpha(this.gameObject, strengthRange, pulseCooldown/part).setDelay(pulseCooldown/part);
                        LeanTween.alpha(this.gameObject, strengthFraction, pulseCooldown/part).setDelay(pulseCooldown*randFrac/part);
                        LeanTween.alpha(this.gameObject, strengthRange, pulseCooldown/part).setDelay(pulseCooldown*(1+randFrac)/part);
                    }
                }
            }
        }
    }
    void OnHit(float shieldForce)
    {

        lastHit = Time.time;
        shieldStrength -= shieldForceRatio*shieldForce;
//        Debug.Log(shieldForceRatio*shieldForce);

        if(shieldStrength < 0){shieldStrength = 0;}
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
