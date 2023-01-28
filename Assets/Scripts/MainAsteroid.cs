﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAsteroid : Asteroid
{
    bool spawning = false; //this is used to mark when this asteroid is spawning in
    bool halfSpawning = false;
    bool callMoveSpawningAsteroid = false;

    public Dictionary<Vector2,GameObject> derivedAsteroids;
    BackgroundCollider backgroundCollider;
    List<GameObject> derivedOnScreen;
    List<GameObject> derivedOffScreen;


    public void OnSpawn(int size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector2 velocity, SquareMesh squareMesh, bool spawning = false)
    {
        this.spawning = spawning;
        backgroundCollider = GameObject.Find("Background").GetComponent<BackgroundCollider>();

        if (spawning)
        {
            this.gameObject.layer = LayerMask.NameToLayer("SpawningAsteroid");
            this.transform.position = new Vector3(transform.position.x,transform.position.y,10);
        }
        else
        {
            this.gameObject.layer = LayerMask.NameToLayer("Default");
            this.transform.position = new Vector3(transform.position.x,transform.position.y,-1);
        }

        if(spawning)
        {
            foreach(GameObject asteroid in asteroidPack)
            {
                if(asteroid != this.gameObject)
                {
                    asteroid.layer = LayerMask.NameToLayer("SpawningDerivedAsteroid");
                }
            }
            foreach(KeyValuePair<Vector2,GameObject> kvp in derivedAsteroids)
            {
                Debug.Log(kvp.Key);
            }
            //Vector2 velocityNorm = new Vector2(Mathf.RoundToInt(velocity.normalized.x),Mathf.RoundToInt(velocity.normalized.y));
            //Debug.Log(velocityNorm);
            //derivedAsteroids[-1*velocityNorm].gameObject.layer = LayerMask.NameToLayer("DerivedAsteroid");
        }
        this.mass = Mathf.Pow(size,2);
        this.size = size;
        this.asteroidController = GameObject.Find("AsteroidController").GetComponent<AsteroidController>();
        //this.asteroidOutlines = this.gameObject.transform.Find("AsteroidOutline").gameObject;
        this.rigid_body = this.GetComponent<Rigidbody2D>();
        this.asteroidPack = asteroidPack;
        this.asteroidgo = this.gameObject;
        this.velocity = velocity;
        this.rigid_body.velocity = velocity;
        this.worldSize = Reference.worldController.worldSize;
        this.location = location;
        this.rigid_body.centerOfMass = new Vector2(0,0);
        this.rotationRate = Mathf.Pow(Random.Range(-1f, 1f),2f) * 0;//random rotation rate
        this.rigid_body.angularVelocity = rotationRate;
        this.rigid_body.mass = mass;

        DrawAsteroid(size,squareMesh);

        //this.size = GetPolygonArea(new List<Vector3>(this.meshVertices));
    }


    // Update is called once per frame
    void Update()
    {
        if(callMoveSpawningAsteroid)
        {
            MoveSpawningAsteroid();
        }
        if(halfSpawning)
        {
            if(CheckIfFullyOnScreen())
            {// asteroid is now fully spawned
                halfSpawning = false;
                Debug.Log("Fully on");
                foreach(KeyValuePair<Vector2,GameObject> derivedAsteroid in derivedAsteroids)
                {
                    derivedAsteroid.Value.layer = LayerMask.NameToLayer("DerivedAsteroids");
                    derivedAsteroid.Value.gameObject.transform.position = new Vector3(
                    derivedAsteroid.Value.gameObject.transform.position.x,derivedAsteroid.Value.gameObject.transform.position.y,-1);
                }   
            }
            if(CheckIfFullyOffScreen())
            {//if it gets nocked offscreen again after having half spawned
                spawning = true;
                halfSpawning = false;
            }

        }
        derivedOffScreen = DerivedAsteroidsOffScreen();
        derivedOnScreen = DerivedAsteroidsOnScreen();
        UpdateAsteroidPosition();
        UpdateAsteroidRotation();
        
    }

    void NewAsteroidEnteredScreen(Vector2 side)
    {
        if(this.spawning == true)
        {
            GameObject derived = derivedAsteroids[side];
            derived.layer = LayerMask.NameToLayer("SpawningAsteroid");
            derived.transform.position = new Vector3(derived.transform.position.x,derived.transform.position.y,10f);
        }
    }
    void MoveSpawningAsteroid()
    {
        if(this.spawning == true)
        {
            callMoveSpawningAsteroid = false;
            //Debug.Break();
            gameObject.layer = LayerMask.NameToLayer("Default");
            transform.position = new Vector3(transform.position.x,transform.position.y,-1f);
            spawning = false;
            halfSpawning = true;
//            Debug.Log("Turtling");
            //Debug.Break();

        }
    }

    void UpdateAsteroidPosition()
    {
        //Vector2 velocity2d = new Vector2(velocity.x, velocity.y);
        //this.rigid_body.position += velocity2d * Time.deltaTime;
        //Debug.Log(velocity.magnitude);


        if (rigid_body.position.x - location.x * worldSize.x > worldSize.x / 2)
        {
            NewAsteroidEnteredScreen(new Vector2(1,0));
            this.rigid_body.position = new Vector2(rigid_body.position.x - worldSize.x, rigid_body.position.y);// asteroidgo.transform.position.z);
            callMoveSpawningAsteroid = true;
        }
        if (rigid_body.position.x - location.x * worldSize.x < -worldSize.x / 2)
        {
            NewAsteroidEnteredScreen(new Vector2(-1,0));
            this.rigid_body.position  = new Vector2(rigid_body.position.x + worldSize.x, rigid_body.position.y);
            callMoveSpawningAsteroid = true;
        }
        if (rigid_body.position.y - location.y * worldSize.y > worldSize.y / 2)
        {
            NewAsteroidEnteredScreen(new Vector2(0,1));
            this.rigid_body.position  = new Vector2(rigid_body.position.x, rigid_body.position.y - worldSize.y);
            callMoveSpawningAsteroid = true;
            
        }
        if (rigid_body.position.y - location.y * worldSize.y < -worldSize.y / 2)
        {
            NewAsteroidEnteredScreen(new Vector2(0,-1));
            this.rigid_body.position  = new Vector2(rigid_body.position.x, rigid_body.position.y + worldSize.y);
            callMoveSpawningAsteroid = true;
        }
    }

    bool CheckIfFullyOnScreen()
    {// says whether the central asteroid is fully on the screen
        bool fullyOnScreen = true;
        foreach(KeyValuePair<Vector2,GameObject> derivedAsteroid in derivedAsteroids)
        {
            if(backgroundCollider.TriggerList.Contains(derivedAsteroid.Value))
            {
                fullyOnScreen = false;
            }
        }
        return fullyOnScreen;
    }

    bool CheckIfFullyOffScreen()
    {// says whether the central asteroid is fully off the screen
        bool fullyOffScreen = true;
        if(backgroundCollider.TriggerList.Contains(this.gameObject))
        {
            fullyOffScreen = false;
        }
        
        return fullyOffScreen;
    }

    List<GameObject> DerivedAsteroidsOnScreen()
    { // list of all derived asteroids on screen right now
        List<GameObject> fullyOnScreen = new List<GameObject>();
        foreach(KeyValuePair<Vector2,GameObject> derivedAsteroid in derivedAsteroids)
        {
            if(backgroundCollider.TriggerList.Contains(derivedAsteroid.Value))
            {
                fullyOnScreen.Add(derivedAsteroid.Value);
            }
        }

        return fullyOnScreen;
    }

    List<GameObject> DerivedAsteroidsOffScreen()
    { // list of all derived asteroids on screen right now
        List<GameObject> fullyOffScreen = new List<GameObject>();
        foreach(KeyValuePair<Vector2,GameObject> derivedAsteroid in derivedAsteroids)
        {
            if(!backgroundCollider.TriggerList.Contains(derivedAsteroid.Value))
            {
                fullyOffScreen.Add(derivedAsteroid.Value);
            }
        }

        return fullyOffScreen;
    }
    void UpdateAsteroidRotation()
    {
        //float frameRotation = Time.deltaTime * rotationRate;
        //rigid_body.rotation += frameRotation;
        //this.gameObject.transform.Rotate(new Vector3(0, 0, frameRotation));
    }

    public void ApplyImpulse(Vector3 direction, float magnitude)
    {
        rigid_body.AddForce(magnitude*direction,ForceMode2D.Impulse);
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        ResolveCollision(collision.gameObject, collision, null, new Vector2(0,0));
    }

    public void DerivedAsteroidCollision(Collider2D collidee, GameObject collidingAsteroid, Vector2 offset, Collision2D collision)
    {
        //Debug.Log("derived collision");
        if (collidee.gameObject.GetComponent<Projectile>() != null)
        {
            // Debug.Log("derived projectile collision");

            ResolveCollision(collidee.gameObject, collision, collidee, offset);
        }
    }


    void ResolveCollision(GameObject otherObject, Collision2D collision, Collider2D collider, Vector2 offset)
    {

        Vector2 collisionLocation;

        if(collision == null)
        {
            collisionLocation = collider.ClosestPoint(otherObject.transform.position);// - offset;
        }
        else
        {
            collisionLocation = collision.contacts[0].point;
        }


        if (otherObject.GetComponent<Projectile>() != null) 
        {
            Projectile projectile = otherObject.GetComponent<Projectile>();
            if (projectile.mainProjectile == true)
            {
                Reference.scoreController.IncrementScore((float)size);
 
                List<SquareMesh> newAstroidMeshes = this.squareMesh.RemoveSquaresInRadius(otherObject.transform.position, 1f);
                if(newAstroidMeshes != null)
                {
                    /// code to tell asteroid controller to destroy theis mesh and spawn multiple new ones
                    Reference.asteroidController.AsteroidHit(this, collisionLocation, otherObject, asteroidPack, newAstroidMeshes,offset);

                }
            }
        }
        else if (otherObject.GetComponent<PlayerSpriteController>() != null)
        {
            //Debug.Log("Asteroid hit player");
        }
        else
        {//if you have collided with another asteroid
            Asteroid otherAsteroid = otherObject.GetComponent<Asteroid>();
            Reference.asteroidController.AsteroidAstroidCollision(this, collisionLocation, asteroidPack);
            //Debug.Log("Asteroid hit other asteroid");

        }

        foreach(GameObject asteroidgo in asteroidPack)
        {
            if(asteroidgo.GetComponent<DerivedAsteroid>() != null)
            {
                asteroidgo.GetComponent<DerivedAsteroid>().CloneAsteroidGeometry(this.gameObject);
            }
        }
    }



    public void ApplyExplosionImpulse(Vector3 direction, float explosionImpulse)
    {
        ApplyImpulse(direction, explosionImpulse);
    }

    public void HitByProjectile()
    {



    }

}
