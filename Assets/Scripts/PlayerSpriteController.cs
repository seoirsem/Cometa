using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    Player player;
    GameObject playergo;
    float rotationRate = 400; //degree/s
    float engineForce = 6; 
    Reference reference;
    float rotation = 0;
    Vector3 velocity = new Vector3(0, 0, 0);
    Vector2 worldEdges;
    float maxSpeed = 10;
    List<GameObject> visualClones;
    float shootingCooldownTimer;
    float cooldown = 0.75f;//s
    

    void Start()
    {
        playergo = Reference.playergo;
        playergo.transform.position = new Vector2(0, 0);
        playergo.transform.rotation = new Quaternion(0, 0, 0, 0);
        player = Reference.worldController.player;
        worldEdges = Reference.worldController.worldSize;
        SpawnVisualClones();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerRotation();
        UpdatePlayerMotion();
        UpdatePlayerShooting();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Collision! - Player");
    }
    void UpdatePlayerShooting()
    {
        if (Reference.playerInputController.spaceBar)
        {
            if (Time.time - shootingCooldownTimer > cooldown)
            {//space to set variable cooldowns, and noises if on cooldown....etc
                shootingCooldownTimer = Time.time;

                Reference.projectileController.ShootProjectile(player.go.transform.position,playergo.transform.rotation* Quaternion.Euler(0, 0, 90));
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

        //visualClones[3] = go;

    }
    void UpdatePlayerRotation()
    {

        float playerInputRotation = 0;
        if (Reference.playerInputController.leftKey)
        {
            playerInputRotation += 1;
        }
        if (Reference.playerInputController.rightKey)
        {
            playerInputRotation -= 1;
        }
        float frameRotation = Time.deltaTime *playerInputRotation*rotationRate;

        playergo.transform.Rotate(new Vector3( 0, 0,frameRotation));
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
        //float rotation = playergo.transform.eulerAngles.z/(2* Mathf.PI);
        //Debug.Log(rotation);
        //Debug.Log(Mathf.Cos(rotation));

        //Debug.Log("Cos "+ Mathf.Cos(rotation) + "Sin " +  Mathf.Sin(rotation));
        
        Vector3 impulse = transform.up * playerInputImpulse * engineForce / player.mass;
        //Debug.Log(impulse);
        velocity += Time.deltaTime*impulse;
        if(velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
        playergo.transform.position += velocity * Time.deltaTime;
        //Debug.Log(velocity.magnitude);




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



}
