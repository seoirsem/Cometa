using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    Player player;
    GameObject playergo;
    GameObject blueFlamePrefab;
    GameObject blueFlame;
    GameObject blueFlameLeftgo;
    GameObject blueFlameRightgo;

    float rotationRate = 500; 
    float engineForce = 8; 
    float mass = 15;
    Reference reference;
    float rotation = 0;
    public Vector3 velocity = new Vector3(0, 0, 0);
    Vector2 worldEdges;
    float maxSpeed = 10;
    List<GameObject> visualClones;
    float bulletCooldownTimer;
    float rocketCooldownTimer;
    float rocketCooldown = 2f;//s
    float bulletCooldown = 0.15f;
    public Rigidbody2D rigid_body;
    bool spaceDown = false;
    float rocketForce = 0.5f;

    bool playingRocketSound = false;
    bool playingTurningSound = false;

    BlueFlameFunction blueFlameFunction;
    BlueFlameFunction blueFlameLeft;
    BlueFlameFunction blueFlameRight;




    void Awake()
    {

        rigid_body = this.gameObject.GetComponent<Rigidbody2D>();
        rigid_body.mass = mass;
        spaceDown = false;

        blueFlamePrefab = Resources.Load("Prefabs/Blue_Flame") as GameObject;

        blueFlameLeftgo = this.gameObject.transform.Find("LeftExhaust").gameObject;
        blueFlameRightgo = this.gameObject.transform.Find("RightExhaust").gameObject;

        

        blueFlame = Instantiate(blueFlamePrefab);
        blueFlame.transform.SetParent(this.gameObject.transform);
        blueFlameFunction = blueFlame.GetComponent<BlueFlameFunction>();
        blueFlameLeft = blueFlameLeftgo.GetComponent<BlueFlameFunction>();
        blueFlameRight = blueFlameRightgo.GetComponent<BlueFlameFunction>();
        //blueFlameFunction.StartJet();

    }

    void Start()
    {
        playergo = Reference.playergo;
        playergo.transform.position = new Vector2(0, -3);
        playergo.transform.rotation = new Quaternion(0, 0, 0, 0);
        player = Reference.worldController.player;
        worldEdges = Reference.worldController.worldSize;
        bulletCooldownTimer = Time.time - bulletCooldown;
        rocketCooldownTimer = Time.time - rocketCooldown;
        SpawnVisualClones();
        //blueFlame.SetActive(false);

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
//        Debug.Log(collision.gameObject.name);
        if(collision.gameObject.GetComponent<Projectile>() == null)
        {
            Reference.worldController.PlayerDead();
        }
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
                Reference.soundController.FireBullet();
                Reference.shakeCamera.StartShake(0.05f,0.05f);

            }
        }
        if (Reference.playerInputController.r)
        {
            if (Time.time - rocketCooldownTimer > rocketCooldown)
            {//space to set variable cooldowns, and noises if on cooldown....etc
                rocketCooldownTimer = Time.time;

                Reference.projectileController.ShootProjectile(player.go.transform.position,playergo.transform.rotation* Quaternion.Euler(0, 0, 90),"Rocket");
            }
        }
    }

    public void ApplyRocketLaunchImpulse()
    {
        Vector2 direction = new Vector2 (gameObject.transform.up.x, gameObject.transform.up.y);
        Vector2 impulse = direction * rocketForce * mass;

        rigid_body.AddForce(-1*impulse,ForceMode2D.Impulse);
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
            if(!playingTurningSound)
            {
                Reference.soundController.StartTurningRocketBoost();
                playingTurningSound = true;
                blueFlameRightgo.SetActive(true);
                blueFlameRight.StartJet();

            }
        }
        if (Reference.playerInputController.rightKey)
        {
            playerInputRotation -= 1;
            if(!playingTurningSound)
            {
                Reference.soundController.StartTurningRocketBoost();
                playingTurningSound = true;
                blueFlameLeftgo.SetActive(true);
                blueFlameLeft.StartJet();

            }
        }
        if(playerInputRotation == 0 && playingTurningSound)
        {
            Reference.soundController.StopTurningRocketBoost();
            playingTurningSound = false;
            blueFlameLeftgo.SetActive(false);
            blueFlameRightgo.SetActive(false);

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
            playerInputImpulse += 1*mass;
            if(!playingRocketSound)
            {
                Reference.soundController.StartRocketBoost();
                playingRocketSound = true;
                blueFlame.SetActive(true);
                blueFlameFunction.StartJet();
            }
        }
        if (Reference.playerInputController.downKey)
        {
            playerInputImpulse -= 1*mass;
            if(!playingRocketSound)
            {// todo - make reverse sound different?
                Reference.soundController.StartRocketBoost();
                playingRocketSound = true;
                
            }
        }
        
        if(playerInputImpulse == 0 && playingRocketSound)
        {

            Reference.soundController.StopRocketBoost();
            playingRocketSound = false;
            blueFlameFunction.StopJet();
            blueFlame.SetActive(false);

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
        //Debug.Log(magnitude);
        //Debug.Log(direction);
        rigid_body.AddForce(magnitude*direction,ForceMode2D.Impulse);
    }


//ToDo animations when rockets are firing for main and side jets

}
