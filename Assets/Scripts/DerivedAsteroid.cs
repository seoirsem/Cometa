using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DerivedAsteroid : Asteroid
{

    Vector2 offset;
    public Vector2 location;
    GameObject mainAsteroid;
    MainAsteroid mainAsteroidClass;

    Vector3 thisStepMainPosition;
    float thisStepMainRotation;

    public void OnSpawn(int size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector2 velocity)
    {
        DrawDerivedAsteroid();

        this.mass = Mathf.Pow(size,2);
        // Update this in future to calculate based on the area of the shape formed by the mesh
        this.location = location;
        this.size = size;
        asteroidController = GameObject.Find("AsteroidController").GetComponent<AsteroidController>();
        //asteroidOutlines = this.gameObject.transform.Find("AsteroidOutline").gameObject;
        mainAsteroidClass = mainAsteroid.GetComponent<MainAsteroid>();
        rigid_body = this.GetComponent<Rigidbody2D>();
//        Debug.Log(rigid_body);
        this.asteroidPack = asteroidPack;
        asteroidgo = this.gameObject;
        this.velocity = velocity;
        this.rigid_body.velocity = velocity;
        this.worldSize = Reference.worldController.worldSize;
        this.location = location;
        rigid_body.centerOfMass = new Vector2(0,0);
        PolygonCollider2D polygonCollider = this.gameObject.GetComponent<PolygonCollider2D>();
        offset = new Vector2(location.x * worldSize.x, location.y * worldSize.y);
        CloneAsteroidGeometry(mainAsteroid);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1);
        thisStepMainPosition = mainAsteroidClass.rigid_body.position + offset;
        thisStepMainRotation = mainAsteroidClass.rigid_body.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        this.rigidBodyVelocity = mainAsteroidClass.rigid_body.velocity;
        UpdateAsteroidPosition();
    }


    void UpdateAsteroidPosition()
    { //always one step behind main

        rigid_body.position = thisStepMainPosition;
        thisStepMainPosition = mainAsteroidClass.rigid_body.position + offset;
        rigid_body.rotation = thisStepMainRotation;
        thisStepMainRotation = mainAsteroidClass.rigid_body.rotation;
    }



    public void CloneAsteroidGeometry(GameObject mainAsteroid)
    {
        Asteroid mainAst = mainAsteroid.GetComponent<Asteroid>();
        //Debug.Log(this.squareMesh);
        this.squareMesh.perimeterVertices = mainAst.squareMesh.perimeterVertices;
        this.squareMesh.RedrawMesh();
        //DrawMesh(meshVertices, meshTriangles);
        //DrawCollider(meshVertices, meshTriangles);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D otherObject = collision.collider;
        // Debug.Log("ontriggerenter");
        // This passes note of any collisions straight to the main asteroid
        mainAsteroidClass.DerivedAsteroidCollision(otherObject, this.gameObject, offset, collision);

    }

    public void ApplyExplosionImpulse(Vector3 direction, Vector2 position, float explosionSize, Projectile projectile)
    {// any nearby explosions are simply handed up to the parent
        mainAsteroidClass.ApplyExplosionImpulse(direction, position + offset, explosionSize, projectile);
    }

    public void DerivedAsteroidHitShields(float distance, Vector2 relativeVelocity, Vector2 relativePosition, Vector2 closestPoint, float shieldDamage)
    {
        mainAsteroidClass.MainAsteroidHitShields(distance, relativeVelocity, relativePosition, closestPoint - offset, shieldDamage);
    }
}
