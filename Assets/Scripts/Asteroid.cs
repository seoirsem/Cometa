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
    public Vector3 CoMShift;
    public AsteroidController asteroidController;
    public float size;
    public float[] vertexHealth;

    public Asteroid[] SplitAsteroid(Vector3[] meshVs, Vector3 collisionPoint, Vector3 collisionDirection)
    {
        Asteroid[] asteroidMeshData = new Asteroid[2];
        asteroidMeshData[0] = new Asteroid();
        asteroidMeshData[1] = new Asteroid();

        Vector2[] tmp = StruckEdgeIndices(collisionDirection, collisionPoint, meshVs);

        // Debug.Log("Yep, can confirm, collision point is:");
        // Debug.Log(collisionPoint);
        // Debug.Log("And direction is:");
        // Debug.Log(collisionDirection);

        List<Vector3> vList1 = new List<Vector3>();
        List<Vector3> vList2 = new List<Vector3>();
        List<Vector3> vReorderedList1 = new List<Vector3>();
        List<Vector3> vReorderedList2 = new List<Vector3>();

        // Sort vertices into two list, either side of the collision direction (direction of incoming missile)
        // Note: last element (CoM of old asteroid) is omitted
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

        Debug.Log("New points are:");
        Debug.Log( (meshVs[(int)tmp[0].x] + (meshVs[(int)tmp[0].y] - meshVs[(int)tmp[0].x]) * 0.5f).ToString("F4") );
        Debug.Log( (meshVs[(int)tmp[1].x] + (meshVs[(int)tmp[1].y] - meshVs[(int)tmp[1].x]) * 0.5f).ToString("F4") );
        Debug.Log("Pierwszy");
        Debug.Log(meshVs[(int)tmp[0].x].ToString("F4") );
        Debug.Log(meshVs[(int)tmp[0].y].ToString("F4") );
        Debug.Log(tmp[0]);
        Debug.Log(tmp[0].x);
        Debug.Log((int)tmp[0].x);
        Debug.Log("Drugi");
        Debug.Log(meshVs[(int)tmp[1].x].ToString("F4") );
        Debug.Log(meshVs[(int)tmp[1].y].ToString("F4") );
        Debug.Log(tmp[1]);
        Debug.Log(tmp[1].x);
        Debug.Log((int)tmp[1].x);

        

        vList1.Add(meshVs[(int)tmp[0].x] + (meshVs[(int)tmp[0].y] - meshVs[(int)tmp[0].x]) * 0.5f);
        vList1.Add(meshVs[(int)tmp[1].x] + (meshVs[(int)tmp[1].y] - meshVs[(int)tmp[1].x]) * 0.5f);

        vList2.Add(meshVs[(int)tmp[0].x] + (meshVs[(int)tmp[0].y] - meshVs[(int)tmp[0].x]) * 0.5f);
        vList2.Add(meshVs[(int)tmp[1].x] + (meshVs[(int)tmp[1].y] - meshVs[(int)tmp[1].x]) * 0.5f);

        Debug.Log("List1");
        Debug.Log(vList1.Count);
        for (int i = 0; i < vList1.Count; i++)
        {
            Debug.Log(vList1[i].ToString("F4"));
        }
        Debug.Log("List2");
        Debug.Log(vList2.Count);
        for (int i = 0; i < vList2.Count; i++)
        {
            Debug.Log(vList2[i].ToString("F4"));
        }

        // vList2.Add(collisionPoint);

        // Would be nice to add exit point to the list, as well as the CoM of original asteroid
        // But this has proven to be quite buggy and challenging to do with arbitrary shapes
        // Vector3 exitPoint = ???
        // vReorderedList2.Add(exitPoint);
        // vReorderedList1.Add(exitPoint);

        // Sort vertices clockwise; using 360 degree convention, increasing anti-cw from 0 at right-pointing vector [1,0,0]
        int loopIterations = vList1.Count;
        float minAngle = 999f;
        int minIndex = -1;

        Vector3 oldCoM = meshVs[meshVs.Length-1] - Vector3.zero;
        float angle;
        bool CoMAdded = false;
        for (int j = 0; j < loopIterations; j++)
        {
            for (int i = 0; i < vList1.Count; i++)
            {
                angle = Vector3.SignedAngle( Vector3.right, (vList1[i] - oldCoM), Vector3.forward );
                if ( angle < 0) { angle = 360 + angle; }
                if (angle < minAngle) { minIndex = i; minAngle = angle;}
            }
            vReorderedList1.Add(vList1[minIndex]); 
            vList1.RemoveAt(minIndex);
            minIndex = -1;
            minAngle = 999f;
        }

        loopIterations = vList2.Count;
        minAngle = 999f;
        minIndex = -1;
        oldCoM = meshVs[meshVs.Length-1] - Vector3.zero;

        // for (int k = 0; k < vList2.Count; k++)
        // {
        //     vList2[k] = new Vector3(-vList2[k].x, vList2[k].y, vList2[k].z);
        // }

        for (int j = 0; j < loopIterations; j++)
        {
            for (int i = 0; i < vList2.Count; i++)
            {
                angle = Vector3.SignedAngle( Vector3.right, (vList2[i] - oldCoM), Vector3.forward );
                if ( angle < 0) { angle = 360 + angle; }
                if (angle < minAngle) { minIndex = i; minAngle = angle;}
            }
            vReorderedList2.Add(vList2[minIndex]); 
            vList2.RemoveAt(minIndex);
            minIndex = -1;
            minAngle = 999f;
        }
        
        // for (int k = 0; k < vReorderedList2.Count; k++)
        // {
        //     vReorderedList2[k] = new Vector3(-vReorderedList2[k].x, vReorderedList2[k].y, vReorderedList2[k].z);
        // }

        // Calculate the new CoM and save it in a separate asteroid objects field
        // So that we know how much to offset the spawning by, so that newly formed
        // Asteroids don't end up overlapping at spawn
        vReorderedList1 = CalculateCOM(vReorderedList1);
        asteroidMeshData[0].CoMShift = vReorderedList1[vReorderedList1.Count-1];
        vReorderedList1.RemoveAt(vReorderedList1.Count-1);
        vReorderedList2 = CalculateCOM(vReorderedList2, -1f);
        asteroidMeshData[1].CoMShift = vReorderedList2[vReorderedList2.Count-1];
        vReorderedList2.RemoveAt(vReorderedList2.Count-1);

        // Size is now proportional to, well, actual size
        // Use below function to get the area of the polygon
        asteroidMeshData[0].size = GetPolygonArea(vReorderedList1);
        asteroidMeshData[1].size = GetPolygonArea(vReorderedList2);

        Vector3[] meshVertices1 = vReorderedList1.ToArray();
        Vector3[] meshVertices2 = vReorderedList2.ToArray();

        // If the resulting asteroid has a low number vertices, don't bother.
        // PErhaps we can just spawn some dust/chaff in the area?
        if (meshVertices1.Length >= 3)
        {
            int[] meshTriangles1 = GetTriangles(meshVertices1.Length - 1);
            asteroidMeshData[0].meshVertices = meshVertices1;
            asteroidMeshData[0].meshTriangles = meshTriangles1;
        }
        else
        {
            asteroidMeshData[0] = null;
        }

        if (meshVertices2.Length >= 3)
        {
            int[] meshTriangles2 = GetTriangles(meshVertices2.Length - 1);
            asteroidMeshData[1].meshVertices = meshVertices2;
            asteroidMeshData[1].meshTriangles = meshTriangles2;
        }
        else
        {
            asteroidMeshData[1] = null;
        }
        return asteroidMeshData;
    }

    public List<Vector3> CalculateCOM(List<Vector3> vertices, float sign = 1f)
    {
        vertices.Add(vertices[0]);
        float A = 0f;
        float Cx = 0f;
        float Cy = 0f;

        // A = sign*GetPolygonArea(vertices);

        // for (int i = 0; i < vertices.Count-1; i++)
        // {
        //     Cx += 1f/(6f*A) * (vertices[i].x + vertices[i+1].x) * (vertices[i].x * vertices[i+1].y - vertices[i+1].x * vertices[i].y);
        // }
        // for (int i = 0; i < vertices.Count-1; i++)
        // {
        //     Cy += 1f/(6f*A) * (vertices[i].y + vertices[i+1].y) * (vertices[i].x * vertices[i+1].y - vertices[i+1].x * vertices[i].y);
        // }

        for (int i = 0; i < vertices.Count-1; i++)
        {
            Cx += vertices[i].x;
        }
        for (int i = 0; i < vertices.Count-1; i++)
        {
            Cy += vertices[i].y;
        }

        vertices.RemoveAt(vertices.Count-1);    
        vertices.Add(new Vector3(Cx/((float)vertices.Count), Cy/((float)vertices.Count), 0f));
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] -= vertices[vertices.Count-1];
        }
        vertices.Add(new Vector3(Cx/((float)vertices.Count), Cy/((float)vertices.Count), 0f));
        return vertices;
    }

    public Vector2[] StruckEdgeIndices(Vector3 direction, Vector3 collisionPoint, Vector3[] vertices)
    {
        float crossProduct1;
        float crossProduct2;
        Vector2[] indices = new Vector2[2];
        indices[0] = new Vector2(-1, -1); indices[1] = new Vector2(-1, -1);
        int count = 0;
        for (int i = 0; i < vertices.Length-1; i++)
        {
            crossProduct1 = Vector3.Cross(vertices[i], direction).z;
            if ( i < vertices.Length-2) { crossProduct2 = Vector3.Cross(vertices[i+1], direction).z; }
            else { crossProduct2 = Vector3.Cross(vertices[0], direction).z; }
            if ( (crossProduct1 > 0f && crossProduct2 < 0f) || 
                 (crossProduct1 < 0f && crossProduct2 > 0f))
            {
                indices[count].x = i;
                indices[count].y = i+1;
                count += 1;
            }
        }
        // Vector2 v1 = (vertices[(int)indices[0].x] - vertices[(int)indices[0].y]).normalized;
        // Vector2 v2 = (vertices[(int)indices[1].x] - vertices[(int)indices[1].y]).normalized;
        // Vector2 index;

        // float linedUp1 = Mathf.Abs(v1.x*direction.normalized.y - v1.y*direction.normalized.x);
        // float linedUp2 = Mathf.Abs(v2.x*direction.normalized.y - v2.y*direction.normalized.x);
        // Debug.Log(linedUp1);
        // Debug.Log(linedUp2);
        // Debug.Log(count);
        // if ( linedUp1 < linedUp2 )
        // { 
        //     index = new Vector2(indices[0].x, indices[0].y); 
        // }
        // else 
        // { 
        //     index = new Vector2(indices[0].x, indices[0].y);
        // }
        return indices;
    }

    public float GetPolygonArea(List<Vector3> vertices)
    {
        float A = 0f;
        for (int i = 0; i < vertices.Count-1; i++)
        {
            A += 0.5f * (vertices[i].x * vertices[i+1].y - vertices[i+1].x * vertices[i].y);
        }
        return Mathf.Abs(A);
    }

    // public float[] ComputeVertexHealth()
    // {
    //     // this.vertexHealth = new float[this.]
    // }

    public void DrawAsteroid(float size)
    {
        //random int including the starting number, excluding the finishing number
        int numberOfSides = Random.Range(8, 12);

        float radius = size / 2f;

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
        this.meshTriangles = meshTriangles;
        this.meshIndices = meshIndices;

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


    public void DrawMesh(Vector3[] vertices, int[] triangles)
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
 
        verticesReduced = new Vector3[vertices.Length-1];
        verticesReduced = verticesReducedList.ToArray();

        mesh2.vertices = verticesReduced;
        
        this.meshIndices = new int[verticesReduced.Length*2];

        for (int i = 0; i < verticesReduced.Length; i++)
        {
            this.meshIndices[2*i] = i;
            this.meshIndices[2*i+1] = i+1;
        }

        this.meshIndices[(verticesReduced.Length)*2-1] = 0;

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
