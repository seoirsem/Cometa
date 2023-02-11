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
    public bool mouseClickDown = false;
    public bool leftClickDown = false;
    public bool spaceBar = false;
    public bool b = false;
    public bool r = false;
    public bool escape = false;
    public bool e = false;

    public bool p = false;
    public bool o = false;
    public bool w = false;
    public bool a = false;
    public bool s = false;
    public bool d = false;
    
    void Start()
    {
        
    }

    void Update()
    {
        GetPlayerInputs();

    }


    void GetPlayerInputs()
    {
        //needs a rewrite for mobile. Currently setup for maximal flexibility

        cursorPosition = Input.mousePosition;

        if (Input.GetKey("escape"))
        {
            escape = true;   
        }
        else 
        {
            escape = false;
        }
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
        if (Input.GetMouseButton(0))
        {
            mouseClickDown = true;
        }
        else
        {
            mouseClickDown = false;
        }
        if (Input.GetMouseButton(1))
        {
            leftClickDown = true;
        }
        else
        {
            leftClickDown = false;
        }
        if (Input.GetKey("space"))
        {
            spaceBar = true;
        }
        else
        {
            spaceBar = false;
        }
        if (Input.GetKey("r"))
        {
            r = true;
        }
        else
        {
            r = false;
        }
        if (Input.GetKey("b"))
        {
            b = true;
        }
        else
        {
            b = false;
        }
        if (Input.GetKey("o"))
        {
            o = true;
        }
        else
        {
            o = false;
        }
        if (Input.GetKey("p"))
        {
            p = true;
        }
        else
        {
            p = false;
        }
        if (Input.GetKey("e"))
        {
            e = true;
        }
        else
        {
            e = false;
        }
        if (Input.GetKey("w"))
        {
            w = true;
        }
        else
        {
            w = false;
        }
        if (Input.GetKey("a"))
        {
            a = true;
        }
        else
        {
            a = false;
        }
        if (Input.GetKey("s"))
        {
            s = true;
        }
        else
        {
            s = false;
        }
        if (Input.GetKey("d"))
        {
            d = true;
        }
        else
        {
            d = false;
        }
    }
}
