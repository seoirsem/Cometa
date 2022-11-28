using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    Ray beamRay;
    RaycastHit2D hit;
    LineRenderer lineRenderer;
    float rayCastDistance;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        rayCastDistance = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        
        lineRenderer.SetPosition(0, Reference.playergo.transform.position);
        float beamAngle = Reference.playergo.GetComponent<Rigidbody2D>().rotation % 360; // Rigidbody angle winds above 360 so take modulo
        if ( beamAngle < 0 ){ beamAngle = (360f + beamAngle); } // Convert from -180:180 to 0:360 convention
        beamAngle = 2f*Mathf.PI*beamAngle/(360f); // Convert to radians for Mathf. trig functions
        Vector3 beamDirection = new Vector3(-Mathf.Sin(beamAngle)*rayCastDistance, Mathf.Cos(beamAngle)*rayCastDistance, 0f);   
        lineRenderer.SetPosition(1, Reference.playergo.transform.position + beamDirection);
        hit = Physics2D.Raycast(Reference.playergo.transform.position, beamDirection, rayCastDistance);
        if (hit.collider != null)
        {
            if ( hit.collider.gameObject.name.Contains("MainAsteroid") )
            {
                Asteroid asteroid = hit.collider.gameObject.GetComponent<Asteroid>();
                ShrinkVertex(asteroid, hit.point - (Vector2)asteroid.gameObject.transform.position);
                //asteroid.meshVertices[asteroid.meshVertices.Length-1]
            }
        }
        else
        {
            // Debug.Log("Didn't hit anything!");;
        }
    }

    void ShrinkVertex(Asteroid asteroid, Vector2 hitPoint)
    {
        float minDif = Mathf.Infinity;
        int minIdx = -1;
        for (int i = 0; i < asteroid.meshVertices.Length; i++ )
        {
            float dif = ((Vector2)asteroid.meshVertices[i] - hitPoint).magnitude;
            if ( ( dif < minDif ) ){ minIdx = i; minDif = dif; }
        }
        asteroid.meshVertices[minIdx] = asteroid.meshVertices[minIdx]*0.99f;
        // I use the CalculateCOM f'n to get the coordinates of the new (post-shrink) COM
        // Then shift COM (asteroid position; we always keep COM at (0,0) in local coords)
        // And all vertices in the opposite direction to compensate the GO movement 
        List<Vector3> tempVertexList = new List<Vector3>(asteroid.meshVertices);
        tempVertexList = asteroid.CalculateCOM(tempVertexList);

        for ( int j = 0; j < asteroid.meshVertices.Length; j++ )
        {
            asteroid.meshVertices[j] -= tempVertexList[tempVertexList.Count-1];
        }
        asteroid.gameObject.transform.position += tempVertexList[tempVertexList.Count-1];
        asteroid.DrawCollider(asteroid.meshVertices, asteroid.meshTriangles);
        foreach (GameObject asteroidGO in asteroid.asteroidPack)
        {
            asteroidGO.GetComponent<Asteroid>().DrawMesh(asteroid.meshVertices, asteroid.meshTriangles);
        }
    }
}
