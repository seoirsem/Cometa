using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAsteroid : Asteroid
{
    bool spawning = false; //this is used to mark when this asteroid is spawning in

    public void OnSpawnSplitAsteroid(float size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector2 velocity, Asteroid asteroidData)
    {

        this.mass = Mathf.Pow(size,2);
        this.size = asteroidData.size;
        Debug.Log(asteroidData.size);
        this.asteroidOutlines = this.gameObject.transform.Find("AsteroidOutline").gameObject;
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

        this.meshVertices = asteroidData.meshVertices;
        this.meshTriangles = asteroidData.meshTriangles;
        this.meshIndices = asteroidData.meshIndices;
        DrawMesh(meshVertices, meshTriangles);
        DrawCollider(meshVertices, meshTriangles);
    }

    public void OnSpawn(float size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector2 velocity, bool spawning = false)
    {
        this.spawning = spawning;

        if (spawning)
        {
            this.gameObject.layer = LayerMask.NameToLayer("SpawningAsteroid");
            this.transform.position = this.transform.position + new Vector3(0,0,10);
        }


        this.mass = Mathf.Pow(size,2);
        // this.size = size;
        this.asteroidController = GameObject.Find("AsteroidController").GetComponent<AsteroidController>();
        this.asteroidOutlines = this.gameObject.transform.Find("AsteroidOutline").gameObject;
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

        DrawAsteroid(8f);

        this.size = GetPolygonArea(new List<Vector3>(this.meshVertices));
    }


    // Update is called once per frame
    void Update()
    {
        UpdateAsteroidPosition();
        UpdateAsteroidRotation();
    }

    void NewAsteroidEnteredScreen()
    {
        if(this.spawning == true)
        {
            Debug.Log("New asteroid changing screen");
            this.gameObject.layer = LayerMask.NameToLayer("Default");
            this.transform.position = this.transform.position + new Vector3(0,0,-10);
            this.spawning = false;
        }
    }
    void UpdateAsteroidPosition()
    {
        //Vector2 velocity2d = new Vector2(velocity.x, velocity.y);
        //this.rigid_body.position += velocity2d * Time.deltaTime;
        //Debug.Log(velocity.magnitude);

        if (rigid_body.position.x - location.x * worldSize.x > worldSize.x / 2)
        {
            this.rigid_body.position = new Vector2(rigid_body.position.x - worldSize.x, rigid_body.position.y);// asteroidgo.transform.position.z);
            NewAsteroidEnteredScreen();
        }
        if (rigid_body.position.x - location.x * worldSize.x < -worldSize.x / 2)
        {
            this.rigid_body.position  = new Vector2(rigid_body.position.x + worldSize.x, rigid_body.position.y);
            NewAsteroidEnteredScreen();
        }
        if (rigid_body.position.y - location.y * worldSize.y > worldSize.y / 2)
        {
            this.rigid_body.position  = new Vector2(rigid_body.position.x, rigid_body.position.y - worldSize.y);
            NewAsteroidEnteredScreen();
        }
        if (rigid_body.position.y - location.y * worldSize.y < -worldSize.y / 2)
        {
            this.rigid_body.position  = new Vector2(rigid_body.position.x, rigid_body.position.y + worldSize.y);
            NewAsteroidEnteredScreen(); 
        }
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

    public void DerivedAsteroidCollision(Collider2D collidee, GameObject collidingAsteroid, Vector2 offset)
    {
        //Debug.Log("derived collision");
        if (collidee.gameObject.GetComponent<Projectile>() != null)
        {
            // Debug.Log("derived projectile collision");

            ResolveCollision(collidee.gameObject, null, collidee, offset);
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
                Reference.asteroidController.AsteroidHit(this, collisionLocation, otherObject, asteroidPack);
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
    }
    public void ApplyExplosionImpulse(Vector3 direction, float explosionImpulse)
    {
        ApplyImpulse(direction, explosionImpulse);
    }

}
