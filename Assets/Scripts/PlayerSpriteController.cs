﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    Player player;
    GameObject playergo;
    GameObject blueFlame;
    GameObject blueFlameLeftgo;
    GameObject blueFlameRightgo;
    GameObject blueFlameReverseLeft;
    GameObject blueFlameReverseRight;

    ShipShields shipShields;

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

    float boostCooldown = 2f;
    float lastBoost;

    BlueFlameFunction blueFlameFunction;
    BlueFlameFunction blueFlameLeft;
    BlueFlameFunction blueFlameRight;




    void Awake()
    {

        rigid_body = this.gameObject.GetComponent<Rigidbody2D>();
        rigid_body.mass = mass;
        spaceDown = false;
        blueFlameLeftgo = this.gameObject.transform.Find("LeftExhaust").gameObject;
        blueFlameRightgo = this.gameObject.transform.Find("RightExhaust").gameObject;
        blueFlame = this.gameObject.transform.Find("Blue_Flame").gameObject;
        blueFlameReverseLeft = GameObject.Find("LeftReverse");
        blueFlameReverseRight = GameObject.Find("RightReverse");
        blueFlameFunction = blueFlame.GetComponent<BlueFlameFunction>();
        blueFlameLeft = blueFlameLeftgo.GetComponent<BlueFlameFunction>();
        blueFlameRight = blueFlameRightgo.GetComponent<BlueFlameFunction>();
        //blueFlameFunction.StartJet();
        shipShields = this.gameObject.transform.Find("ShipShields").GetComponent<ShipShields>();
        lastBoost = Time.time - boostCooldown;
    }

    void Start()
    {
        playergo = Reference.playergo;
        playergo.transform.position = new Vector2(0, -7);
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
        if(collision.gameObject.GetComponent<Projectile>() == null && shipShields.shieldStrength <= 0)
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

 ////////////// KEYBOARD CONTROLS ////////////////

        if (Reference.playerInputController.spaceBar)
        {
            if (Time.time - bulletCooldownTimer > bulletCooldown)
            {//space to set variable cooldowns, and noises if on cooldown....etc
                bulletCooldownTimer = Time.time;
                Reference.projectileController.ShootProjectile(player.go.transform.position, playergo.transform.rotation * Quaternion.Euler(0, 0, 90),"Bullet");
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

///////////// MOUSE CONTROLS //////////////////

        if (Reference.playerInputController.mouseClickDown)
        {
            Vector2 cursorPosition = Reference.mainCamera.ScreenToWorldPoint(Reference.playerInputController.cursorPosition);
            if (Time.time - bulletCooldownTimer > bulletCooldown)
            {//space to set variable cooldowns, and noises if on cooldown....etc
                bulletCooldownTimer = Time.time;
                float angle = -1*FindDegree(Reference.playergo.GetComponent<Rigidbody2D>().position,cursorPosition) + 90; // I have to hack this angle, why??
                //Debug.Log($"Player: {Reference.playergo.GetComponent<Rigidbody2D>().position}, Cursor: {cursorPosition}, Angle: {angle}");
//                Debug.Log(angle);
                Reference.projectileController.ShootProjectile(player.go.transform.position, Quaternion.Euler(0,0,angle),"Bullet");
                Reference.soundController.FireBullet();
                Reference.shakeCamera.StartShake(0.05f,0.05f);

            }
        }
        if (Reference.playerInputController.leftClickDown)
        {
            Vector2 cursorPosition = Reference.mainCamera.ScreenToWorldPoint(Reference.playerInputController.cursorPosition);
            if (Time.time - rocketCooldownTimer > rocketCooldown)
            {//space to set variable cooldowns, and noises if on cooldown....etc
                rocketCooldownTimer = Time.time;
                float angle = -1*FindDegree(Reference.playergo.GetComponent<Rigidbody2D>().position,cursorPosition) + 90; // I have to hack this angle, why??
                Reference.projectileController.ShootProjectile(player.go.transform.position,Quaternion.Euler(0,0,angle),"Rocket");
            }
        }

    }
    public static float FindDegree(Vector2 v1, Vector2 v2)
    {
        float x = (v2.x - v1.x);
        float y = (v2.y - v1.y);
        float value = (float)((Mathf.Atan2(x, y) / Mathf.PI) * 180f);
        if(value < 0) value += 360f;
    
        return value;
    }
    public void ApplyRocketLaunchImpulse(Quaternion rotation)
    {   
        Debug.Log(rotation);
        Quaternion euler = Quaternion.Euler(0,0,-90);
        Vector2 direction = euler*(rotation * new Vector2 (gameObject.transform.up.x, gameObject.transform.up.y));
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
        if (Reference.playerInputController.leftKey || Reference.playerInputController.a)
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
        if (Reference.playerInputController.rightKey || Reference.playerInputController.d)
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
        if (Reference.playerInputController.upKey || Reference.playerInputController.w)
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
        if (Reference.playerInputController.downKey || Reference.playerInputController.s)
        {
            playerInputImpulse -= 1*mass;
            if(!playingRocketSound)
            {// todo - make reverse sound different?
                Reference.soundController.StartRocketBoost();
                playingRocketSound = true;
                blueFlameReverseLeft.SetActive(true);
                blueFlameReverseRight.SetActive(true);
                
            }
        }
        
        if(playerInputImpulse == 0 && playingRocketSound)
        {

            Reference.soundController.StopRocketBoost();
            playingRocketSound = false;
            blueFlameFunction.StopJet();
            blueFlame.SetActive(false);
            blueFlameReverseLeft.SetActive(false);
            blueFlameReverseRight.SetActive(false);

        }
        
        if(Reference.playerInputController.b && Time.time - lastBoost > boostCooldown)
        {
            lastBoost = Time.time;
            Vector2 directionBoost = new Vector2 (gameObject.transform.up.x, gameObject.transform.up.y);
            Vector2 impulseBoost = directionBoost * mass *  engineForce/2;
            rigid_body.AddForce(impulseBoost,ForceMode2D.Impulse);

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
