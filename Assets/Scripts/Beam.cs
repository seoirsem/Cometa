using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    Ray beamRay;
    LineRenderer lineRenderer;
    float maxRayCastDistance;
    float rayCastDistance;
    float rayCastAdvanceSpeed;
    float rayCastRetractSpeed;
    float beamPower;
    float maxBeamPower;
    float beamDischargeRate;
    float beamChargeRate;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        maxRayCastDistance = 3f;
        maxBeamPower = 100f;
        beamPower = 100f;
        beamDischargeRate = 0.1f;
        beamChargeRate = 0.01f;
        rayCastAdvanceSpeed = 0.1f;
        rayCastRetractSpeed = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if ( beamPower < 100f ){ beamPower += beamChargeRate; }
        
        float beamAngle = Reference.playergo.GetComponent<Rigidbody2D>().rotation % 360; // Rigidbody angle winds above 360 so take modulo
        if ( beamAngle < 0 ){ beamAngle = (360f + beamAngle); } // Convert from -180:180 to 0:360 convention
        beamAngle = 2f*Mathf.PI*beamAngle/(360f); // Convert to radians for Mathf. trig functions

        if ( (Reference.playerInputController.b == false || beamPower < 5f) && rayCastDistance >= rayCastRetractSpeed) 
        { 
            rayCastDistance -= rayCastRetractSpeed;  // debugging tool
            if ( lineRenderer.positionCount > 2 )  // debugging tool
            {  // debugging tool
                Vector3 direction = lineRenderer.GetPosition(2) - lineRenderer.GetPosition(3); // debugging tool
                lineRenderer.SetPosition(3, lineRenderer.GetPosition(3) + direction.normalized*rayCastRetractSpeed); // debugging tool
                if ( direction.magnitude < rayCastRetractSpeed ) { lineRenderer.positionCount = 2; } // debugging tool
            }
        }
        if ( Reference.playerInputController.b == true && beamPower > 0f ) 
        {   
            beamPower -= beamDischargeRate; 
            if ( rayCastDistance < maxRayCastDistance ) { rayCastDistance += rayCastAdvanceSpeed; }
        }
          
        Vector3 beamDirection = new Vector3(-Mathf.Sin(beamAngle)*rayCastDistance, Mathf.Cos(beamAngle)*rayCastDistance, 0f);   
        lineRenderer.SetPosition(0, Reference.playergo.transform.position); // debugging tool
        lineRenderer.SetPosition(1, Reference.playergo.transform.position + beamDirection); // debugging tool
        if ( Reference.playerInputController.b == true && beamPower > 0f )
        {
            DoRaycast(Reference.playergo.transform.position, beamDirection, rayCastDistance, 1);
        }   
    }

    void DoRaycast(Vector3 from, Vector3 direction, float distance, int depth)
    {
        RaycastHit2D hit;
        bool crossScreenBeam = false;
        if ( distance > 0f )
        {
            hit = Physics2D.Raycast(from, direction, distance);
            if (hit.collider != null)
            {
                Debug.Log(crossScreenBeam);
                Debug.Log(depth);
                if ( hit.collider.gameObject.name.Contains("EdgeCollider") ) 
                { 
                    crossScreenBeam = true;
                    if ( lineRenderer.positionCount < 4) { lineRenderer.positionCount += 2;} // debugging tool
                    float leftoverDisatnce = distance - ((Vector2)from - hit.point).magnitude;
                    Vector2 worldSize = Reference.worldController.worldSize;
                    if ( hit.collider.gameObject.name.Contains("Right") ) 
                    { 
                        DoRaycast(hit.point - new Vector2(worldSize.x*0.999f, 0f), direction, leftoverDisatnce, depth+1); 
                        lineRenderer.SetPosition(2, hit.point - new Vector2(worldSize.x*0.999f, 0f)); // debugging tool
                        lineRenderer.SetPosition(3, hit.point - new Vector2(worldSize.x*0.999f, 0f) + (Vector2)direction.normalized*leftoverDisatnce); // debugging tool
                    }
                    if ( hit.collider.gameObject.name.Contains("Left") ) 
                    { 
                        DoRaycast(hit.point + new Vector2(worldSize.x*0.999f, 0f), direction, leftoverDisatnce, depth+1); 
                        lineRenderer.SetPosition(2, hit.point + new Vector2(worldSize.x*0.999f, 0f)); // debugging tool
                        lineRenderer.SetPosition(3, hit.point + new Vector2(worldSize.x*0.999f, 0f) + (Vector2)direction.normalized*leftoverDisatnce); // debugging tool
                    }
                    if ( hit.collider.gameObject.name.Contains("Top") ) 
                    { 
                        DoRaycast(hit.point - new Vector2(0f, worldSize.y*0.999f), direction, leftoverDisatnce, depth+1);
                        Debug.Log(hit.point); 
                        lineRenderer.SetPosition(2, hit.point - new Vector2(0f, worldSize.y*0.999f)); // debugging tool
                        lineRenderer.SetPosition(3, hit.point - new Vector2(0f, worldSize.y*0.999f) + (Vector2)direction.normalized*leftoverDisatnce); // debugging tool
                    }
                    if ( hit.collider.gameObject.name.Contains("Bottom") ) 
                    { 
                        DoRaycast(hit.point + new Vector2(0f, worldSize.y*0.999f), direction, leftoverDisatnce, depth+1); 
                        lineRenderer.SetPosition(2, hit.point + new Vector2(0f, worldSize.y*0.999f)); // debugging tool
                        lineRenderer.SetPosition(3, hit.point + new Vector2(0f, worldSize.y*0.999f) + (Vector2)direction.normalized*leftoverDisatnce); // debugging tool
                    }
                }
                else{ ResolveRayhits(hit); } 
            }
            if (crossScreenBeam == false && depth == 1) { lineRenderer.positionCount = 2; } // debugging tool
        }
    }

    void ResolveRayhits(RaycastHit2D hit)
    {
        if ( hit.collider.gameObject.name.Contains("MainAsteroid") )
        {
            Asteroid asteroid = hit.collider.gameObject.GetComponent<Asteroid>();
            ShrinkVertex(asteroid, hit.point - (Vector2)asteroid.gameObject.transform.position);
        }
        else 
        {
            Debug.Log("Raycast hit object:");
            Debug.Log(hit.collider.gameObject.name);
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
