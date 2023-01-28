﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]

public class Asteroid : MonoBehaviour
{
    //public GameObject asteroidOutlines;
    // Start is called before the first frame update
    public Mesh mesh;
    public Mesh colliderMesh;
    public Vector3[] meshVertices;
    public int[] meshTriangles;
    public int[] meshIndices;
    public float mass;
    public Vector3 rotation;
    public float rotationRate;
    public GameObject asteroidgo;
    public Vector3 velocity;
    public Vector2 worldSize;
    public Vector2 location;
    public List<GameObject> asteroidPack;
    public PolygonCollider2D polygonCollider;
    public Rigidbody2D rigid_body;
    public Vector3 CoMShift;
    public AsteroidController asteroidController;
    public float[] vertexHealth;

    static public float celSize;
    public int size;

    public SquareMesh squareMesh;

    private void Awake() {
        celSize = 0.1f;
    }

    public void DrawAsteroid(int size, SquareMesh squareMeshIn)
    {              
        polygonCollider = this.gameObject.GetComponent<PolygonCollider2D>();
        this.size = size;

        if(squareMeshIn == null)
        {
            squareMesh = new SquareMesh();
            squareMesh.SetAsteroid(this);
            squareMesh.GenerateCircularMesh(size,celSize);
            squareMesh.FindOutline();
            squareMesh.ScaleEdgeLength();
            squareMesh.ResetMesh();
            squareMesh.ResetColliderMesh();
            Debug.Log(squareMesh.squares.GetLength(0));
            Debug.Log(squareMesh.squares.GetLength(1));
        }
        else
        {
            squareMesh.SetAsteroid(this);
            squareMesh = squareMeshIn;
            squareMesh.FindOutline();
            squareMesh.ResetMesh();
            squareMesh.ResetColliderMesh();
        }

        rigid_body = this.gameObject.GetComponent<Rigidbody2D>();
        rigid_body.centerOfMass = squareMesh.centreOfMass;
        rigid_body.mass = squareMesh.mass;

    }

    public void DrawDerivedAsteroid()
    {
        squareMesh = new SquareMesh();
        squareMesh.edgeLength = celSize;
        squareMesh.SetAsteroid(this);
        polygonCollider = this.gameObject.GetComponent<PolygonCollider2D>();
        rigid_body = this.gameObject.GetComponent<Rigidbody2D>();
        rigid_body.centerOfMass = squareMesh.centreOfMass;
        rigid_body.mass = squareMesh.mass;
    }

    void SetupRigidBody()
    {
 //       Rigidbody2D rigi
    }


}
