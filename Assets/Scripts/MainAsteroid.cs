using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAsteroid : Asteroid
{
    // Start is called before the first frame update
    public void OnSpawn(int size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector2 velocity)
    {
        this.mass = Mathf.Pow(size,2);
        // Update this in future to calculate based on the area of the shape formed by the mesh
        //Debug.Log(this.mass);

        this.size = size;
        asteroidController = GameObject.Find("AsteroidController").GetComponent<AsteroidController>();
        asteroidOutlines = this.gameObject.transform.Find("AsteroidOutline").gameObject;
        rigid_body = this.GetComponent<Rigidbody2D>();
        this.asteroidPack = asteroidPack;
        asteroidgo = this.gameObject;
        this.velocity = velocity;
        this.rigid_body.velocity = velocity;
        this.worldSize = Reference.worldController.worldSize;
        this.location = location;
        rigid_body.centerOfMass = new Vector2(0,0);


        //in Unity trianges are drawn clockwise
        this.rotationRate = Mathf.Pow(Random.Range(-1f, 1f),2f) * 250;//random rotation rate
        this.rigid_body.angularVelocity = rotationRate;
        DrawAsteroid(size);


    }


    // Update is called once per frame
    void Update()
    {
        UpdateAsteroidPosition();
        UpdateAsteroidRotation();
    }

    void UpdateAsteroidPosition()
    {
        //Vector2 velocity2d = new Vector2(velocity.x, velocity.y);
        //this.rigid_body.position += velocity2d * Time.deltaTime;
        //Debug.Log(velocity.magnitude);

        if (rigid_body.position.x - location.x * worldSize.x > worldSize.x / 2)
        {
            this.rigid_body.position = new Vector2(rigid_body.position.x - worldSize.x, rigid_body.position.y);// asteroidgo.transform.position.z);
        }
        if (rigid_body.position.x - location.x * worldSize.x < -worldSize.x / 2)
        {
            this.rigid_body.position  = new Vector2(rigid_body.position.x + worldSize.x, rigid_body.position.y);
        }
        if (rigid_body.position.y - location.y * worldSize.y > worldSize.y / 2)
        {
            this.rigid_body.position  = new Vector2(rigid_body.position.x, rigid_body.position.y - worldSize.y);
        }
        if (rigid_body.position.y - location.y * worldSize.y < -worldSize.y / 2)
        {
            this.rigid_body.position  = new Vector2(rigid_body.position.x, rigid_body.position.y + worldSize.y);
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
        Debug.Log("OnCollisionEnter2D - asteroid");
        if (collision.gameObject.GetComponent<Projectile>() != null) 
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            if (projectile.mainProjectile == true)
            {
                Reference.scoreController.IncrementScore((float)size);
                Reference.asteroidController.AsteroidHit(this, collision, asteroidPack);
            }
        }
        else if (collision.gameObject.GetComponent<PlayerSpriteController>() != null)
        {
            Debug.Log("Asteroid hit player");
        }
        else
        {//if you have collided with another asteroid
            Asteroid otherAsteroid = collision.gameObject.GetComponent<Asteroid>();
            asteroidController.AsteroidAstroidCollision(this, collision, asteroidPack);
            Debug.Log("Asteroid hit other asteroid");

        }
    }

}
