using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEdgeVisualsController : MonoBehaviour
{
    string side;
    GameObject playergo;
    GameObject go;
    Vector2 mapSize;
    void Start()
    {
        go = this.gameObject;
        playergo = Reference.playerSpriteController.gameObject;
        mapSize = Reference.worldController.worldSize;
    }

    public void SetSide(string side)
    {
        this.side = side;
    }

    void Update()
    {
        if(side == "up")
        {
            go.transform.rotation = playergo.transform.rotation;
            go.transform.position = playergo.transform.position + new Vector3(0, mapSize.y, 0);
        }
        if(side == "down")
        {
            go.transform.rotation = playergo.transform.rotation;
            go.transform.position = playergo.transform.position + new Vector3(0, -mapSize.y, 0);
        }
        if(side == "left")
        {
            go.transform.rotation = playergo.transform.rotation;
            go.transform.position = playergo.transform.position + new Vector3(-mapSize.x, 0, 0);
        }
        if(side == "right")
        {
            go.transform.rotation = playergo.transform.rotation;
            go.transform.position = playergo.transform.position + new Vector3(mapSize.x, 0, 0);
        }
        if(side == "upRight")
        {
            go.transform.rotation = playergo.transform.rotation;
            go.transform.position = playergo.transform.position + new Vector3(mapSize.x, mapSize.y, 0);
        }
        if(side == "downRight")
        {
            go.transform.rotation = playergo.transform.rotation;
            go.transform.position = playergo.transform.position + new Vector3(mapSize.x, -mapSize.y, 0);
        }
        if(side == "upLeft")
        {
            go.transform.rotation = playergo.transform.rotation;
            go.transform.position = playergo.transform.position + new Vector3(-mapSize.x, mapSize.y, 0);
        }
        if(side == "downLeft")
        {
            go.transform.rotation = playergo.transform.rotation;
            go.transform.position = playergo.transform.position + new Vector3(-mapSize.x, -mapSize.y, 0);
        }

    }
}
