using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]

public class Asteroid : MonoBehaviour
{
    GameObject asteroidOutlines;
    // Start is called before the first frame update
    Mesh mesh;
    Mesh colliderMesh;
    public Vector3[] meshVertices;
    public int[] meshTriangles;
    public int[] meshIndices;
    float mass = 1;
    Vector3 rotation;
    public float rotationRate;
    GameObject asteroidgo;
    public Vector3 velocity;
    Vector2 worldSize;
    Vector2 location;
    List<GameObject> asteroidPack;
    PolygonCollider2D polygonCollider;
    Rigidbody2D rigid_body;
    AsteroidController asteroidController;
    public int size;
    GameObject mainAsteroid;



    public void OnSpawn(int size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector3 velocity)
    {
        this.mass = Mathf.Pow(size,2);
        // Update this in future to calculate based on the area of the shape formed by the mesh
        //Debug.Log(this.mass);

        this.size = size;
        asteroidController = GameObject.Find("AsteroidController").GetComponent<AsteroidController>();
        asteroidOutlines = this.gameObject.transform.Find("AsteroidOutline").gameObject;
        rigid_body = this.GetComponent<Rigidbody2D>();
        this.asteroidPack = asteroidPack;
        asteroidgo = this.gameObject;
        this.velocity = velocity;
        this.worldSize = Reference.worldController.worldSize;
        this.location = location;

        //creates the mesh, or clones the mesh if this is not the first one spawned
        if (this.gameObject == mainAsteroid)
        {
            //in Unity trianges are drawn clockwise
            this.rotationRate = Mathf.Pow(Random.Range(-1f, 1f),2f) * 250;//random rotation rate
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
        int numberOfSides = Random.Range(5, 13);

        float radius = size / 6f;

        meshVertices = new Vector3[numberOfSides+1];
        float angle = 0;
        Vector3 randomOffset;
        float randomAngleOffset;
        float randomRadius;
        for (int i = 0; i < numberOfSides; i++)
        {//drawing a simple circle
            angle = 2 * Mathf.PI * i / numberOfSides;
            randomRadius = Random.Range(0.6f*radius,1.2f*radius);//allowable range of radii
            randomAngleOffset = Random.Range(angle + 2 * Mathf.PI * 0.5f / numberOfSides, angle - 2 * Mathf.PI * 0.5f / numberOfSides);
            randomOffset = new Vector3(randomRadius*Mathf.Sin(randomAngleOffset), randomRadius * Mathf.Cos(randomAngleOffset), 0);
            meshVertices[i] = randomOffset;
            //new Vector3(radius*Mathf.Sin(angle), radius * Mathf.Cos(angle), 0);
        
        }
        meshVertices[numberOfSides] = new Vector3(0, 0, 0); // the centre of the object is the last vertex in the list


        meshTriangles = new int[3*numberOfSides];
        for (int i = 0; i < numberOfSides - 1; i++)
        {
            meshTriangles[3*i] = i;
            meshTriangles[3*i+1] = i+1;
            meshTriangles[3*i+2] = numberOfSides;
        }
        meshTriangles[3 * (numberOfSides - 1)] = numberOfSides - 1;
        meshTriangles[3 * (numberOfSides - 1) + 1] = 0;
        meshTriangles[3 * (numberOfSides - 1) + 2] = numberOfSides;

        meshIndices = new int[2 * numberOfSides];
        for (int i = 0; i < numberOfSides - 1; i++)
        {
            meshIndices[2 * i] = i;
            meshIndices[2 * i + 1] = i + 1;
        }
        meshIndices[2 * (numberOfSides - 1) - 1] = numberOfSides - 1;
        meshIndices[2 * (numberOfSides - 1)] = 0;




        //Notes on drawing lines
        //https://docs.unity3d.com/ScriptReference/MeshTopology.html
        // code to do so: https://www.h3xed.com/programming/create-2d-mesh-outline-in-unity-silhouette

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


        DrawMesh(meshVertices, meshTriangles, meshIndices);
        DrawCollider(meshVertices, meshTriangles);

    }

    void CloneAsteroid(GameObject mainAsteroid)
    {
        Asteroid mainAst = mainAsteroid.GetComponent<Asteroid>();
        meshVertices = mainAst.meshVertices;
        meshTriangles = mainAst.meshTriangles;
        meshIndices = mainAst.meshIndices;
        DrawMesh(meshVertices, meshTriangles, meshIndices);
        DrawCollider(meshVertices, meshTriangles);
        
    }
    void DrawMesh(Vector3[] vertices, int[] triangles, int[] meshIndices)
    {
        mesh = new Mesh();
        this.gameObject.GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        MeshFilter outlineFilter = asteroidOutlines.GetComponent<MeshFilter>();
        Mesh mesh2 = new Mesh();
        outlineFilter.mesh = mesh2;
        Vector3[] verticesReduced = new Vector3[vertices.Length - 1];
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            verticesReduced[i] = vertices[i];
        }
        //foreach (Vector2 item in verticesReduced)
        //{
        //    Debug.Log(item.x + " " + item.y);
        //}
        mesh2.vertices = verticesReduced;
        mesh2.SetIndices(meshIndices, MeshTopology.Lines, 0);



    }

    void DrawCollider(Vector3[] vertices, int[] triangles)
    {
        colliderMesh = mesh;
        polygonCollider = this.gameObject.GetComponent<PolygonCollider2D>();

        Vector2[] xyPoints = new Vector2[vertices.Length-1];
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            xyPoints[i] = new Vector2(vertices[i].x, vertices[i].y);
        }
        polygonCollider.points = xyPoints; 

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
        rigid_body.rotation += frameRotation;
        //this.gameObject.transform.Rotate(new Vector3(0, 0, frameRotation));
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        //if (this.gameObject == mainAsteroid)
        //{
        if (collision.gameObject.GetComponent<Asteroid>() is null)
        {
            asteroidController.AsteroidHit(this, collision, asteroidPack);
        }
        else
        {//if you have collided with another asteroid
           Asteroid otherAsteroid = collision.gameObject.GetComponent<Asteroid>();
           //Vector3 positionVector = collision.conta
            // //foreach (ContactPoint contact in collision.GetContacts)
            // {
            //     Debug.DrawRay(contact.point, contact.normal, Color.white);
            // }
        }
        //}
    }

}
