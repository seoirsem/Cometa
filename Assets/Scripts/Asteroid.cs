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

    public Asteroid[] SplitAsteroid(Vector3[] meshVs, Vector3 collisionPoint, Vector3 collisionDirection)
    {
        Asteroid[] asteroidMeshData = new Asteroid[2];
        asteroidMeshData[0] = new Asteroid();
        asteroidMeshData[1] = new Asteroid();

        List<Vector3> vList1 = new List<Vector3>();
        List<Vector3> vList2 = new List<Vector3>();
        List<Vector3> vReorderedList1 = new List<Vector3>();
        List<Vector3> vReorderedList2 = new List<Vector3>();


        for (int i = 0; i < meshVs.Length-1; i++)
        {
            if (Vector3.Cross(meshVs[i], collisionDirection).z > 0f)
            {
                vList1.Add(meshVs[i]);
            }
            else
            {
                vList2.Add(meshVs[i]);
            }
        }

        vReorderedList1.Add(meshVs[meshVs.Length-1]);
        vReorderedList1.Add(collisionPoint);
        int loopIterations = vList1.Count;
        float minAngle = 999f;
        int minIndex = -1;
        Vector3 CoM = meshVs[meshVs.Length-1];
        Vector3 fromCoMtoCP = collisionPoint - CoM;
        for (int i = 0; i < loopIterations; i++)
        {
            for (int j = 0; j < vList1.Count; j++)
            {
                float newAngle = Vector3.SignedAngle( fromCoMtoCP, (vList1[j] - CoM), Vector3.back );
                if ( newAngle < minAngle )
                {
                    minAngle = newAngle;
                    minIndex = j;
                }
            }
            vReorderedList1.Add(vList1[minIndex]);
            vList1.RemoveAt(minIndex);
            minAngle = 999f;
        }
        
        vReorderedList2.Add(meshVs[meshVs.Length-1]);
        vReorderedList2.Add(collisionPoint);
        loopIterations = vList2.Count;
        minAngle = 999f;
        minIndex = -1;
        CoM = meshVs[meshVs.Length-1];
        fromCoMtoCP = collisionPoint - CoM;
        for (int i = 0; i < loopIterations; i++)
        {
            for (int j = 0; j < vList2.Count; j++)
            {
                float newAngle = Vector3.SignedAngle( fromCoMtoCP, (vList2[j] - CoM), Vector3.forward );
                if ( newAngle < minAngle )
                {
                    minAngle = newAngle;
                    minIndex = j;
                }
            }
            vReorderedList2.Add(vList2[minIndex]);
            vList2.RemoveAt(minIndex);
            minAngle = 999f;
        }
        
        Vector3 exitPoint = vReorderedList2[vReorderedList2.Count-1] + (vReorderedList1[vReorderedList1.Count-1] - vReorderedList2[vReorderedList2.Count-1])*Random.Range(0.2f,0.8f);
        vReorderedList2.Add(exitPoint);
        vReorderedList1.Add(exitPoint);
        
        Vector3[] meshVertices1 = vReorderedList1.ToArray();
        Vector3[] meshVertices2 = vReorderedList2.ToArray();

        if (meshVertices1.Length >= 3)
        {
            int[] meshTriangles1 = GetTriangles(meshVertices1.Length - 1);
            int[] meshIndices1 = GetIndices(meshVertices1.Length - 1);
            asteroidMeshData[0].meshVertices = meshVertices1;
            asteroidMeshData[0].meshTriangles = meshTriangles1;
            asteroidMeshData[0].meshIndices = meshIndices1;
        }
        else
        {
            asteroidMeshData[0] = null;
        }

        if (meshVertices2.Length >= 3)
        {
            int[] meshTriangles2 = GetTriangles(meshVertices2.Length - 1);
            int[] meshIndices2 = GetIndices(meshVertices2.Length - 1);
            asteroidMeshData[1].meshVertices = meshVertices2;
            asteroidMeshData[1].meshTriangles = meshTriangles2;
            asteroidMeshData[1].meshIndices = meshIndices2;
        }
        else
        {
            asteroidMeshData[1] = null;
        }

        return asteroidMeshData;
    }

    public void DrawAsteroid(int size)
    {
        //random int including the starting number, excluding the finishing number
        int numberOfSides = Random.Range(4, 4);

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
        
        int[] meshTriangles = GetTriangles(numberOfSides);
        int[] meshIndices = GetIndices(numberOfSides);

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


        DrawMesh(meshVertices, meshTriangles);
        DrawCollider(meshVertices, meshTriangles);

    }

    private static int[] GetIndices(int numberOfSides)
    {
        int[] meshIs = new int[2 * numberOfSides];
        for (int i = 0; i < numberOfSides - 1; i++)
        {
            meshIs[2 * i] = i;
            meshIs[2 * i + 1] = i + 1;
        }
        meshIs[2 * (numberOfSides - 1) - 1] = numberOfSides - 1;
        meshIs[2 * (numberOfSides - 1)] = 0;
        return meshIs;
    }

    private static int[] GetTriangles(int numberOfSides)
    {
        int[] meshTs = new int[3*numberOfSides];
        for (int i = 0; i < numberOfSides - 1; i++)
        {
            meshTs[3*i] = i;
            meshTs[3*i+1] = i+1;
            meshTs[3*i+2] = numberOfSides;
        }
        meshTs[3 * (numberOfSides - 1)] = numberOfSides - 1;
        meshTs[3 * (numberOfSides - 1) + 1] = 0;
        meshTs[3 * (numberOfSides - 1) + 2] = numberOfSides;
        return meshTs;
    }


    public void DrawMesh(Vector3[] vertices, int[] triangles, bool fromSplit = false)
    {
        // mesh = new Mesh();
        // this.gameObject.GetComponent<MeshFilter>().mesh = mesh;
        // mesh.Clear();
        // mesh.vertices = vertices;
        // mesh.triangles = triangles;

        // Debug.Log(vertices.Length);

        MeshFilter outlineFilter = asteroidOutlines.GetComponent<MeshFilter>();
        Mesh mesh2 = new Mesh();
        outlineFilter.mesh = mesh2;
        List<Vector3> verticesReducedList = new List<Vector3>();
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            verticesReducedList.Add(vertices[i]);
        }
        Vector3[] verticesReduced;
        if (fromSplit == true)
        {
            verticesReduced = new Vector3[vertices.Length];
            verticesReduced = vertices;
        }
        else
        {
            verticesReduced = new Vector3[vertices.Length-1];
            verticesReduced = verticesReducedList.ToArray();
        }
        // Debug.Log(verticesReduced.Length);
        mesh2.vertices = verticesReduced;
        
        int[] meshIndices = new int[verticesReduced.Length*2];

        for (int i = 0; i < verticesReduced.Length; i++)
        {
            meshIndices[2*i] = i;
            meshIndices[2*i+1] = i+1;
        }
        meshIndices[verticesReduced.Length*2-1] = 0;
        
        // Debug.Log("vertices:");
        // for (int i = 0; i < verticesReduced.Length; i++)
        // {
        //     Debug.Log(verticesReduced[i]);
        // }
        // Debug.Log("indices:");
        // for (int i = 0; i < meshIndices.Length; i++)
        // {
        //     Debug.Log(meshIndices[i]);
        // }

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

    public static bool operator == (Asteroid d1, Asteroid d2)
     {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(d1, d2))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)d1 == null) || ((object)d2 == null))
        {
            return false;
        }

        // Return true if the fields match:
        return d1.asteroidOutlines == d2.asteroidOutlines && d1.location == d2.location && d1.asteroidgo == d2.asteroidgo;
     }

    public override bool Equals(System.Object obj)
     {
        // If parameter cannot be cast to ThreeDPoint return false:
        Asteroid d = obj as Asteroid;
        if ((object)d == null)
        {
            return false;
        }
        
        // Return true if the fields match:
        return asteroidOutlines == d.asteroidOutlines && location == d.location && asteroidgo == d.asteroidgo;
     }

    public static bool operator !=(Asteroid d1, Asteroid d2)
    {
        return !(d1 == d2);
    }



}
