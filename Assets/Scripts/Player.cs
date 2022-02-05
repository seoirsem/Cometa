using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Vector2 velocity;
    public float mass = 1;
    public int lives;
    public GameObject go;


    public Player(GameObject go)
    {
        this.go = go;
    }
    
    public void OnHit()
    {
        this.lives -= 1;
    }



}
