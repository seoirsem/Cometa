using System.Collections;
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
    // public float rotationRate;
    public GameObject asteroidgo;
    public Vector3 velocity;
    public Vector2 worldSize;
    public Vector2 location;
    public List<GameObject> asteroidPack;
    public PolygonCollider2D polygonCollider;
    public Rigidbody2D rigid_body;
    // public Vector3 CoMShift;
    public AsteroidController asteroidController;
    public float[] vertexHealth;

    public Vector2 rigidBodyVelocity;

    static public float celSize;
    public int size;

    public SquareMesh squareMesh;

    private void Awake() {
        celSize = 0.3f;
    }

    public void ReDrawAsteroid()
    {
        squareMesh.FindOutline();
        squareMesh.ScaleEdgeLengthAndShift();
        // squareMesh.ResetMesh();
        squareMesh.ResetColliderMesh();
        squareMesh.FindCentreOfMass();
        this.gameObject.transform.Find("Texture").GetComponent<AsteroidTextureController>().UpdateTexture();
        rigid_body = this.gameObject.GetComponent<Rigidbody2D>();
        rigid_body.centerOfMass = squareMesh.centreOfMass;
        this.mass = squareMesh.mass;
        rigid_body.mass = squareMesh.mass;
    }

    public void DrawAsteroid(int size, SquareMesh oldMesh, SquareMesh squareMeshIn)
    {              
        polygonCollider = this.gameObject.GetComponent<PolygonCollider2D>();
        this.size = size;

        if(squareMeshIn == null)
        {
            squareMesh = new SquareMesh();
            squareMesh.SetAsteroid(this);
            // squareMesh.GenerateRandomShapeMesh(size,celSize);
            squareMesh.GenerateCircularMesh(size,celSize);
            squareMesh.FindOutline();
            // Debug.Log(squareMesh.perimeterVertices.Count);
            squareMesh.ScaleEdgeLengthAndShift();
            squareMesh.ResetMesh();
            squareMesh.ResetColliderMesh();
            squareMesh.FindCentreOfMass();
            // this.gameObject.transform.Find("Texture").GetComponent<AsteroidTextureController>().UpdateTexture();
        }
        else
        {
            squareMesh = squareMeshIn;
            squareMesh.SetAsteroid(this);
            squareMesh.FindOutline();
            squareMesh.ScaleEdgeLengthAndShift();
            squareMesh.ResetMesh();
            squareMesh.ResetColliderMesh();
            squareMesh.FindCentreOfMass();
            this.gameObject.transform.Find("Texture").GetComponent<AsteroidTextureController>().UpdateTexture(oldMesh);
        }

        rigid_body = this.gameObject.GetComponent<Rigidbody2D>();
        rigid_body.centerOfMass = squareMesh.centreOfMass;
        //rigid_body.ResetCenterOfMass();
        this.mass = squareMesh.mass;
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
        this.mass = squareMesh.mass;
        rigid_body.mass = squareMesh.mass;
    }

    void SetupRigidBody()
    {
 //       Rigidbody2D rigi
    }


}
