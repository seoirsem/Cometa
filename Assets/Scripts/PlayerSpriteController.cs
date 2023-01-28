using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    Player player;
    GameObject playergo;
    float rotationRate = 500; 
    float engineForce = 8; 
    float mass = 1;
    Reference reference;
    float rotation = 0;
    public Vector3 velocity = new Vector3(0, 0, 0);
    Vector2 worldEdges;
    float maxSpeed = 10;
    List<GameObject> visualClones;
    float bulletCooldownTimer;
    float rocketCooldownTimer;
    float rocketCooldown = 0.75f;//s
    float bulletCooldown = 0.3f;
    public Rigidbody2D rigid_body;
    

    void Start()
    {
        playergo = Reference.playergo;
        playergo.transform.position = new Vector2(0, -3);
        playergo.transform.rotation = new Quaternion(0, 0, 0, 0);
        player = Reference.worldController.player;
        worldEdges = Reference.worldController.worldSize;
        rigid_body = this.gameObject.GetComponent<Rigidbody2D>();
        rigid_body.mass = mass;
        bulletCooldownTimer = Time.time;
        rocketCooldownTimer = Time.time;
        SpawnVisualClones();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePlayerRotation();
        UpdatePlayerMotion();
        UpdatePlayerShooting();
    }

    void OnCollisionEnter2D(Collision2D collision)
    { // A collision with the ship's hull has occurred! This is game over (for the time being)
        Debug.Log(collision.gameObject.name);
        Reference.worldController.PlayerDead();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name);
    }
    void UpdatePlayerShooting()
    {
        if (Reference.playerInputController.spaceBar)
        {
            if (Time.time - bulletCooldownTimer > bulletCooldown)
            {//space to set variable cooldowns, and noises if on cooldown....etc
                bulletCooldownTimer = Time.time;

                Reference.projectileController.ShootProjectile(player.go.transform.position,playergo.transform.rotation* Quaternion.Euler(0, 0, 90),"Bullet");
            }
        }
        if (Reference.playerInputController.r)
        {
            if (Time.time - rocketCooldownTimer > bulletCooldown)
            {//space to set variable cooldowns, and noises if on cooldown....etc
                rocketCooldownTimer = Time.time;

                Reference.projectileController.ShootProjectile(player.go.transform.position,playergo.transform.rotation* Quaternion.Euler(0, 0, 90),"Rocket");
            }
        }
    }

    void SpawnVisualClones()
    {
        //visualClones = new List<GameObject>(4);
        GameObject go;
        go = Instantiate(Resources.Load("Prefabs/PlayerEdgeVisuals")) as GameObject;
        go.GetComponent<PlayerEdgeVisualsController>().SetSide("up");
        go.transform.parent = this.gameObject.transform;
        //visualClones[0] = go;
        go = Instantiate(Resources.Load("Prefabs/PlayerEdgeVisuals")) as GameObject;
        go.GetComponent<PlayerEdgeVisualsController>().SetSide("down");
        go.transform.parent = this.gameObject.transform;

        //visualClones[1] = go; 
        go = Instantiate(Resources.Load("Prefabs/PlayerEdgeVisuals")) as GameObject;
        go.GetComponent<PlayerEdgeVisualsController>().SetSide("left");
        go.transform.parent = this.gameObject.transform;

        //visualClones[2] = go; 
        go = Instantiate(Resources.Load("Prefabs/PlayerEdgeVisuals")) as GameObject;
        go.GetComponent<PlayerEdgeVisualsController>().SetSide("right");
        go.transform.parent = this.gameObject.transform;
        
        go = Instantiate(Resources.Load("Prefabs/PlayerEdgeVisuals")) as GameObject;
        go.GetComponent<PlayerEdgeVisualsController>().SetSide("upRight");
        go.transform.parent = this.gameObject.transform;
        
        go = Instantiate(Resources.Load("Prefabs/PlayerEdgeVisuals")) as GameObject;
        go.GetComponent<PlayerEdgeVisualsController>().SetSide("downRight");
        go.transform.parent = this.gameObject.transform;

        go = Instantiate(Resources.Load("Prefabs/PlayerEdgeVisuals")) as GameObject;
        go.GetComponent<PlayerEdgeVisualsController>().SetSide("upLeft");
        go.transform.parent = this.gameObject.transform;

        go = Instantiate(Resources.Load("Prefabs/PlayerEdgeVisuals")) as GameObject;
        go.GetComponent<PlayerEdgeVisualsController>().SetSide("downLeft");
        go.transform.parent = this.gameObject.transform;
        //visualClones[3] = go;


    }
    void UpdatePlayerRotation()
    {
        rigid_body.angularVelocity = 0;
        float playerInputRotation = 0;
        if (Reference.playerInputController.leftKey)
        {
            playerInputRotation += 1;
        }
        if (Reference.playerInputController.rightKey)
        {
            playerInputRotation -= 1;
        }
//        float frameRotation = Time.deltaTime *playerInputRotation*rotationRate;
        //float torque = Time.deltaTime *playerInputRotation*rotationRate;
        //rigid_body.AddTorque(torque, ForceMode2D.Impulse);
        float rotation = rigid_body.rotation;
        rigid_body.MoveRotation(rotation + Time.fixedDeltaTime *playerInputRotation*rotationRate);
        //playergo.transform.Rotate(new Vector3( 0, 0,frameRotation));
        //Debug.Log(frameRotation);
    }

    void UpdatePlayerMotion()
    {
        float playerInputImpulse = 0;
        if (Reference.playerInputController.upKey)
        {
            playerInputImpulse += 1;
        }
        if (Reference.playerInputController.downKey)
        {
            playerInputImpulse -= 1;
        }

        
        //Vector3 impulse = transform.up * playerInputImpulse * engineForce / player.mass;
        //velocity += Time.deltaTime*impulse;
        //if(velocity.magnitude > maxSpeed)
        //{
        //    velocity = velocity.normalized * maxSpeed;
        //}
        //playergo.transform.position += velocity * Time.deltaTime;
        Vector2 direction = new Vector2 (gameObject.transform.up.x, gameObject.transform.up.y);
        Vector2 impulse = direction * playerInputImpulse * engineForce;
        rigid_body.AddForce(impulse);




        if (playergo.transform.position.x > worldEdges.x/2)
        {
            playergo.transform.position = new Vector3(-worldEdges.x / 2, playergo.transform.position.y, 0);
        }
        if (playergo.transform.position.x < -worldEdges.x/2)
        {
            playergo.transform.position = new Vector3(worldEdges.x / 2, playergo.transform.position.y, 0);
        } 
        if (playergo.transform.position.y > worldEdges.y/2)
        {
            playergo.transform.position = new Vector3(playergo.transform.position.x, -worldEdges.y / 2, 0);
        } 
        if (playergo.transform.position.y < -worldEdges.y/2)
        {
            playergo.transform.position = new Vector3(playergo.transform.position.x, worldEdges.y / 2, 0);
        }


    }

    public void ApplyExplosionImpulse(Vector3 direction, float magnitude)
    {

    }


//ToDo animations when rockets are firing for main and side jets

}
