using System.Collections;
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
    float thrust = 1f;
    public List<GameObject> objectPack;
    public bool mainProjectile;
    bool leftPlayerCollider = false;
    CapsuleCollider2D capsuleCollider2D;
    Rigidbody2D rigid_body;
    float rotationalPosition;


    void Start()
    {
        
    }

    public void OnFired(Vector2 screenCenter, List<GameObject> objectPack)
    {
        go = this.gameObject;
        this.objectPack = objectPack;
        this.screenCenter = screenCenter;
        timeFired = Time.time;
        worldSize = Reference.worldController.worldSize;
        leftPlayerCollider = false;
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
        if(Time.time - timeFired > 0.5f)
        {
            rigid_body.AddForce(transform.right*thrust);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
            Debug.Log("Collision! - Projectile");
            DestroySelf();

    }
    void OnTriggerEnter2D(Collider2D collider)//you need both so you can collide with triggering and non triggering objects
    {
        if(Time.time - timeFired > 0.4f)//Only use the collider if 0.1s has elapsed to avoid interactions with the player
        {
            Debug.Log("Collisdsion! - Projectile");
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
    {
        Reference.animationController.SpawnExplosionAnimation(this.transform.position);

        Reference.projectileController.DespawnProjectile(go,objectPack);

    }
}
