using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this code from: https://stackoverflow.com/questions/38728714/unity3d-how-to-detect-taps-on-android

public class TapManager : MonoBehaviour
{        
    private float[] timeTouchBegan;
    private bool[] touchDidMove;
    private float tapTimeThreshold = 0.2f;

    public Vector2 worldTapPosition;
    public bool shootRocket;
    float rocketCooldown = 0.05f;
    float timeShootRocket;

    Vector2 worldSize;

    void Start()
    {
        shootRocket = false;
        timeShootRocket = Time.time;
        worldSize = Reference.worldController.worldSize;
        timeTouchBegan = new float[10];
        touchDidMove = new bool[10];

    }

    private void Update()
    {
        // Touches
        foreach (Touch touch in Input.touches)
        {
            int fingerIndex = touch.fingerId;

            if (touch.phase == TouchPhase.Began)
            {
                //Debug.Log("Finger #" + fingerIndex.ToString() + " entered!");
                timeTouchBegan[fingerIndex] = Time.time;
                touchDidMove[fingerIndex] = false;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                //Debug.Log("Finger #" + fingerIndex.ToString() + " moved!");
                touchDidMove[fingerIndex] = true;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                float tapTime = Time.time - timeTouchBegan[fingerIndex];
//                Debug.Log(tapTime);
                Debug.Log("Finger #" + fingerIndex.ToString() + " left. Tap time: " + tapTime.ToString());
                if (tapTime <= tapTimeThreshold)// && touchDidMove[fingerIndex] == false)
                {
                    Vector2 tapPosition = touch.position;
                    worldTapPosition = Reference.mainCamera.ScreenToWorldPoint(tapPosition);
                    Debug.Log("Finger #" + fingerIndex.ToString() + " TAP DETECTED at: " + worldTapPosition.ToString());
                    if((worldTapPosition.x < worldSize.x/2 && worldTapPosition.x > -worldSize.x/2) && (worldTapPosition.y < worldSize.y/2 && worldTapPosition.y > -worldSize.y/2))
                    { // the player has tapped on the world - fire a rocket!
                        //Debug.Log("On world");
                        shootRocket = true;
                        timeShootRocket = Time.time;
                    }
                }
            }         

        }

        if(shootRocket && Time.time - rocketCooldown > timeShootRocket)
        {
            shootRocket = false;
        }

        
    }   
}