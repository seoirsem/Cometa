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

    float spawnCooldown;


    void Start()
    {
        mainAsteroidPrefab = Resources.Load("Prefabs/MainAsteroid") as GameObject;
        deriveAsteroidPrefab = Resources.Load("Prefabs/DerivedAsteroid") as GameObject;
        worldSize = Reference.worldController.worldSize;
        spawnCooldown = Time.time;

        SpawnAsteroid(20, new Vector3(0, worldSize.y/2 - 0.2f*20f/2, 0), new Vector3(0, 0, 0), null, false, new Vector2(0,0)); 
    }

    void Update()
    {
        if (Reference.playerInputController.mouseClicked && !Reference.worldController.isPaused)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if(Reference.playerInputController.o)
        { //clear screen
            foreach(Transform child in this.transform)
            {
                GameObject.Destroy(child.gameObject);
                List<GameObject> asteroids = new List<GameObject>();
                List<List<GameObject>> asteroidSets = new List<List<GameObject>>();
            }
        }
        if(Reference.playerInputController.p & Time.time - spawnCooldown > 2f)
        { //spawnn new asteroid
            spawnCooldown = Time.time;
            SpawnNewAsteroid(10);
        }

    }

    public void SpawnNewAsteroid(int size)
    {
        /// velocity
        int[] numbers = {-1,0,1,0,0,1,0,-1};
        int randomIndex = Random.Range(0,3);
        Vector3 directionOrientation = new Vector3(numbers[randomIndex*2],numbers[randomIndex*2+1],0);
        float magnitude = Random.Range(0.2f,1f);
        Vector3 direction = -1 * magnitude*directionOrientation;

        Vector3 position = new Vector3(0,0,0);
        if(randomIndex == 0)
        {// up
            position = new Vector3(-worldSize.x/2 - 1.05f*size*Asteroid.celSize, -0.5f*size*Asteroid.celSize,0);
            direction += new Vector3(0,Random.Range(-0.4f,0.4f),0);
        }
        if(randomIndex == 1)
        {// up
            position = new Vector3(worldSize.x/2 + 0.05f*size*Asteroid.celSize, -0.5f*size*Asteroid.celSize,0);
            direction += new Vector3(0,Random.Range(-0.4f,0.4f),0);
        }
        if(randomIndex == 2)
        {// up
            position = new Vector3(-0.5f*size*Asteroid.celSize,worldSize.y/2 + 0.05f*size*Asteroid.celSize,0);
            direction += new Vector3(Random.Range(-0.4f,0.4f),0,0);
        }
        if(randomIndex == 3)
        {// up
            position = new Vector3(-0.5f*size*Asteroid.celSize,-worldSize.y/2 - 1.05f*size*Asteroid.celSize,0);
            direction += new Vector3(Random.Range(-0.4f,0.4f),0,0);
        }
        /// position
        //Vector3 position = new Vector3(directionOrientation.x*(worldSize.x/2 - size*Asteroid.celSize),directionOrientation.y*(worldSize.y/2 - size*Asteroid.celSize),0);

        SpawnAsteroid(size, position, direction, null, true, directionOrientation);    
        Debug.Log("Spawning new asteroid");
    }

    public void AsteroidHit(Asteroid asteroid, Vector2 contact, GameObject otherObject, List<GameObject> asteroidPack, List<SquareMesh> newAstroidMeshes,Vector3 offsetFromActualCollision = new Vector3())
    {
//        Debug.Log("Asteroid splitting - in asteroid controller");
        // Debug.Log(otherObject.name);
        
        DespawnAsteroid(asteroid,asteroidPack);

        Debug.Log(newAstroidMeshes.Count);
        foreach(SquareMesh squareMesh in newAstroidMeshes)
        {
        
            int[] numbersx = {-1,0,1,-1,1,-1,0,1};
            int[] numbersy = {1,1,1,0,0,-1,-1,-1};
            int randomIndex = Random.Range(0, 7);
            Vector3 direction = new Vector3(numbersx[randomIndex],numbersy[randomIndex],0);
            float magnitude = Random.Range(0.2f,1f);
            direction = magnitude*direction;

            SpawnAsteroid(40, new Vector3(Random.Range(-worldSize.x/4,worldSize.x/4), Random.Range(-worldSize.y/4, worldSize.y/4),10), direction, squareMesh,false, new Vector2(0,0));
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



    void SpawnAsteroid(int size, Vector3 position, Vector3 velocity, SquareMesh squareMesh, bool NewAsteroid, Vector2 positionOrientation )
    {
        List<GameObject> asteroidPack = new List<GameObject>();

        GameObject asteroidgo = SimplePool.Spawn(mainAsteroidPrefab, position, new Quaternion(0,0,0,0));//spawns the first asteroid
        asteroidgo.transform.position = new Vector3(asteroidgo.transform.position.x,asteroidgo.transform.position.y,10);
        MainAsteroid mainAsteroid = asteroidgo.GetComponent<MainAsteroid>();
        mainAsteroid.derivedAsteroids = new Dictionary<Vector2,GameObject>();
        float d = 2*worldSize.x;
        GameObject asteroidgo1 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*1,0,0), new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo2 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*2,0,0), new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo3 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*3,0,0), new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo4 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*4,0,0), new Quaternion(0, 0, 0, 0));

        GameObject asteroidgo5 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*5,0,0), new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo6 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*6,0,0), new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo7 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*7,0,0), new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo8 = SimplePool.Spawn(deriveAsteroidPrefab,position + new Vector3(d*8,0,0), new Quaternion(0, 0, 0, 0));

    
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
        asteroidgo.GetComponent<MainAsteroid>().OnSpawn(size, new Vector2(0, 0), asteroidPack, asteroidgo, velocity, squareMesh, NewAsteroid, positionOrientation);
        asteroids.Add(asteroidgo);

        asteroidgo1.transform.SetParent(this.gameObject.transform);
        asteroidgo1.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo1);
        mainAsteroid.derivedAsteroids.Add(new Vector2(1, 0),asteroidgo1);

        asteroidgo2.transform.SetParent(this.gameObject.transform);
        asteroidgo2.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo2);
        mainAsteroid.derivedAsteroids.Add(new Vector2(-1, 0),asteroidgo2);

        asteroidgo3.transform.SetParent(this.gameObject.transform);
        asteroidgo3.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo3);
        mainAsteroid.derivedAsteroids.Add(new Vector2(0, 1),asteroidgo3);

        asteroidgo4.transform.SetParent(this.gameObject.transform);
        asteroidgo4.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo4);
        mainAsteroid.derivedAsteroids.Add(new Vector2(0, -1),asteroidgo4);

        asteroidgo5.transform.SetParent(this.gameObject.transform);
        asteroidgo5.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo5);
        mainAsteroid.derivedAsteroids.Add(new Vector2(-1, -1),asteroidgo5);

        asteroidgo6.transform.SetParent(this.gameObject.transform);
        asteroidgo6.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo6);
        mainAsteroid.derivedAsteroids.Add(new Vector2(1, -1),asteroidgo6);

        asteroidgo7.transform.SetParent(this.gameObject.transform);
        asteroidgo7.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo7);
        mainAsteroid.derivedAsteroids.Add(new Vector2(-1, 1),asteroidgo7);

        asteroidgo8.transform.SetParent(this.gameObject.transform);
        asteroidgo8.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo8);
        mainAsteroid.derivedAsteroids.Add(new Vector2(1, 1),asteroidgo8);

        asteroidSets.Add(asteroidPack);

        if(NewAsteroid)
        {
            asteroidgo1.SetActive(false);
            asteroidgo2.SetActive(false);
            asteroidgo3.SetActive(false);
            asteroidgo4.SetActive(false);
            asteroidgo5.SetActive(false);
            asteroidgo6.SetActive(false);
            asteroidgo7.SetActive(false);
            asteroidgo8.SetActive(false);
        }

        //mainAsteroid.SetSpawningDerivedAsteroid();
    }

    // void SpawnSplitAsteroid(int size, Vector3 position, Vector3 velocity, Asteroid asteroidData)
    // {
    //     List<GameObject> asteroidPack = new List<GameObject>();

    //     GameObject asteroidgo = SimplePool.Spawn(mainAsteroidPrefab, position, new Quaternion(0,0,0,0));//spawns the first asteroid
    //     MainAsteroid mainAsteroid = asteroidgo.GetComponent<MainAsteroid>();
    //     mainAsteroid.derivedAsteroids = new Dictionary<Vector2,GameObject>();

    //     GameObject asteroidgo1 = SimplePool.Spawn(deriveAsteroidPrefab,position, new Quaternion(0, 0, 0, 0));
    //     GameObject asteroidgo2 = SimplePool.Spawn(deriveAsteroidPrefab,position, new Quaternion(0, 0, 0, 0));
    //     GameObject asteroidgo3 = SimplePool.Spawn(deriveAsteroidPrefab,position, new Quaternion(0, 0, 0, 0));
    //     GameObject asteroidgo4 = SimplePool.Spawn(deriveAsteroidPrefab,position, new Quaternion(0, 0, 0, 0));

    //     asteroidPack.Add(asteroidgo);
    //     asteroidPack.Add(asteroidgo1);
    //     asteroidPack.Add(asteroidgo2);
    //     asteroidPack.Add(asteroidgo3);
    //     asteroidPack.Add(asteroidgo4);

    //     asteroidgo.transform.SetParent(this.gameObject.transform);
    //     asteroidgo.GetComponent<MainAsteroid>().OnSpawn(size, new Vector2(0, 0), asteroidPack, asteroidgo, velocity, asteroidData);
    //     asteroids.Add(asteroidgo);

    //     asteroidgo1.transform.SetParent(this.gameObject.transform);
    //     asteroidgo1.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, 0), asteroidPack, asteroidgo, velocity);
    //     asteroids.Add(asteroidgo1);
    //     mainAsteroid.derivedAsteroids.Add(new Vector2(1, 0),asteroidgo1);


    //     asteroidgo2.transform.SetParent(this.gameObject.transform);
    //     asteroidgo2.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, 0), asteroidPack, asteroidgo, velocity);
    //     asteroids.Add(asteroidgo2);
    //     mainAsteroid.derivedAsteroids.Add(new Vector2(-1, 0),asteroidgo2);


    //     asteroidgo3.transform.SetParent(this.gameObject.transform);
    //     asteroidgo3.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, 1), asteroidPack, asteroidgo, velocity);
    //     asteroids.Add(asteroidgo3);
    //     mainAsteroid.derivedAsteroids.Add(new Vector2(0, 1),asteroidgo3);


    //     asteroidgo4.transform.SetParent(this.gameObject.transform);
    //     asteroidgo4.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, -1), asteroidPack, asteroidgo, velocity);
    //     asteroids.Add(asteroidgo4);
    //     mainAsteroid.derivedAsteroids.Add(new Vector2(0, -1),asteroidgo4);


    //     asteroidSets.Add(asteroidPack);
    // }
}
