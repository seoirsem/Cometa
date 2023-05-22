using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidTextureController : MonoBehaviour
{

    // Ear clipping algorithm goes a bit bananas when you have several points on the same straight line in your polygon
    // So at the moment just have a dirty safety break on the while loop. In the future need some nice detection of zero-area triangles. 

    Vector3[] meshVertices;
    int[] meshTriangles;
    Texture2D texture;
    float scale;
    Dictionary<Vector2, Vector2> hitboxVertexToVisualVertex;

    void Start()
    {
        texture = Resources.Load<Texture2D>("texture");
        // hitboxVertexToVisualVertex = new Dictionary<Vector2, Vector2>();
        UpdateTexture();
    }

    public void UpdateTexture()
    {
        List<Vector2> meshVertices2D = this.transform.parent.GetComponent<MainAsteroid>().squareMesh.perimeterVertices;
        
        if ( hitboxVertexToVisualVertex == null ) { hitboxVertexToVisualVertex = new Dictionary<Vector2, Vector2>();}
        float cellSize = Asteroid.celSize;
        for (int j = 0; j < meshVertices2D.Count; j++)
        {
            
            if ( hitboxVertexToVisualVertex.ContainsKey(meshVertices2D[j]) )
            {
                // Debug.Log("Already in dict");
                meshVertices2D[j] = hitboxVertexToVisualVertex[meshVertices2D[j]];
            }
            else
            {
                Vector3 localVector = (Vector3)meshVertices2D[j];
                Reference.animationController.SpawnDustCloudAnimationOnBulletHit(this.transform.position + localVector);
                float asteroidMass = this.transform.parent.GetComponent<Asteroid>().squareMesh.mass;
                float asteroidMaxMass = (float)Reference.worldController.maxAsteroidSize*(float)Reference.worldController.maxAsteroidSize;
                float currAsteroidToMaxAsteroidMassRatio = asteroidMass/asteroidMaxMass;
                Vector2 deltaVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * cellSize * 0.3f;
                hitboxVertexToVisualVertex.Add(meshVertices2D[j], meshVertices2D[j] + deltaVector);
                meshVertices2D[j] = meshVertices2D[j] + deltaVector;
            }
        }

        meshVertices2D = RemoveColinearVertices(meshVertices2D);

        List<Vector3> vertices3D = new List<Vector3>();
        int i = 0;
        float minX = float.PositiveInfinity;
        float minY = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float maxY = float.NegativeInfinity;
        foreach ( Vector3 v2 in meshVertices2D )
        {
            if ( v2.x < minX ) { minX = v2.x; }
            if ( v2.y < minY ) { minY = v2.y; }
            if ( v2.x > maxX ) { maxX = v2.x; }
            if ( v2.y > maxY ) { maxY = v2.y; }
            vertices3D.Add( new Vector3(v2.x, v2.y, 0.0f) );

            i += 1;
        }
        
        Mesh mesh = new Mesh();
        meshVertices = vertices3D.ToArray();
        mesh.vertices = meshVertices;
        // Debug.Log(meshVertices.Length);

        meshTriangles = Triangulate(meshVertices2D).ToArray();
        // int[] triangles = new int[] { 0, meshVertices2D.Count/4, meshVertices2D.Count*2/4, meshVertices2D.Count*2/4, meshVertices2D.Count*3/4, 0 };
        mesh.triangles = meshTriangles;

        
        // Debug.Log(meshTriangles.Length);

        // Will need to scale this so that max UV values are in range <0,1>
        // Will also be able to move the UVs inside the texture to give different asteroids different patches of the texture
        List<Vector2> UVs = new List<Vector2>();
        scale = 1;
        float currAsteroidToMaxAsteroidSizeRatio = (float)this.transform.parent.GetComponent<MainAsteroid>().size / (float)Reference.worldController.maxAsteroidSize;
        float maxAsteroidDimension = Mathf.Max( ( Mathf.Abs(minX) + Mathf.Abs(maxX) ), ( Mathf.Abs(minY) + Mathf.Abs(maxY) ) );
        float normalisation = maxAsteroidDimension * currAsteroidToMaxAsteroidSizeRatio;
        foreach ( Vector3 v3 in meshVertices )
        {
            // Map asteroid's -1-to-1 (middle of the asteroid coordinates) onto 0-to-1 (left bottom corner convention for textures)
            float xCoord = ( (v3.x + Mathf.Abs(minX)) / maxAsteroidDimension ) * currAsteroidToMaxAsteroidSizeRatio / 3;
            float yCoord = ( (v3.y + Mathf.Abs(minY)) / maxAsteroidDimension ) * currAsteroidToMaxAsteroidSizeRatio / 3;
            Vector2 tempV2 = new Vector2( xCoord, yCoord );
            UVs.Add(tempV2);
        }
        Vector2[] uv = UVs.ToArray(); 
        // Debug.Log(uv.Length);
        mesh.uv = uv;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material.mainTexture = texture;

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        filter.mesh = mesh;
    }


    public List<int> Triangulate(List<Vector2> vertices)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < vertices.Count; i++)
        {
            indices.Add(i);
        }

        List<int> triangles = new List<int>();

        int safety = 0;
        while (indices.Count > 3)
        {
            safety += 1;
            if ( safety > 990)
            {
                Debug.Log(indices.Count);
                Debug.Log(vertices[indices[indices.Count-1]]);
                Debug.Log(vertices[0]);
                Debug.Log(vertices[indices[1]]);
            }
            if ( safety > 1000 ) { Debug.Log("Breaking a safety loop"); break; }

            for (int i = 0; i < indices.Count; i++)
            {
                int prevIndex = (i == 0) ? indices.Count - 1 : i - 1;
                int nextIndex = (i == indices.Count - 1) ? 0 : i + 1;

                Vector2 prevVertex = vertices[indices[prevIndex]];
                Vector2 currVertex = vertices[indices[i]];
                Vector2 nextVertex = vertices[indices[nextIndex]];

                if (IsEar(prevVertex, currVertex, nextVertex, vertices))
                {
                    triangles.Add(indices[prevIndex]);
                    triangles.Add(indices[i]);
                    triangles.Add(indices[nextIndex]);
                    indices.RemoveAt(i);
                    break;
                }
            }
        }

        // the remaining indices form a single triangle
        if ( indices.Count == 3 )
        {
            triangles.Add(indices[0]);
            triangles.Add(indices[1]);
            triangles.Add(indices[2]);
        }
        else if ( triangles.Count != 3 ) { Debug.Log("weird number of vertices left!");}
        
        return triangles;
    }

    List<Vector2> RemoveColinearVertices(List<Vector2> verticesRaw)
    {
        List<Vector2> noColinearVertices = new List<Vector2>();
        verticesRaw.Add(verticesRaw[0]);
        verticesRaw.Add(verticesRaw[1]);

        for (int i = 0; i < verticesRaw.Count - 2 ; i++)
        {
            if ( ArePointsCollinear(verticesRaw[i], verticesRaw[i+1], verticesRaw[i+2]) == false)
            {
                noColinearVertices.Add(verticesRaw[i+1]);
            }
        }
        return noColinearVertices;
    }

    public bool ArePointsCollinear(Vector2 a, Vector2 b, Vector2 c)
    {
        float slopeAB = (b.y - a.y) / (b.x - a.x);
        float slopeBC = (c.y - b.y) / (c.x - b.x);

        // Check if the slopes are equal (within a small threshold)
        float threshold = 0.0001f;
        bool equalSlopes = Mathf.Abs(slopeAB - slopeBC) < threshold;

        return equalSlopes;
    }

    float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
    {
        float abX = b.x - a.x;
        float abY = b.y - a.y;
        float acX = c.x - a.x;
        float acY = c.y - a.y;
        float crossProduct = abX * acY - abY * acX;
        float area = 0.5f * Mathf.Abs(crossProduct);
        return area;
    }

    bool IsEar(Vector2 a, Vector2 b, Vector2 c, List<Vector2> polygon)
    {
        // Check if the angle between the edges is concave
        Vector2 ab = b - a;
        Vector2 ac = c - a;
        float cross = ab.x * ac.y - ab.y * ac.x;

        if (cross > 0)
        {
            return false;
        }

        // Check if any vertex of the polygon lies inside the triangle
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 p = polygon[i];

            if (p == a || p == b || p == c)
            {
                continue;
            }

            if (PointInTriangle(p, a, b, c))
            {
                return false;
            }
        }

        return true;
    }

    bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        bool b1, b2, b3;

        b1 = Sign(p, a, b) < 0.0f;
        b2 = Sign(p, b, c) < 0.0f;
        b3 = Sign(p, c, a) < 0.0f;

        return ((b1 == b2) && (b2 == b3));
    }

    float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return ((p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y));
    }
}
