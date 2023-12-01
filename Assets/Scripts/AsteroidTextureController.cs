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
    Dictionary<Vector2, Vector2> hitboxVertexToTextureUV;
    int alreadyTextured = -1;
    float visualRoughness = 0.6f;
    float sqrt2 = 1.41421356237f;
    float cellSize;
    public float dxAsteroidCentreDrift;
    public float dyAsteroidCentreDrift;
    List<Vector2> meshVertices2D;
    float exchangeRate;

    void Start()
    {
        cellSize = Asteroid.celSize;
        texture = Resources.Load<Texture2D>("texture");
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material.mainTexture = texture;
        // hitboxVertexToVisualVertex = new Dictionary<Vector2, Vector2>();
        if (alreadyTextured == -1) {UpdateTexture();}
    }

    public void UpdateTexture(SquareMesh oldMesh = null)
    {
        alreadyTextured = 1;
        cellSize = Asteroid.celSize;
        SquareMesh squareMesh = this.transform.parent.GetComponent<Asteroid>().squareMesh;

        meshVertices2D = new List<Vector2>();
        foreach ( Vector2 v in squareMesh.perimeterVertices )
        {
            meshVertices2D.Add(v); // This for loop is necessary as we don't want to pass by reference!
        }
        
        if ( hitboxVertexToVisualVertex == null ) { hitboxVertexToVisualVertex = new Dictionary<Vector2, Vector2>();}
        


        Dictionary<Vector2, Vector2> oldDict = new Dictionary<Vector2, Vector2>();
        Dictionary<Vector2, Vector2> oldUVs = new Dictionary<Vector2, Vector2>();
        List<Vector2> oldVerticesWorldCoords = new List<Vector2>();
        Vector2 oldAsteroidWorldPosition = new Vector2(0f, 0f);
        if ( oldMesh != null )
        {
            oldDict = oldMesh.asteroid.gameObject.transform.Find("Texture").GetComponent<AsteroidTextureController>().hitboxVertexToVisualVertex;
            oldUVs = oldMesh.asteroid.gameObject.transform.Find("Texture").GetComponent<AsteroidTextureController>().hitboxVertexToTextureUV;
            List<Vector2> oldVertices = oldMesh.perimeterVertices;
            
            oldAsteroidWorldPosition = (Vector2)oldMesh.asteroid.gameObject.transform.position;
            // if ( oldMesh.asteroid.gameObject.name.Contains("Derived") )
            // {
            //     // Debug.Log("Was derived asteroid, finding the main");
            //     DerivedAsteroid tempAsteroid1 = (DerivedAsteroid)oldMesh.asteroid;
            //     oldAsteroidWorldPosition = tempAsteroid1.mainAsteroid.gameObject.transform.position;
            // }
            if ( oldAsteroidWorldPosition.x > oldMesh.asteroid.worldSize.x ) { oldAsteroidWorldPosition -= new Vector2( oldMesh.asteroid.worldSize.x * 2, 0f ); }

            foreach ( Vector2 v in oldVertices )
            {
                oldVerticesWorldCoords.Add( v + oldAsteroidWorldPosition );
            }
        }

        Vector2 newAsteroidWorldPosition = (Vector2)gameObject.transform.position;
        // if ( squareMesh.asteroid.gameObject.name.Contains("Derived") )
        // {
        //     // Debug.Log("Was derived asteroid, finding the main");
        //     DerivedAsteroid tempAsteroid2 = (DerivedAsteroid)squareMesh.asteroid;
        //     newAsteroidWorldPosition = tempAsteroid2.mainAsteroid.gameObject.transform.position;
        // }
        if (oldMesh != null)
        {
            for (int j = 0; j < meshVertices2D.Count; j++)
            {
                if ( oldVerticesWorldCoords.Count > 0 )
                {
                    Vector2 newVertexWorldPosition = newAsteroidWorldPosition + meshVertices2D[j];
                    if ( newVertexWorldPosition.x > oldMesh.asteroid.worldSize.x ) { newVertexWorldPosition -= new Vector2( oldMesh.asteroid.worldSize.x * 2, 0f ); }

                    for ( int n = 0; n < oldVerticesWorldCoords.Count; n++ )
                    {
                        Vector2 oldVertex = oldVerticesWorldCoords[n];

                        if ( (oldVertex - newVertexWorldPosition).magnitude <= sqrt2*visualRoughness*cellSize )
                        {
                            Vector2 oldVisualVertex = FindValueInDict(oldDict, oldVertex - oldAsteroidWorldPosition);
                            hitboxVertexToVisualVertex[meshVertices2D[j]] = meshVertices2D[j] + (oldVisualVertex - (oldVertex - oldAsteroidWorldPosition));
                            break;
                        }
                    }
                }
            }
        }
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
                Vector2 deltaVector = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)) * cellSize * visualRoughness;
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


        // Debug.Log("Calling ResetMesh");
        this.transform.parent.GetComponent<Asteroid>().squareMesh.ResetMesh(vertices3D);
        // foreach ( KeyValuePair<Vector2, GameObject> kvp in this.transform.parent.GetComponent<MainAsteroid>().derivedAsteroids )
        // {
        //     kvp.Value.GetComponent<DerivedAsteroid>().squareMesh.ResetMesh(vertices3D);
        // }



        // Debug.Log("Xs");
        // Debug.Log(minX);
        // Debug.Log(maxX);
        // Debug.Log("Ys");
        // Debug.Log(minY);
        // Debug.Log(maxY);
        
        Mesh mesh = new Mesh();
        meshVertices = vertices3D.ToArray();
        mesh.vertices = meshVertices;
        // Debug.Log(meshVertices.Length);

        meshTriangles = Triangulate(meshVertices2D).ToArray();

        mesh.triangles = meshTriangles;

        List<Vector2> UVs = new List<Vector2>();

        for ( int k = 0; k < meshVertices2D.Count; k++)
        {
            UVs.Add(new Vector2(0f, 0f));
        }

        Vector2[] UVsArray = UVs.ToArray(); 
        mesh.uv = UVsArray;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material.mainTexture = texture;

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        filter.mesh = mesh;

        MainAsteroid mainAstro;
 
        mainAstro = (MainAsteroid)this.gameObject.transform.parent.GetComponent<Asteroid>();

        // Debug.Log(mainAstro);
        // Debug.Log(mainAstro.derivedAsteroids);
        foreach ( KeyValuePair<Vector2, GameObject> kvp in mainAstro.derivedAsteroids )
        {
            MeshRenderer derivedAstroRenderer = kvp.Value.transform.Find("Texture").GetComponent<MeshRenderer>();
            derivedAstroRenderer.material.mainTexture = texture;
            MeshFilter derivedAstroMeshFilter = kvp.Value.transform.Find("Texture").GetComponent<MeshFilter>();
            derivedAstroMeshFilter.mesh = mesh;
        }

        
        // Debug.Log(meshTriangles.Length);

        // Will need to scale this so that max UV values are in range <0,1>
        // Will also be able to move the UVs inside the texture to give different asteroids different patches of the texture
        
        // scale = 1;
        // float currAsteroidToMaxAsteroidSizeRatio = (float)this.transform.parent.GetComponent<MainAsteroid>().size / (float)Reference.worldController.maxAsteroidSize;
        // float maxAsteroidDimension = Mathf.Max( ( Mathf.Abs(minX) + Mathf.Abs(maxX) ), ( Mathf.Abs(minY) + Mathf.Abs(maxY) ) );
        // float normalisation = maxAsteroidDimension * currAsteroidToMaxAsteroidSizeRatio;
        // float dxAsteroidCentreDrift = 0f; float dyAsteroidCentreDrift =  0f;

        // if ( oldMesh != null ) 
        // {
        //     dxAsteroidCentreDrift += (squareMesh.leftmostSplitCoord + oldMesh.leftmostSplitCoord)*cellSize;
        //     dyAsteroidCentreDrift += (squareMesh.bottomSplitCoord + oldMesh.bottomSplitCoord)*cellSize;
            
        //     squareMesh.leftmostSplitCoord += oldMesh.leftmostSplitCoord;
        //     squareMesh.bottomSplitCoord += oldMesh.bottomSplitCoord;
        // }

        // Debug.Log("Doing UVs---------------------------------------------------");


        // if ( oldMesh == null )
        // {   
        //     // foreach( Square sq in squareMesh.perimeterSquares )
        //     // {
        //         foreach ( Vector2 v2 in meshVertices2D )
        //         {
        //             // float xCoord = ( (v2.x + visualRoughness*cellSize + Mathf.Round( Mathf.Abs(minX)/(cellSize/2f) )/2f ) ) * 1f / 15f;
        //             // float yCoord = ( (v2.y + visualRoughness*cellSize + Mathf.Round( Mathf.Abs(minY)/(cellSize/2f) )/2f ) ) * 1f / 15f;
        //             Vector2 tempUV = meshVertices2D; //new Vector2( xCoord, yCoord );
        //             UVs.Add(tempUV);
        //         }
        //     // }
        // }

        // if ( oldMesh != null )
        // {
            
        // }


        // if ( oldMesh == null )
        // {
        //     if ( hitboxVertexToTextureUV == null ) 
        //     {
        //         // Calculate a fresh set of UVs for a brand new asteroid
        //         hitboxVertexToTextureUV = new Dictionary<Vector2, Vector2>();
        //         for ( int j = 0; j < meshVertices.GetLength(0); j++ )
        //         {
        //             if ( hitboxVertexToTextureUV.ContainsKey((Vector2)meshVertices[j]) == false )
        //             {
        //                 float xCoord = ( (((Vector2)meshVertices[j]).x + visualRoughness*cellSize + Mathf.Round( Mathf.Abs(minX)/(cellSize/2f) )/2f ) ) * 1f / 15f;
        //                 float yCoord = ( (((Vector2)meshVertices[j]).y + visualRoughness*cellSize + Mathf.Round( Mathf.Abs(minY)/(cellSize/2f) )/2f ) ) * 1f / 15f;
        //                 Vector2 tempUV = new Vector2( xCoord, yCoord );
        //                 // Debug.Log("Added UV for vertex");
        //                 // Debug.Log((Vector2)meshVertices[j]);
        //                 hitboxVertexToTextureUV.Add((Vector2)meshVertices[j], tempUV);
        //             }
        //         }
        //         Vector2 tempVdiff = (Vector2)meshVertices[0] - (Vector2)meshVertices[1];
        //         Vector2 tempUVdiff = hitboxVertexToTextureUV[(Vector2)meshVertices[0]] - hitboxVertexToTextureUV[(Vector2)meshVertices[1]];

        //         if ( tempVdiff.x != 0 ) { exchangeRate = tempUVdiff.x / tempVdiff.x; }
        //         if ( tempVdiff.y != 0 ) { exchangeRate = tempUVdiff.y / tempVdiff.y; }
        //     }
        //     else
        //     {
        //         for ( int j = 0; j < meshVertices.GetLength(0); j++ )
        //         {
        //             if ( hitboxVertexToTextureUV.ContainsKey((Vector2)meshVertices[j]) == false )
        //             {
        //                 int idx = j == 0 ? j+1 : j-1;

        //                 Vector2 otherUV = FindValueInDict(hitboxVertexToTextureUV, (Vector2)meshVertices[idx]);
 
        //                 Vector2 newUV = otherUV + ( (Vector2)meshVertices[j] - (Vector2)meshVertices[idx] ) * exchangeRate;
        //                 // Debug.Log(exchangeRate);
        //                 // Debug.Log(( (Vector2)meshVertices[j] - (Vector2)meshVertices[idx] ) * exchangeRate);

        //                 hitboxVertexToTextureUV.Add((Vector2)meshVertices[j], newUV);
        //             }
        //         }
        //     }
        // }
        // else
        // {
        //     if ( hitboxVertexToTextureUV != null ) { Debug.Log("dict already exists - unexpected"); }
        //     if ( hitboxVertexToTextureUV == null ) { hitboxVertexToTextureUV = new Dictionary<Vector2, Vector2>(); }
        //     if ( exchangeRate == 0f ) { exchangeRate = oldMesh.asteroid.gameObject.transform.Find("Texture").GetComponent<AsteroidTextureController>().exchangeRate; Debug.Log(exchangeRate);}
        //     newAsteroidWorldPosition = (Vector2)squareMesh.asteroid.gameObject.transform.position;
            
        //     if ( newAsteroidWorldPosition.x > oldMesh.asteroid.worldSize.x ) { newAsteroidWorldPosition -= new Vector2( oldMesh.asteroid.worldSize.x * 2, 0f ); }
        //     oldAsteroidWorldPosition = (Vector2)oldMesh.asteroid.gameObject.transform.position;
        //     if ( oldAsteroidWorldPosition.x > oldMesh.asteroid.worldSize.x ) { oldAsteroidWorldPosition -= new Vector2( oldMesh.asteroid.worldSize.x * 2, 0f ); }
        //     // Debug.Log(newAsteroidWorldPosition);
        //     // Debug.Log(oldAsteroidWorldPosition);
        //     for ( int j = 0; j < meshVertices.GetLength(0); j++ )
        //     {
        //         Vector2 newVertexWorldPosition = newAsteroidWorldPosition + (Vector2)meshVertices[j];
        //         // Debug.Log("Looking for a UV for new vertex:");
        //         // Debug.Log(newVertexWorldPosition);
        //         for ( int n = 0; n < oldVerticesWorldCoords.Count; n++ )
        //         {
        //             Vector2 oldVertex = oldVerticesWorldCoords[n];
        //             // Debug.Log("Testing old vertex:");
        //             // Debug.Log(oldVertex);

        //             if ( (oldVertex - newVertexWorldPosition).magnitude <= sqrt2*visualRoughness*cellSize )
        //             {
        //                 Vector2 oldUV = FindValueInDict(oldUVs, oldVertex - oldAsteroidWorldPosition);

        //                 if ( hitboxVertexToTextureUV.ContainsKey((Vector2)meshVertices[j]) == false )
        //                 {
        //                     hitboxVertexToTextureUV.Add((Vector2)meshVertices[j], oldUV);
        //                     // Debug.Log("Found suitable old vertex!");
        //                     // Debug.Log("Associated UV was:");
        //                     // Debug.Log(oldUV);
        //                     // Debug.Log(exchangeRate);
        //                 }
        //                 else
        //                 { 
        //                     hitboxVertexToTextureUV.Remove((Vector2)meshVertices[j]);
        //                     hitboxVertexToTextureUV.Add((Vector2)meshVertices[j], oldUV);
        //                     Debug.Log("Already had a UV for this vertex, had to re-add");     
        //                 }
        //                 break;
        //             }
        //             else
        //             {
                        
        //                 if( n == oldVerticesWorldCoords.Count - 1 ) 
        //                 { 
        //                     Debug.Log("Have to make up a vertex"); 

        //                     int idx = j == 0 ? j+1 : j-1;

        //                     Vector2 otherUV = FindValueInDict(hitboxVertexToTextureUV, (Vector2)meshVertices[idx]);
    
        //                     Vector2 newUV = otherUV + ( (Vector2)meshVertices[j] - (Vector2)meshVertices[idx] ) * exchangeRate;

        //                     hitboxVertexToTextureUV.Add((Vector2)meshVertices[j], newUV);
        //                 }
        //             }
        //         }
        //     }
        // }


        // for ( int j = 0; j < meshVertices.GetLength(0); j++ )
        // {
        //     if ( hitboxVertexToTextureUV.ContainsKey((Vector2)meshVertices[j]) )
        //     {
        //         UVs.Add(hitboxVertexToTextureUV[(Vector2)meshVertices[j]]);
        //     }
        //     else
        //     {
        //         Debug.Log("No UV for the givn key");
        //     }
        // }


        // if ( oldMesh == null )
        // {
        //     foreach ( Vector2 v2 in meshVertices2D )
        //     {
        //         // Map asteroid's -1-to-1 (middle of the asteroid coordinates) onto 0-to-1 (left bottom corner convention for textures)
        //         float xCoord = ( (v2.x + dxAsteroidCentreDrift + squareMesh.emptyCols*cellSize + visualRoughness*cellSize + Mathf.Round( Mathf.Abs(minX)/(cellSize/2f) )/2f ) ) * 1f / 15f;
        //         float yCoord = ( (v2.y + dyAsteroidCentreDrift + squareMesh.emptyRows*cellSize + visualRoughness*cellSize + Mathf.Round( Mathf.Abs(minY)/(cellSize/2f) )/2f ) ) * 1f / 15f;
        //         Vector2 tempV2 = new Vector2( xCoord, yCoord );
        //         UVs.Add(tempV2);
        //         if ( hitboxVertexToTextureUV.ContainsKey(v2 == false )
        //         {
        //             hitboxVertexToTextureUV.Add(v2, tempV2);
        //         }
            
        //     }
        // }
        // else
        // {
        //     foreach (Vector3 v3 in meshVertices)
        //     {
        //         // Need to do similar world-space vertex identification as I did previosuly for shapes!
        //         Vector2 oldUV = FindValueInDict(oldUVs, (Vector2)v3);
        //         Vector2 tempV2 = new Vector2( oldUV.x, oldUV.y );
        //         if ( hitboxVertexToTextureUV.ContainsKey((Vector2)v3) == false )
        //         {
        //             hitboxVertexToTextureUV.Add((Vector2)v3, tempV2);
        //         }
        //         UVs.Add(tempV2);
        //     }
        // }
        
    }

    Vector2 FindValueInDict(Dictionary<Vector2, Vector2> dict, Vector2 key)
    {
        foreach ( KeyValuePair<Vector2, Vector2> kvp in dict ) 
        { 
            if ( (kvp.Key - key).magnitude < visualRoughness*sqrt2*cellSize ) { return kvp.Value; }
        }
        Debug.Log("Returning zero");
        return new Vector2(0f, 0f);
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
