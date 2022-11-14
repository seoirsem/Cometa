﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector2 screenCenter;// (0,-1,1 in each of x and y to determine which of the 5 screens it is on)
    GameObject go;
    float timeFired;
    float lifespan = 2;//s
    Vector2 worldSize;
    float projectileSpeed = 11;
    float explosionSize = 2f; // to modulate how much explosion thrust is applied to surrounding objects
    float minExplosionRadius = 0.25f; //to avoid huge impulses near the explosion 
    float thrust = 1f;
    public List<GameObject> objectPack;
    public bool mainProjectile;
    bool leftPlayerCollider = false;
    CapsuleCollider2D capsuleCollider2D;
    Rigidbody2D rigid_body;
    float rotationalPosition;
    GameObject explosionRadiusGO;
    ExplosionRadius explosionRadius;


    bool animationStarted = false;
    GameObject blueFlameAnimation;

    void Start()
    {
        
    }

    public void OnFired(Vector2 screenCenter, List<GameObject> objectPack)
    {
        go = this.gameObject;
        explosionRadiusGO = go.transform.Find("ExplosionRadius").gameObject;
        explosionRadius = explosionRadiusGO.GetComponent<ExplosionRadius>();
        this.objectPack = objectPack;
        this.screenCenter = screenCenter;
        timeFired = Time.time;
        worldSize = Reference.worldController.worldSize;
        leftPlayerCollider = false;
        animationStarted = false;

        rigid_body = go.GetComponent<Rigidbody2D>();
        this.capsuleCollider2D = go.GetComponent<CapsuleCollider2D>();
        capsuleCollider2D.enabled = false;
        rotationalPosition = Reference.playerSpriteController.GetComponent<Rigidbody2D>().rotation;
        rigid_body.rotation = rotationalPosition + 90f;

        Vector3 playerVelocity = Reference.playerSpriteController.velocity;
        rigid_body.velocity = new Vector2(playerVelocity.x, playerVelocity.y);//Reference.playergo.GetComponent<Rigidbody2D>().velocity;
        rigid_body.AddForce(0.1f*transform.right,ForceMode2D.Impulse);
    }
    // Update is called once per frame
    void Update()
    {

        UpdateMotion();
        if(Time.time - timeFired > lifespan)
        {
            //Debug.Log("Projectile Timed Out");
            DestroySelf();
        }
        if(Time.time - timeFired > 0.1f)//Only use the collider if 0.1s has elapsed to avoid interactions with the player
        {
            capsuleCollider2D.enabled = true;
        }
    }
    void FixedUpdate()
    {
        explosionRadius.GetComponent<Rigidbody2D>().position = rigid_body.position;
        
        if(Time.time - timeFired > 0.5f)
        {
            rigid_body.AddForce(transform.right*thrust);

        }
        if (Time.time - timeFired > 0.5f && !animationStarted)
        {
            Vector3 tailLocation = this.gameObject.transform.position -0.15f*this.gameObject.transform.right + 0.01f*this.gameObject.transform.up;
            blueFlameAnimation = Reference.animationController.SpawnBlueFlameAnimation(tailLocation, this.gameObject);
            /// this does nothing yet but may do in the future
            /// blueFlameAnimation.GetComponent<BlueFlameFunction>().TransitionToFullJet();
            animationStarted = true;

        }


    }
    void OnCollisionEnter2D(Collision2D collision)
    {
            DestroySelf();

    }
    void OnTriggerEnter2D(Collider2D collider)//you need both so you can collide with triggering and non triggering objects
    {
        if(Time.time - timeFired > 0.4f)//Only use the collider if 0.1s has elapsed to avoid interactions with the player
        {
            DestroySelf();
        }
    }
    void OnCollisonExit2D(Collision2D collision)
    {
        //Debug.Log("Successfully fired");
        leftPlayerCollider = true;
        //capsuleCollider2D.enabled = true;

    }
    void UpdateMotion()
    {

        //go.transform.position += Quaternion.Euler(0, 0, -90) * transform.up * projectileSpeed * Time.deltaTime; 

        if(rigid_body.position.x - screenCenter.x * worldSize.x > worldSize.x/2)
        {
            rigid_body.position = new Vector2(rigid_body.position.x - worldSize.x, rigid_body.position.y);
        }
        if(rigid_body.position.x - screenCenter.x * worldSize.x < -worldSize.x/2)
        {
            rigid_body.position = new Vector2(rigid_body.position.x + worldSize.x, rigid_body.position.y);
        }
        if(rigid_body.position.y - screenCenter.y * worldSize.y > worldSize.y/2)
        {
            rigid_body.position = new Vector2(rigid_body.position.x, rigid_body.position.y - worldSize.y);
        }
        if(rigid_body.position.y - screenCenter.y * worldSize.y < -worldSize.y/2)
        {
            rigid_body.position = new Vector2(rigid_body.position.x, rigid_body.position.y + worldSize.y);
        }

    }

    void DestroySelf()
    {   /// add thrust to nearby objects due to explosion
        // a helpful note: the prefab for the projectile has a disabled sprite renderer. This shows the radius of explosion

        foreach (Collider2D hitCollider in explosionRadius.TriggerList)
        {
            if (hitCollider.gameObject.GetComponent<Asteroid>() != null)
            {
                Vector3 direction = hitCollider.gameObject.transform.position - this.transform.position;
                float distance = direction.magnitude;

                Debug.Log(distance);

                if (distance < minExplosionRadius){distance = minExplosionRadius;} // to avoid very huge impulses
                float explosionImpulse = explosionSize / distance * distance;

                // this is janky repeating code which can probably be fixed by using derived classes better oh well
                // we will also want to damage nearby asteroids in the radius too
                if(hitCollider.gameObject.GetComponent<MainAsteroid>() != null)
                {
                    MainAsteroid asteroid = hitCollider.gameObject.GetComponent<MainAsteroid>();
                    asteroid.ApplyExplosionImpulse(direction, explosionImpulse);

                }
                else
                {
                    DerivedAsteroid asteroid = hitCollider.gameObject.GetComponent<DerivedAsteroid>();
                    asteroid.ApplyExplosionImpulse(direction, explosionImpulse);
                }
            }

        }
        



        /// perform animations etc
        Reference.animationController.SpawnExplosionAnimation(this.transform.position);
        
        if(blueFlameAnimation != null)
        {
            blueFlameAnimation.GetComponent<BlueFlameFunction>().DestroyAnimationGO();
        }
        Reference.projectileController.DespawnProjectile(go,objectPack);

    }
}
