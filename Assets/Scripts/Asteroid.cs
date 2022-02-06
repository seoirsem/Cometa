using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]

public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    Mesh mesh;
    public Vector3[] meshVertices;
    public int[] meshTriangles;
    float mass = 1;
    Vector3 rotation;
    public float rotationRate;
    GameObject asteroidgo;
    Vector3 velocity;
    Vector2 worldSize;
    Vector2 location;
    List<GameObject> asteroidPack;



    public void OnSpawn(int size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector3 velocity)
    {
        this.asteroidPack = asteroidPack;
        asteroidgo = this.gameObject;
        this.velocity = velocity;
        this.worldSize = Reference.worldController.worldSize;
        this.location = location;

        //creates the mesh, or clones the mesh if this is not the first one spawned
        if (this.gameObject == mainAsteroid)
        {
            //in Unity trianges are drawn clockwise
            this.rotationRate = Mathf.Pow(Random.Range(0f, 1f),2.2f) * 400;//random rotation rate
            DrawAsteroid(size);
        }
        else
        {
            this.rotationRate = mainAsteroid.GetComponent<Asteroid>().rotationRate;
            CloneAsteroid(mainAsteroid);
        }


    }

    void DrawAsteroid(int size)
    {
        //random int including the starting number, excluding the finishing number
        int numberOfSides = Random.Range(4, 11);

        float radius = size / 6f;

        meshVertices = new Vector3[numberOfSides];
        float angle = 0;
        for (int i = 0; i < numberOfSides; i++)
        {//drawing a simple circle
            angle = 2 * Mathf.PI * i / numberOfSides;
            meshVertices[i] = new Vector3(radius*Mathf.Sin(angle), radius * Mathf.Cos(angle), 0);
        
        }
        
        meshTriangles = new int[3*numberOfSides];
        for (int i = 0; i < numberOfSides - 2; i++)
        {
            meshTriangles[3*i] = i;
            meshTriangles[3*i+1] = i+1;
            meshTriangles[3*i+2] = i+2;
        }
        meshTriangles[3 * (numberOfSides - 2)] = numberOfSides - 2;
        meshTriangles[3 * (numberOfSides - 2) + 1] = numberOfSides - 1;
        meshTriangles[3 * (numberOfSides - 2) + 2] = 0;
        meshTriangles[3 * (numberOfSides - 1)] = numberOfSides - 1;
        meshTriangles[3 * (numberOfSides - 1) + 1] = 0;
        meshTriangles[3 * (numberOfSides - 1) + 2] = 1;



        //Notes on drawing lines
        //https://docs.unity3d.com/ScriptReference/MeshTopology.html

        //notation for drawing a square
        //meshVertices = new Vector3[]
        //{
        //    new Vector3(1,1,0),
        //    new Vector3(1,-1,0),
        //    new Vector3(-1,-1,0),
        //    new Vector3(-1,1,0)
        //};

        //meshTriangles = new int[]
        //{
        //    0,1,2,
        //    2,3,0
        //};


        DrawMesh(meshVertices, meshTriangles);

    }

    void CloneAsteroid(GameObject mainAsteroid)
    {
        Asteroid mainAst = mainAsteroid.GetComponent<Asteroid>();
        DrawMesh(mainAst.meshVertices, mainAst.meshTriangles);

    }
    void DrawMesh(Vector3[] vertices, int[] triangles)
    {
        mesh = new Mesh();
        this.gameObject.GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateAsteroidPosition();
        UpdateAsteroidRotation();
    }

    void UpdateAsteroidPosition()
    {
        this.asteroidgo.transform.position += velocity * Time.deltaTime;
        //Debug.Log(velocity.magnitude);

        if (asteroidgo.transform.position.x - location.x * worldSize.x > worldSize.x / 2)
        {
            asteroidgo.transform.position = new Vector3(asteroidgo.transform.position.x - worldSize.x, asteroidgo.transform.position.y, asteroidgo.transform.position.z);
        }
        if (asteroidgo.transform.position.x - location.x * worldSize.x < -worldSize.x / 2)
        {
            asteroidgo.transform.position = new Vector3(asteroidgo.transform.position.x + worldSize.x, asteroidgo.transform.position.y, asteroidgo.transform.position.z);
        }
        if (asteroidgo.transform.position.y - location.y * worldSize.y > worldSize.y / 2)
        {
            asteroidgo.transform.position = new Vector3(asteroidgo.transform.position.x, asteroidgo.transform.position.y - worldSize.y, asteroidgo.transform.position.z);
        }
        if (asteroidgo.transform.position.y - location.y * worldSize.y < -worldSize.y / 2)
        {
            asteroidgo.transform.position = new Vector3(asteroidgo.transform.position.x, asteroidgo.transform.position.y + worldSize.y, asteroidgo.transform.position.z);
        }
    }
    void UpdateAsteroidRotation()
    {
        float frameRotation = Time.deltaTime * rotationRate;
        this.gameObject.transform.Rotate(new Vector3(0, 0, frameRotation));
    }
}
