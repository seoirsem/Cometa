using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]

public class Asteroid : MonoBehaviour
{
    public GameObject asteroidOutlines;
    // Start is called before the first frame update
    public Mesh mesh;
    public Mesh colliderMesh;
    public Vector3[] meshVertices;
    public int[] meshTriangles;
    public int[] meshIndices;
    public float mass = 1;
    public Vector3 rotation;
    public float rotationRate;
    public GameObject asteroidgo;
    public Vector3 velocity;
    public Vector2 worldSize;
    public Vector2 location;
    public List<GameObject> asteroidPack;
    public PolygonCollider2D polygonCollider;
    public Rigidbody2D rigid_body;
    public AsteroidController asteroidController;
    public int size;


    public void DrawAsteroid(int size)
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


    public void DrawMesh(Vector3[] vertices, int[] triangles, int[] meshIndices)
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

    public void DrawCollider(Vector3[] vertices, int[] triangles)
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

}
