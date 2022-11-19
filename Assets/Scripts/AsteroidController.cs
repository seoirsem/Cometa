﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    GameObject mainAsteroidPrefab;
    GameObject derivedAsteroidPrefab;
    List<GameObject> asteroids = new List<GameObject>();
    List<List<GameObject>> asteroidSets = new List<List<GameObject>>();
    Vector2 worldSize;

    Vector3 dummy1 = new Vector3(1,1,1);
    Vector3[] dummy2 = new Vector3[1];


    void Start()
    {
        mainAsteroidPrefab = Resources.Load("Prefabs/MainAsteroid") as GameObject;
        derivedAsteroidPrefab = Resources.Load("Prefabs/DerivedAsteroid") as GameObject;
        worldSize = Reference.worldController.worldSize;
        SpawnAsteroid(4, new Vector3(0, 10f, 0), new Vector3(0,0,0));
        // SpawnAsteroid(6, new Vector3(0, 3, 0), new Vector3(0,0,0));
        Vector3 a = new Vector3(1,0,0);
        Vector3 b = new Vector3(0,-1,0);
        // Debug.Log(Vector3.SignedAngle(a,b,Vector3.forward));
        // Debug.Log(Vector3.forward);
        // Debug.Log(Vector3.Cross(b,a));
    }

    void Update()
    {
        if (Reference.playerInputController.mouseClicked)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // SpawnAsteroid(6, new Vector3(0f,3f,0), new Vector3(0, 0, 0)); 
            // SpawnAsteroid(4, new Vector3(mousePosition.x,mousePosition.y,0), new Vector3(Random.Range(-1,1), Random.Range(-1, 1), 0));    
        }
    }
    public void AsteroidHit(Asteroid asteroid, Vector2 contact, GameObject otherObject, List<GameObject> asteroidPack)
    {
        int size = asteroid.size;
        Vector3 asteroidPosition = asteroid.gameObject.transform.position;
        Vector3 asteroidVelocity = asteroid.velocity;
        Vector3 collisionPoint = new Vector3(contact.x, contact.y, 0);
        Vector3 collisionDirection = (collisionPoint - asteroidPosition).normalized;
        Vector3 left = Vector3.Cross(collisionDirection, new Vector3(0, 0, 1)).normalized;
        Vector3 right = Vector3.Cross(collisionDirection, new Vector3(0, 0, -1)).normalized;

        Asteroid[] splitAsteroidData = asteroid.SplitAsteroid(asteroid.meshVertices, collisionPoint - asteroidPosition, collisionDirection);

        DespawnAsteroid(asteroid, asteroidPack);
        
        // Asteroid[] splitAsteroidData = new Asteroid[2];
        
        float debugDontMove = 1f;

        // Oooo-kayyyy... So in Logs below, first evaluates to null, both the second has a well defined value.
        // God is dead.
        // Debug.Log(splitAsteroidData[0] == null);
        // Debug.Log(splitAsteroidData[0]);
        // Debug.Log(splitAsteroidData[0].meshVertices[0]);
        //////////////// return to split asteroids
        // if(size != 1)
        // {
        //     SpawnAsteroid(size - 1, asteroidPosition + left * (size / 6f), (asteroidVelocity + left * 1f)*debugDontMove);
        //     SpawnAsteroid(size - 1, asteroidPosition + right * (size / 6f), (asteroidVelocity + right * 1f)*debugDontMove);
        // }
        // return;

        //////////////
        if (splitAsteroidData[0] != null)
        {
            // Debug.Log(splitAsteroidData[0].meshVertices.Length);
            SpawnSplitAsteroid(2, asteroidPosition + left * (size / 1f)*0f, (asteroidVelocity + left * 1f)*debugDontMove, splitAsteroidData[0]);
        
        }

        if (splitAsteroidData[1] != null)
        {
            // Debug.Log(splitAsteroidData[1].meshVertices.Length);
            SpawnSplitAsteroid(2, asteroidPosition + left * (size / 6f)*0f, (asteroidVelocity + right * 1f)*debugDontMove, splitAsteroidData[1]);
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
        }
        //asteroid.ApplyImpulse(collisionDirection,1f);
    }

    void DespawnAsteroid(Asteroid asteroid, List<GameObject> asteroidPack)
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



    void SpawnAsteroid(int size, Vector3 position, Vector3 velocity)
    {
        List<GameObject> asteroidPack = new List<GameObject>();

        GameObject asteroidgo = SimplePool.Spawn(mainAsteroidPrefab, position, new Quaternion(0,0,0,0));//spawns the first asteroid

        GameObject asteroidgo1 = SimplePool.Spawn(derivedAsteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo2 = SimplePool.Spawn(derivedAsteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo3 = SimplePool.Spawn(derivedAsteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo4 = SimplePool.Spawn(derivedAsteroidPrefab, position, new Quaternion(0, 0, 0, 0));

        asteroidPack.Add(asteroidgo);
        asteroidPack.Add(asteroidgo1);
        asteroidPack.Add(asteroidgo2);
        asteroidPack.Add(asteroidgo3);
        asteroidPack.Add(asteroidgo4);

        asteroidgo.transform.SetParent(this.gameObject.transform);
        asteroidgo.GetComponent<MainAsteroid>().OnSpawn(size, new Vector2(0, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo);

        asteroidgo1.transform.SetParent(this.gameObject.transform);
        asteroidgo1.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo1);

        asteroidgo2.transform.SetParent(this.gameObject.transform);
        asteroidgo2.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo2);

        asteroidgo3.transform.SetParent(this.gameObject.transform);
        asteroidgo3.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo3);

        asteroidgo4.transform.SetParent(this.gameObject.transform);
        asteroidgo4.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo4);

        asteroidSets.Add(asteroidPack);
    }

    void SpawnSplitAsteroid(int size, Vector3 position, Vector3 velocity, Asteroid asteroidData)
    {
        List<GameObject> asteroidPack = new List<GameObject>();

        GameObject asteroidgo = SimplePool.Spawn(mainAsteroidPrefab, position, new Quaternion(0,0,0,0));//spawns the first asteroid

        GameObject asteroidgo1 = SimplePool.Spawn(derivedAsteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo2 = SimplePool.Spawn(derivedAsteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo3 = SimplePool.Spawn(derivedAsteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo4 = SimplePool.Spawn(derivedAsteroidPrefab, position, new Quaternion(0, 0, 0, 0));

        asteroidPack.Add(asteroidgo);
        asteroidPack.Add(asteroidgo1);
        asteroidPack.Add(asteroidgo2);
        asteroidPack.Add(asteroidgo3);
        asteroidPack.Add(asteroidgo4);

        asteroidgo.transform.SetParent(this.gameObject.transform);
        asteroidgo.GetComponent<MainAsteroid>().OnSpawnSplitAsteroid(size, new Vector2(0, 0), asteroidPack, asteroidgo, velocity, asteroidData);
        asteroids.Add(asteroidgo);

        asteroidgo1.transform.SetParent(this.gameObject.transform);
        asteroidgo1.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo1);

        asteroidgo2.transform.SetParent(this.gameObject.transform);
        asteroidgo2.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(-1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo2);

        asteroidgo3.transform.SetParent(this.gameObject.transform);
        asteroidgo3.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo3);

        asteroidgo4.transform.SetParent(this.gameObject.transform);
        asteroidgo4.GetComponent<DerivedAsteroid>().OnSpawn(size, new Vector2(0, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo4);

        asteroidSets.Add(asteroidPack);
    }
}
