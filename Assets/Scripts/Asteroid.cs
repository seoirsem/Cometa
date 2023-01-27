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

    public SquareMesh squareMesh;




    public void DrawAsteroid(float size)
    {
        squareMesh = new SquareMesh();

        squareMesh.edgeLength = 0.2f;
        squareMesh.SetAsteroid(this);

        polygonCollider = this.gameObject.GetComponent<PolygonCollider2D>();
        squareMesh.GenerateMesh(20);
        squareMesh.FindOutline();
        squareMesh.ScaleEdgeLength();
        squareMesh.ResetMesh();
        squareMesh.ResetColliderMesh();
    }

    public void DrawDerivedAsteroid()
    {
        squareMesh = new SquareMesh();
        squareMesh.edgeLength = 0.2f;
        squareMesh.SetAsteroid(this);
        polygonCollider = this.gameObject.GetComponent<PolygonCollider2D>();
    }


    public List<Vector3> CalculateCOM(List<Vector3> vertices, float sign = 1f)
    {
        vertices.Add(vertices[0]);
        float A = 0f;
        float Cx = 0f;
        float Cy = 0f;


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


    public float GetPolygonArea(List<Vector3> vertices)
    {
        float A = 0f;
        for (int i = 0; i < vertices.Count-1; i++)
        {
            A += 0.5f * (vertices[i].x * vertices[i+1].y - vertices[i+1].x * vertices[i].y);
        }
        return Mathf.Abs(A);
    }

    // public static bool operator == (Asteroid d1, Asteroid d2)
    //  {
    //     // If both are null, or both are same instance, return true.
    //     if (System.Object.ReferenceEquals(d1, d2))
    //     {
    //         return true;
    //     }

    //     // If one is null, but not both, return false.
    //     if (((object)d1 == null) || ((object)d2 == null))
    //     {
    //         return false;
    //     }

    //     // Return true if the fields match:
    //     return d1.asteroidOutlines == d2.asteroidOutlines && d1.location == d2.location && d1.asteroidgo == d2.asteroidgo;
    //  }

    // public override bool Equals(System.Object obj)
    //  {
    //     // If parameter cannot be cast to ThreeDPoint return false:
    //     Asteroid d = obj as Asteroid;
    //     if ((object)d == null)
    //     {
    //         return false;
    //     }
        
    //     // Return true if the fields match:
    //     return asteroidOutlines == d.asteroidOutlines && location == d.location && asteroidgo == d.asteroidgo;
    //  }

    // public static bool operator !=(Asteroid d1, Asteroid d2)
    // {
    //     return !(d1 == d2);
    // }



}
