using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector2 screenCenter;// (0,-1,1 in each of x and y to determine which of the 5 screens it is on)
    GameObject go;
    public float timeFired;
    float lifespan = 2;//s
    Vector2 worldSize;
    float projectileSpeed = 11;
    float explosionSize = 0f; // to modulate how much explosion thrust is applied to surrounding objects
    float minExplosionRadius = 0.25f; //to avoid huge impulses near the explosion 
    float thrust = 1f;
    public List<GameObject> objectPack;
    public bool mainProjectile;
    bool leftPlayerCollider = false;
    CapsuleCollider2D capsuleCollider2D;
    float rotationalPosition;
    GameObject explosionRadiusGO;
    public Vector2 velocity; // I need this so that I know the velocity of the projectile at collision; rb2D goes to 0 at despawn
    CircleCollider2D circleCollider2d; 
    Rigidbody2D rigid_body;
    ExplosionRadius explosionRadius;
    public string projectileType;


    bool animationStarted = false;
    GameObject blueFlameAnimation;

    void Start()
    {
        
    }

    public void OnFired(Vector2 screenCenter, List<GameObject> objectPack, string projectileType)
    {
        this.projectileType = projectileType;
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
        if(projectileType == "Rocket")
        {
            this.capsuleCollider2D = go.GetComponent<CapsuleCollider2D>();
            capsuleCollider2D.enabled = false;
        }
        else if(projectileType == "Bullet")
        {
            this.circleCollider2d = go.GetComponent<CircleCollider2D>();
            circleCollider2d.enabled = false;
        }
        rotationalPosition = Reference.playerSpriteController.GetComponent<Rigidbody2D>().rotation;
        rigid_body.rotation = rotationalPosition + 90f;

        Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(),Reference.playergo.GetComponent<Collider2D>(), true);


//        Vector3 playerVelocity = Reference.playerSpriteController.velocity;

        rigid_body.velocity = Reference.playerSpriteController.rigid_body.velocity;
        
        if(projectileType == "Rocket")
        {
            //Debug.Log("Rocket fired");
            rigid_body.AddForce(0.1f*transform.right,ForceMode2D.Impulse);
        }
        if(projectileType == "Bullet")
        {
            rigid_body.AddForce(0.4f*transform.right,ForceMode2D.Impulse);
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.velocity = this.rigid_body.velocity;
        UpdateMotion();
        if(Time.time - timeFired > lifespan)
        {
            DestroySelf();
        }
        if(Time.time - timeFired > 0.25f)//Only use the collider if 0.1s has elapsed to avoid interactions with the player
        {
            if(projectileType == "Rocket")
            {
                capsuleCollider2D.enabled = true;
            }
            else if(projectileType == "Bullet")
            {
                circleCollider2d.enabled = true;
            }
        }
    }
    void FixedUpdate()
    {
        explosionRadius.GetComponent<Rigidbody2D>().position = rigid_body.position;
        
        if(projectileType == "Rocket")
        {
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
                Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(),Reference.playergo.GetComponent<Collider2D>(), false);
            }
        }
        else if(projectileType == "Bullet")
        {
            // animation to make it pulse slightly?
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
            Vector3 direction = hitCollider.gameObject.transform.position - this.transform.position;
            float distance = direction.magnitude;            
            if (distance < minExplosionRadius){distance = minExplosionRadius;} // to avoid very huge impulses
            float explosionImpulse = 0f;
            if(projectileType == "Rocket")
            {
                explosionImpulse = explosionSize / distance * distance;
            }
            else if(projectileType == "Bullet")
            {
                explosionImpulse = explosionSize / (distance * distance * 10f);
            }


            if (hitCollider.gameObject.GetComponent<Asteroid>() != null)
            {
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
            else if (hitCollider.gameObject.GetComponent<PlayerSpriteController>() != null)
            {
                hitCollider.gameObject.GetComponent<PlayerSpriteController>().ApplyExplosionImpulse(direction,explosionImpulse);
            }

        }
        


        if(projectileType == "Rocket")
        {
            Debug.Log(projectileType);
            /// perform animations etc
            Reference.animationController.SpawnExplosionAnimation(this.transform.position);
            
            if(blueFlameAnimation != null)
            {
                blueFlameAnimation.GetComponent<BlueFlameFunction>().DestroyAnimationGO();
            }
        }
        Reference.projectileController.DespawnProjectile(go,objectPack);

    }
}
