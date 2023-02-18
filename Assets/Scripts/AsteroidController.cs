using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    GameObject mainAsteroidPrefab;
    GameObject deriveAsteroidPrefab;
    List<GameObject> asteroids = new List<GameObject>();
    List<List<GameObject>> asteroidSets = new List<List<GameObject>>();
    Vector2 worldSize;

    Vector3 dummy1 = new Vector3(1,1,1);
    Vector3[] dummy2 = new Vector3[1];
    


    void Start()
    {
        mainAsteroidPrefab = Resources.Load("Prefabs/MainAsteroid") as GameObject;
        deriveAsteroidPrefab = Resources.Load("Prefabs/DerivedAsteroid") as GameObject;
        worldSize = Reference.worldController.worldSize;

        SpawnAsteroid(20, new Vector3(0f, 0f, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, null, false, new Vector2(0,0)); 
    }

    void Update()
    {

        if(Reference.playerInputController.o)
        { //clear screen
            foreach(Transform child in this.transform)
            {
                GameObject.Destroy(child.gameObject);
                List<GameObject> asteroids = new List<GameObject>();
                List<List<GameObject>> asteroidSets = new List<List<GameObject>>();
            }
        }

    }
    


    public void SpawnNewAsteroid(int size, int directionIndex, Vector3 asteroidPositionOffset, Vector3 velocity)
    {
        /// velocity
        int[] numbers = {-1,0,1,0,0,1,0,-1};
//        int directionIndex = Random.Range(0,3);
        Vector3 directionOrientation = new Vector3(numbers[directionIndex*2],numbers[directionIndex*2+1],0);
        float magnitude = Random.Range(1f,5f);
        Vector3 direction = -1 * magnitude*directionOrientation;

        Vector3 position = new Vector3(0,0,0);
        if(directionIndex == 0)
        {// left
            position = new Vector3(-worldSize.x/2f - 0.55f*size*Asteroid.celSize, -0.0f*size*Asteroid.celSize,0);
            direction += new Vector3(0,Random.Range(-0.4f,0.4f),0);
        }
        if(directionIndex == 1)
        {// right
            position = new Vector3(worldSize.x/2f + 0.55f*size*Asteroid.celSize, -0.0f*size*Asteroid.celSize,0);
            direction += new Vector3(0,Random.Range(-0.4f,0.4f),0);
        }
        if(directionIndex == 2)
        {// up
            position = new Vector3(-0.0f*size*Asteroid.celSize,worldSize.y/2 + 0.55f*size*Asteroid.celSize,0);
            direction += new Vector3(Random.Range(-0.4f,0.4f),0,0);
        }
        if(directionIndex == 3)
        {// down
            position = new Vector3(-0.0f*size*Asteroid.celSize,-worldSize.y/2 - 0.55f*size*Asteroid.celSize,0);
            direction += new Vector3(Random.Range(-0.4f,0.4f),0,0);
        }

        /// position
        //Vector3 position = new Vector3(directionOrientation.x*(worldSize.x/2 - size*Asteroid.celSize),directionOrientation.y*(worldSize.y/2 - size*Asteroid.celSize),0);
        if(velocity != new Vector3(0,0,0))
        {
            direction = velocity;
        }
        SpawnAsteroid(size, position + asteroidPositionOffset, direction, new Vector3(0, 0, 0), GaussianRandom.generateNormalRandom(0,30f), null, true, directionOrientation);    
    }

    public void SpawnRandomAsteroid(int size, Vector3 position)
    {
        SpawnAsteroid(size, position, new Vector3(0,0,0), new Vector3(0, 0, 0), Random.Range(-20f,20f), null, false, new Vector2(0,0));    
    }

    public void AsteroidHit(Asteroid asteroid, Vector2 contact, GameObject otherObject, List<GameObject> asteroidPack, List<SquareMesh> newAstroidMeshes, int numberOfSquaresInAsteroid, Vector3 offsetFromActualCollision = new Vector3())
    {
        int numberOfSquaresLost = numberOfSquaresInAsteroid;
        Reference.soundController.RockDestroy();
        
        if ( newAstroidMeshes == null ) 
        {
            Debug.Log("Returning as list of meshes itself is null"); 
            // Just copy geometry to the derived clones and return, no splitting/spawning
            numberOfSquaresLost -= asteroid.squareMesh.NumberOfSquaresInMesh();
            // (MainAsteroid)asteroid.CloneGeometryToDerivedAsteroids(); 
        }
        else
        {
            Vector3 preSplitPosition = asteroid.gameObject.transform.position;
            Vector3 preSplitVelocity = (Vector3)asteroid.rigid_body.velocity;
            Vector3 preSplitEulerAngles = asteroid.gameObject.transform.eulerAngles;
            float preSplitAngularVelocity = asteroid.rigid_body.angularVelocity;

            float asteroidRotation = asteroid.gameObject.transform.rotation.eulerAngles.z * Mathf.PI/180f;
            float cornerOffsetX = ((float)asteroid.squareMesh.squares.GetLength(0)) * asteroid.squareMesh.edgeLength/2f;
            float cornerOffsetY = ((float)asteroid.squareMesh.squares.GetLength(1)) * asteroid.squareMesh.edgeLength/2f;
            float rotatedCornerOffsetX = cornerOffsetX * Mathf.Cos(asteroidRotation) - cornerOffsetY * Mathf.Sin(asteroidRotation);
            float rotatedCornerOffsetY = cornerOffsetX * Mathf.Sin(asteroidRotation) + cornerOffsetY * Mathf.Cos(asteroidRotation);
            // Debug.Log(preSplitPosition);
            // Debug.Log(asteroidRotation);
            // Debug.Log(asteroid.squareMesh.squares.GetLength(0));
            // Debug.Log(asteroid.squareMesh.squares.GetLength(1));
            // Debug.Log(rotatedCornerOffsetX);
            // Debug.Log(rotatedCornerOffsetY);
            Vector3 asteroidCornerInWC = preSplitPosition - new Vector3( rotatedCornerOffsetX, rotatedCornerOffsetY, 0f );

            DespawnAsteroid(asteroid,asteroidPack);

            // Check if all meshes are null; this would mean there was a split but all sub-asteroids are destroyed
            bool allNullFlag = true;
            foreach ( SquareMesh squareMesh in newAstroidMeshes )
            {
                if ( squareMesh != null ) { allNullFlag = false; }
            }

            if ( allNullFlag == true ) 
            { 
                // Debug.Log("All meshes are null"); 
                numberOfSquaresLost -= asteroid.squareMesh.NumberOfSquaresInMesh();
            } 
            else 
            {
                // Debug.Log("Non-null meshes present - spawning chunks");
                foreach(SquareMesh squareMesh in newAstroidMeshes)
                {        
                    // If the mesh is null, then this asteroid fragment was completely destroyed
                    if ( squareMesh != null )
                    {
                        // Debug.Log("Spawning an asteroid from a new mesh");
                        // Debug.Log(asteroidCornerInWC);
//                        Debug.Log(squareMesh.leftmostSplitCoord);
//                        Debug.Log(squareMesh.rightmostSplitCoord);
                        // Debug.Log(squareMesh.leftmostSplitCoord + squareMesh.rightmostSplitCoord);
                        // Debug.Log((squareMesh.leftmostSplitCoord + squareMesh.rightmostSplitCoord) * squareMesh.edgeLength/2f);
//                        Debug.Log(squareMesh.bottomSplitCoord);
//                        Debug.Log(squareMesh.topSplitCoord);
                        
                        // Debug.Log(squareMesh.topSplitCoord + squareMesh.bottomSplitCoord);
                        // Debug.Log((squareMesh.topSplitCoord + squareMesh.bottomSplitCoord) * squareMesh.edgeLength/2f);
                        float splitOffsetX = (float)(squareMesh.leftmostSplitCoord + squareMesh.rightmostSplitCoord+1) * squareMesh.edgeLength/2f;
                        float splitOffsetY = (float)(squareMesh.topSplitCoord+1 + squareMesh.bottomSplitCoord) * squareMesh.edgeLength/2f;
                               
                        float rotatedSplitOffsetX = splitOffsetX * Mathf.Cos(asteroidRotation) - splitOffsetY * Mathf.Sin(asteroidRotation);
                        float rotatedSplitOffsetY = splitOffsetX * Mathf.Sin(asteroidRotation) + splitOffsetY * Mathf.Cos(asteroidRotation);

                        Vector3 splitOffset = new Vector3(rotatedSplitOffsetX, rotatedSplitOffsetY, 0f);
                        Vector3 postSplitPosition = asteroidCornerInWC + splitOffset;
//                        Debug.Log(asteroidCornerInWC);
//                        Debug.Log(splitOffset);
//                        Debug.Log(asteroidCornerInWC + splitOffset);
                        // Debug.Break();
                        SpawnAsteroid(40, postSplitPosition, preSplitVelocity, preSplitEulerAngles, preSplitAngularVelocity, squareMesh, false, new Vector2(0,0));
                        numberOfSquaresLost -= squareMesh.NumberOfSquaresInMesh();
                    }
                }
            }  
        }

        Reference.scoreController.IncrementScore((float)numberOfSquaresLost, contact);
        // Reference.hudController.ScoreText(contact, numberOfSquaresLost, new Color(255,215,0));
        if(otherObject.GetComponent<ShipShields>() != null)
        {
            otherObject.GetComponent<ShipShields>().ShieldsDestroyedAsteroidSquares(numberOfSquaresLost,contact);
        }
    }
    
    public void AsteroidAstroidCollision(Asteroid asteroid, Vector2 contact, List<GameObject> asteroidPack)
    {
        Vector3 asteroidPosition = asteroid.gameObject.transform.position;
        Vector3 asteroidVelocity = asteroid.velocity;
        Vector3 collisionPoint = new Vector3(contact.x,contact.y,0);//transform.position;
        Vector3 collisionDirection = (collisionPoint - asteroidPosition).normalized;

        // this is a fixed reference direction for deciding which asteroid triggers the impact
        Vector3 referenceDirection = new Vector3(Reference.worldController.worldSize.x, Reference.worldController.worldSize.y, 0);

        if (Vector3.Dot(collisionDirection,referenceDirection) > 0)//only one of the asteroids triggers a dust cloud
        {
            Reference.animationController.SpawnDustCloudAnimation(collisionPoint, asteroid.gameObject);
            Reference.soundController.asteroidCollisionSound();
        }
        //asteroid.ApplyImpulse(collisionDirection,1f);
    }

    public void DespawnAsteroid(Asteroid asteroid, List<GameObject> asteroidPack)
    {

        if (asteroidSets.Contains(asteroidPack))
        {
            foreach (GameObject asteroidObject in asteroidPack)
            {
                if (asteroids.Contains(asteroidObject))
                {
                    SimplePool.Despawn(asteroidObject);
                    asteroids.Remove(asteroidObject);
                }
            }
            asteroidSets.Remove(asteroidPack);
        }
        else
        {
            // Debug.LogError("This asteroid object set is not present in the list");
        }
    }



    void SpawnAsteroid(int size, Vector3 position, Vector3 velocity, Vector3 eulerAngles, float angularVelocity, SquareMesh squareMesh, bool NewAsteroid, Vector2 positionOrientation )
    {
        List<GameObject> asteroidPack = new List<GameObject>();

        float d = 2*worldSize.x;
        Quaternion rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);

        GameObject asteroidgo = SimplePool.Spawn(mainAsteroidPrefab, position + new Vector3(d,0,0), rotation);//spawns the first asteroid
        asteroidgo.transform.position = new Vector3(asteroidgo.transform.position.x,asteroidgo.transform.position.y,10);
        MainAsteroid mainAsteroid = asteroidgo.GetComponent<MainAsteroid>();
        mainAsteroid.derivedAsteroids = new Dictionary<Vector2,GameObject>();
        
        GameObject asteroidgo1 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*1,0,0), rotation);
        GameObject asteroidgo2 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*2,0,0), rotation);
        GameObject asteroidgo3 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*3,0,0), rotation);
        GameObject asteroidgo4 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*4,0,0), rotation);

        GameObject asteroidgo5 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*5,0,0), rotation);
        GameObject asteroidgo6 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*6,0,0), rotation);
        GameObject asteroidgo7 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*7,0,0), rotation);
        GameObject asteroidgo8 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*8,0,0), rotation);

        asteroidPack.Add(asteroidgo);
        asteroidPack.Add(asteroidgo1);
        asteroidPack.Add(asteroidgo2);
        asteroidPack.Add(asteroidgo3);
        asteroidPack.Add(asteroidgo4);
        asteroidPack.Add(asteroidgo5);
        asteroidPack.Add(asteroidgo6);
        asteroidPack.Add(asteroidgo7);
        asteroidPack.Add(asteroidgo8);


        asteroidgo.transform.SetParent(this.gameObject.transform);
        asteroidgo.GetComponent<MainAsteroid>().OnSpawn(size, new Vector2(0, 0), asteroidPack, asteroidgo, velocity, angularVelocity, squareMesh, NewAsteroid, positionOrientation);
        asteroidgo.transform.position = position;
        asteroids.Add(asteroidgo);

        asteroidgo1.transform.SetParent(this.gameObject.transform);
        asteroidgo1.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo1);
        if(NewAsteroid){asteroidgo1.SetActive(false);}
        mainAsteroid.derivedAsteroids.Add(new Vector2(1, 0),asteroidgo1);

        asteroidgo2.transform.SetParent(this.gameObject.transform);
        asteroidgo2.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo2);
        if(NewAsteroid){asteroidgo2.SetActive(false);}
        mainAsteroid.derivedAsteroids.Add(new Vector2(-1, 0),asteroidgo2);

        asteroidgo3.transform.SetParent(this.gameObject.transform);
        asteroidgo3.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo3);
        if(NewAsteroid){asteroidgo3.SetActive(false);}
        mainAsteroid.derivedAsteroids.Add(new Vector2(0, 1),asteroidgo3);

        asteroidgo4.transform.SetParent(this.gameObject.transform);
        asteroidgo4.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo4);
        if(NewAsteroid){asteroidgo4.SetActive(false);}
        mainAsteroid.derivedAsteroids.Add(new Vector2(0, -1),asteroidgo4);

        asteroidgo5.transform.SetParent(this.gameObject.transform);
        asteroidgo5.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo5);
        if(NewAsteroid){asteroidgo5.SetActive(false);}
        mainAsteroid.derivedAsteroids.Add(new Vector2(-1, -1),asteroidgo5);

        asteroidgo6.transform.SetParent(this.gameObject.transform);
        asteroidgo6.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo6);
        if(NewAsteroid){asteroidgo6.SetActive(false);}
        mainAsteroid.derivedAsteroids.Add(new Vector2(1, -1),asteroidgo6);

        asteroidgo7.transform.SetParent(this.gameObject.transform);
        asteroidgo7.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo7);
        if(NewAsteroid){asteroidgo7.SetActive(false);}
        mainAsteroid.derivedAsteroids.Add(new Vector2(-1, 1),asteroidgo7);

        asteroidgo8.transform.SetParent(this.gameObject.transform);
        asteroidgo8.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo8);
        if(NewAsteroid){asteroidgo8.SetActive(false);}
        mainAsteroid.derivedAsteroids.Add(new Vector2(1, 1),asteroidgo8);

        asteroidSets.Add(asteroidPack);


    }


}
