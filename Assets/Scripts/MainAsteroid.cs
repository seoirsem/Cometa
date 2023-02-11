using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAsteroid : Asteroid
{
    bool spawning = false; //this is used to mark when this asteroid is spawning in
    bool halfSpawning = false;
    Vector2 spawnDirection;
    bool callMoveSpawningAsteroid = false;

    int waitFrames;

    public Dictionary<Vector2,GameObject> derivedAsteroids;
    BackgroundCollider backgroundCollider;
    List<GameObject> derivedOnScreen;
    List<GameObject> derivedOffScreen;
    float timeAlive;

    float maximumVelocity = 20f;

    public void SetSpawningDerivedAsteroid()
    {
        if(spawnDirection == new Vector2(1,0) || spawnDirection == new Vector2(0,1))
        {
            derivedAsteroids[spawnDirection].gameObject.layer = LayerMask.NameToLayer("DerivedAsteroids");
        }
        else
        {
            this.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void OnSpawn(int size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector2 velocity, float rotationRate, SquareMesh squareMesh, bool spawning, Vector2 positionOrientation)
    {
        this.timeAlive = Time.time;

        waitFrames = 0;
        this.spawnDirection = positionOrientation;
        this.spawning = spawning;
        backgroundCollider = GameObject.Find("Background").GetComponent<BackgroundCollider>();

        if (spawning)
        {
//            this.gameObject.layer = LayerMask.NameToLayer("SpawningAsteroid");
            this.gameObject.layer = LayerMask.NameToLayer("Default");
//            this.transform.position = new Vector3(transform.position.x,transform.position.y,10);
            this.transform.position = new Vector3(transform.position.x,transform.position.y,-1);

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
                    asteroid.layer = LayerMask.NameToLayer("NoInteractions");
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
        else
        {
            foreach(GameObject asteroid in asteroidPack)
            {
                if(asteroid != this.gameObject)
                {
                    asteroid.layer = LayerMask.NameToLayer("DerivedAsteroids");
                }
                
            }
        }
        //this.mass = Mathf.Pow(size,2);
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
        // this.rotationRate = Mathf.Pow(Random.Range(-1f, 1f),2f) * 0;//random rotation rate
        this.rigid_body.angularVelocity = rotationRate;
        this.rigid_body.mass = mass;
        this.rigidBodyVelocity = rigid_body.velocity;

        

        float timeAlive; // set in asteroid controller on spawn

        DrawAsteroid(size,squareMesh);

        //this.size = GetPolygonArea(new List<Vector3>(this.meshVertices));
    }


    // Update is called once per frame
    void Update()
    {
        this.rigidBodyVelocity = rigid_body.velocity;


        if(rigid_body.velocity.magnitude > maximumVelocity)
        {
            rigid_body.velocity = rigid_body.velocity.normalized * maximumVelocity;
        }
  
        if(callMoveSpawningAsteroid)
        {
            MoveSpawningAsteroid();
        }
        if(spawning)
        {
            if(CheckIfFullyOnScreen())
            {// asteroid is now fully spawned
                spawning = false;
//                Debug.Log("Fully on");
                foreach(KeyValuePair<Vector2,GameObject> derivedAsteroid in derivedAsteroids)
                {
                    derivedAsteroid.Value.SetActive(true);
                    derivedAsteroid.Value.layer = LayerMask.NameToLayer("DerivedAsteroids");
                    derivedAsteroid.Value.gameObject.transform.position = new Vector3(
                    derivedAsteroid.Value.gameObject.transform.position.x,derivedAsteroid.Value.gameObject.transform.position.y,-1);
                }   
            }
            if(CheckIfFullyOffScreen())
            {//if it gets nocked offscreen again after having half spawned
                spawning = true;
                halfSpawning = false;
                asteroidController.DespawnAsteroid(this,asteroidPack);
            }


        }
        derivedOffScreen = DerivedAsteroidsOffScreen();
        derivedOnScreen = DerivedAsteroidsOnScreen();
        if(!spawning)
        {
            UpdateAsteroidPosition();
            UpdateAsteroidRotation();
        }
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
        Vector2 velocity = rigid_body.velocity;

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
        rigid_body.velocity = velocity;
    }

    bool CheckIfFullyOnScreen()
    {// says whether the central asteroid is fully on the screen
        bool fullyOnScreen = false;
        if((this.rigid_body.position.x > -worldSize.x/2 && this.rigid_body.position.x < worldSize.x/2 - Asteroid.celSize*size) && (this.rigid_body.position.y > -worldSize.y/2 && this.rigid_body.position.y < worldSize.y/2 - Asteroid.celSize*size))
        {
            fullyOnScreen = true;
        }
        return fullyOnScreen;
    }

    bool CheckIfFullyOffScreen()
    {// says whether the central asteroid is fully off the screen
        bool fullyOffScreen = false;
        float edgeSpace = 1.05f;
        if((this.rigid_body.position.x < -edgeSpace*worldSize.x/2 || this.rigid_body.position.x > edgeSpace*worldSize.x/2 - Asteroid.celSize*size) && (this.rigid_body.position.y < -edgeSpace*worldSize.y/2 || this.rigid_body.position.y > edgeSpace*worldSize.y/2 - Asteroid.celSize*size))
        {
            Debug.Log("fully off screen");
            fullyOffScreen = true;
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
//        Debug.Log(collision.gameObject.name);
        ResolveCollision(collision.gameObject, collision, null, new Vector2(0,0));
    }

    public void DerivedAsteroidCollision(Collider2D collidee, GameObject collidingAsteroid, Vector2 offset, Collision2D collision)
    {
        //Debug.Log("derived collision");
        //Debug.Break();
        if (collidee.gameObject.GetComponent<Projectile>() != null)
        {
//            Debug.Log("derived projectile collision");

            ResolveCollision(collidee.gameObject, collision, collidee, offset);
        }
    }


    void ResolveCollision(GameObject otherObject, Collision2D collision, Collider2D collider, Vector2 offset)
    {
        int numberOfSquaresInAsteroid = squareMesh.NumberOfSquaresInMesh();
        Vector2 collisionLocation;

        if(collision == null)
        {
            collisionLocation = collider.ClosestPoint(otherObject.transform.position) - offset;
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
                float radius = projectile.explosionRadiusDiameter;
                //Reference.scoreController.IncrementScore((float)size);

                List<SquareMesh> newAstroidMeshes = this.squareMesh.RemoveSquaresInRadius(otherObject.transform.position - new Vector3(offset.x,offset.y,0), radius);
                if(newAstroidMeshes != null)
                {

                    /// code to tell asteroid controller to destroy theis mesh and spawn multiple new ones
                    Reference.asteroidController.AsteroidHit(this, collisionLocation, otherObject, asteroidPack, newAstroidMeshes,numberOfSquaresInAsteroid,offset);
                }
            }
        }
        else if (otherObject.GetComponent<PlayerSpriteController>() != null)
        {
            //Debug.Log("Asteroid hit player");
        }
        else if(otherObject.GetComponent<Asteroid>() != null)
        {//if you have collided with another asteroid
        //REMEMBER TO ALWAYS LOOK FOR MAIN ASTEROID RIGID BODY VELOCITY
            Asteroid otherAsteroid = otherObject.GetComponent<Asteroid>();
            Reference.asteroidController.AsteroidAstroidCollision(this, collisionLocation, asteroidPack);
            Vector2 relativeVelocity = rigidBodyVelocity - otherAsteroid.rigidBodyVelocity;
            Vector2 relativeMomentum = rigidBodyVelocity*mass - otherAsteroid.rigidBodyVelocity*otherAsteroid.mass;
            //Debug.Log(relativeMomentum);
            //Debug.Log($"Relative {relativeVelocity.magnitude}, size {mass}");
            if(relativeMomentum.magnitude > 200f)
            {
                float radius = relativeMomentum.magnitude/450f;
                Debug.Log($"Mass: {mass}, Other Object mass: {otherAsteroid.mass} Relative momentum: {relativeMomentum.magnitude}, radius: {radius}");
                if(otherAsteroid.mass == 1600){Debug.Log(otherAsteroid.gameObject.name); Debug.Break();}
                List<SquareMesh> newAstroidMeshes = this.squareMesh.RemoveSquaresInRadius(collision.contacts[0].point, radius);
                if(newAstroidMeshes != null)
                {
                    /// code to tell asteroid controller to destroy theis mesh and spawn multiple new ones
                    Reference.asteroidController.AsteroidHit(this, collisionLocation, otherObject, asteroidPack, newAstroidMeshes,numberOfSquaresInAsteroid,offset);
                }

            }
            else if(mass < 3 && relativeVelocity.magnitude > 3f && otherAsteroid.mass > 10)
            {//chance to be destroyed if it is a small asteroid
                // This is an attempt at crowd control
                float random = Random.Range(0,1);
                if(random<0.2f)
                {
                    asteroidController.DespawnAsteroid(this,asteroidPack);
                }
            }
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

    public void ApplyExplosionImpulse(Vector3 direction, Vector2 position, float explosionImpulse, Projectile projectile)
    {
        ApplyImpulse(direction, explosionImpulse);
        ApplyExplosion(position, projectile);
    }

    public void ApplyExplosion(Vector2 position, Projectile projectile)
    {

        int numberOfSquaresInAsteroid = squareMesh.NumberOfSquaresInMesh();
        if (projectile.mainProjectile == true)
        {
            float radius = projectile.explosionRadiusDiameter;
            //Reference.scoreController.IncrementScore((float)size);
            List<SquareMesh> newAstroidMeshes = this.squareMesh.RemoveSquaresInRadius(position, radius);
            if(newAstroidMeshes != null)
            {
                /// code to tell asteroid controller to destroy theis mesh and spawn multiple new ones
                Reference.asteroidController.AsteroidHit(this, position, projectile.gameObject, asteroidPack, newAstroidMeshes,numberOfSquaresInAsteroid,new Vector2(0,0));
            }
        }
    }

    public void ApplyFlames(Vector2 position, float radius, GameObject gameObject)
    {
        int numberOfSquaresInAsteroid = squareMesh.NumberOfSquaresInMesh();

        List<SquareMesh> newAstroidMeshes = this.squareMesh.RemoveSquaresInRadius(position, radius);
        if(newAstroidMeshes != null)
        {
            /// code to tell asteroid controller to destroy theis mesh and spawn multiple new ones
            Reference.asteroidController.AsteroidHit(this, position, gameObject, asteroidPack, newAstroidMeshes,numberOfSquaresInAsteroid,new Vector2(0,0));
        }
       
    }

    public void HitByProjectile()
    {



    }
    public void MainAsteroidHitShields(float distance, Vector2 relativeVelocity, Vector2 relativePosition, Vector2 closestPoint, float shieldDamage)
    {
            
        if(relativeVelocity.magnitude > 4f)
        {
            int numberOfSquaresInAsteroid = this.squareMesh.NumberOfSquaresInMesh();
            List<SquareMesh> newAstroidMeshes = this.squareMesh.RemoveSquaresInRadius(closestPoint,shieldDamage);
            if(newAstroidMeshes != null)
            {
                Reference.asteroidController.AsteroidHit(this, closestPoint, Reference.shipShields.gameObject, asteroidPack, newAstroidMeshes,numberOfSquaresInAsteroid,new Vector2(0,0));
            }
        }
    }

}
