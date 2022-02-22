using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    GameObject asteroidPrefab;
    List<GameObject> asteroids = new List<GameObject>();
    List<List<GameObject>> asteroidSets = new List<List<GameObject>>();
    Vector2 worldSize;


    void Start()
    {
        asteroidPrefab = Resources.Load("Prefabs/Asteroid") as GameObject;
        worldSize = Reference.worldController.worldSize;
        SpawnAsteroid(3, new Vector3(2, 0, 0), new Vector3(1,1,0));
    }



    void Update()
    {
        if (Reference.playerInputController.mouseClicked)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SpawnAsteroid(3, new Vector3(mousePosition.x,mousePosition.y,0), new Vector3(Random.Range(-1,1), Random.Range(-1, 1), 0));    
        }
    }
    public void AsteroidHit(Asteroid asteroid, Collider2D collider2D, List<GameObject> asteroidPack)
    {

        int size = asteroid.size;
        Vector3 asteroidPosition = asteroid.gameObject.transform.position;
        Vector3 asteroidVelocity = asteroid.velocity;
        Vector3 collisionPoint = collider2D.transform.position;
        Vector3 collisionDirection = (collisionPoint - asteroidPosition).normalized;
        Vector3 left = Vector3.Cross(collisionDirection, new Vector3(0, 0, 1)).normalized;
        Vector3 right = Vector3.Cross(collisionDirection, new Vector3(0, 0, -1)).normalized;
        //        Debug.Log(left);
        //        Debug.Log(right);

        DespawnAsteroid(asteroid, asteroidPack);
        if (size == 3)
        {
            SpawnAsteroid(2, asteroidPosition + left * (3 / 6f), asteroidVelocity + left * 1f);
            SpawnAsteroid(2, asteroidPosition + right * (3 / 6f), asteroidVelocity + right * 1f);
        }
        else if(size == 2)
        {
            SpawnAsteroid(1, asteroidPosition + left * (2 / 6f), asteroidVelocity + left * 1f);
            SpawnAsteroid(1, asteroidPosition + right * (2 / 6f), asteroidVelocity + right * 1f);
        }

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
            Debug.LogError("This asteroid object set is not present in the list");
        }
    }

    void SpawnAsteroid(int size, Vector3 position, Vector3 velocity)
    {
        List<GameObject> asteroidPack = new List<GameObject>();

        GameObject asteroidgo = SimplePool.Spawn(asteroidPrefab, position, new Quaternion(0,0,0,0));//spawns the first asteroid

        GameObject asteroidgo1 = SimplePool.Spawn(asteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo2 = SimplePool.Spawn(asteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo3 = SimplePool.Spawn(asteroidPrefab, position, new Quaternion(0, 0, 0, 0));
        GameObject asteroidgo4 = SimplePool.Spawn(asteroidPrefab, position, new Quaternion(0, 0, 0, 0));

        asteroidPack.Add(asteroidgo);
        asteroidPack.Add(asteroidgo1);
        asteroidPack.Add(asteroidgo2);
        asteroidPack.Add(asteroidgo3);
        asteroidPack.Add(asteroidgo4);

        asteroidgo.transform.SetParent(this.gameObject.transform);
        asteroidgo.GetComponent<Asteroid>().OnSpawn(size, new Vector2(0, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo);

        asteroidgo1.transform.SetParent(this.gameObject.transform);
        asteroidgo1.GetComponent<Asteroid>().OnSpawn(size, new Vector2(1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo1);

        asteroidgo2.transform.SetParent(this.gameObject.transform);
        asteroidgo2.GetComponent<Asteroid>().OnSpawn(size, new Vector2(-1, 0), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo2);

        asteroidgo3.transform.SetParent(this.gameObject.transform);
        asteroidgo3.GetComponent<Asteroid>().OnSpawn(size, new Vector2(0, 1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo3);

        asteroidgo4.transform.SetParent(this.gameObject.transform);
        asteroidgo4.GetComponent<Asteroid>().OnSpawn(size, new Vector2(0, -1), asteroidPack, asteroidgo, velocity);
        asteroids.Add(asteroidgo4);

        asteroidSets.Add(asteroidPack);


    }
}
