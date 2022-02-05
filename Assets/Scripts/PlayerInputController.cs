using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public bool upKey = false;
    public bool downKey = false;
    public bool leftKey = false;
    public bool rightKey = false;
    public Vector2 cursorPosition;
    public bool mouseClicked = false;
    public bool spaceBar = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInputs();

    }


    void GetPlayerInputs()
    {
        //needs a rewrite for mobile. Currently setup for maximal flexibility

        cursorPosition = Input.mousePosition;

        if (Input.GetKey("up"))
        {
            upKey = true;   
        }
        else
        {
            upKey = false;
        }
        if (Input.GetKey("down"))
        {
            downKey = true;   
        }
        else
        {
            downKey = false;
        }
        if (Input.GetKey("left"))
        {
            leftKey = true;   
        }
        else
        {
            leftKey = false;
        }
        if (Input.GetKey("right"))
        {
            rightKey = true;   
        }
        else
        {
            rightKey = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            mouseClicked = true;
        }
        else
        {
            mouseClicked = false;
        }
        if (Input.GetKey("space"))
        {
            spaceBar = true;
        }
        else
        {
            spaceBar = false;
        }
    }
}
