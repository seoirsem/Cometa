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
    public bool shootRocket = false;
    public bool shootBullet = false;
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
    public float angle;
    public float angleMove;
    public Vector2 moveDirection;
    public Vector2 shootDirection;
    public float moveMagnitude;

    GameObject moveJoystickgo;
    GameObject shootJoystickgo;
    bl_MovementJoystick moveStick;
    bl_MovementJoystick shootStick;

    string platform;
    
    void Awake()
    {

    }

    void Start()
    {
        platform = Reference.worldController.platform;

        moveJoystickgo = GameObject.Find("MoveJoystick");
        shootJoystickgo = GameObject.Find("ShootJoystick");
        moveStick = moveJoystickgo.GetComponent<bl_MovementJoystick>();
        shootStick = shootJoystickgo.GetComponent<bl_MovementJoystick>();
        if(platform != "Android")
        {
            //set the joysticks inactive
            moveJoystickgo.SetActive(false);
            shootJoystickgo.SetActive(false);
        }
    }

    void Update()
    {
        if(platform == "Android")
        {
            GetAndroidControls();
        }
        else if(platform == "Windows")
        {
            GetWindowsControls();
        }
        else if(platform == "Editor")
        {
            GetDebuggingControls();
        }
    }

    void GetAndroidControls()
    {
//        Debug.Log(moveStick.Horizontal);
        moveDirection = new Vector2(moveStick.Horizontal,moveStick.Vertical).normalized;
        shootDirection = new Vector2(shootStick.Horizontal,shootStick.Vertical).normalized;
        moveMagnitude = (new Vector2(moveStick.Horizontal,moveStick.Vertical)).magnitude;
        if(shootDirection != new Vector2(0,0))
        {
            shootBullet = true;
            angle = -1*FindDegree(new Vector2(0,0),shootDirection) + 90;
        }
        else
        {
            shootBullet = false;
        }
        if(moveDirection != new Vector2(0,0))
        {
            angleMove = -1*FindDegree(new Vector2(0,0),moveDirection);
        }
        else
        {
            angleMove = 720f; //ie a large number
        }


    }


    void GetWindowsControls()
    {
        
        cursorPosition = Reference.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        angle = -1*FindDegree(Reference.playergo.GetComponent<Rigidbody2D>().position,cursorPosition) + 90;

        if (Input.GetMouseButton(0))
        {
            shootBullet = true;
        }
        else
        {
            shootBullet = false;
        }
        if (Input.GetMouseButton(1))
        {
            shootRocket = true;
        }
        else
        {
            shootRocket = false;
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
        if (Input.GetKey("escape"))
        {
            escape = true;   
        }
        else 
        {
            escape = false;
        }
    }

    void GetDebuggingControls()
    {
        //needs a rewrite for mobile. Currently setup for maximal flexibility

        cursorPosition = Reference.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        angle = -1*FindDegree(Reference.playergo.GetComponent<Rigidbody2D>().position,cursorPosition) + 90;

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
            shootBullet = true;
        }
        else
        {
            shootBullet = false;
        }
        if (Input.GetMouseButton(1))
        {
            shootRocket = true;
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

    public static float FindDegree(Vector2 v1, Vector2 v2)
    {
        float x = (v2.x - v1.x);
        float y = (v2.y - v1.y);
        float value = (float)((Mathf.Atan2(x, y) / Mathf.PI) * 180f);
        if(value < 0) value += 360f;
    
        return value;
    }
}
