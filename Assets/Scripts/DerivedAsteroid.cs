using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DerivedAsteroid : Asteroid
{


    Vector2 offset;
    GameObject mainAsteroid;
    MainAsteroid mainAsteroidClass;

    public void OnSpawn(int size, Vector2 location, List<GameObject> asteroidPack, GameObject mainAsteroid, Vector2 velocity)
    {
        this.mass = Mathf.Pow(size,2);
        // Update this in future to calculate based on the area of the shape formed by the mesh

        this.size = size;
        asteroidController = GameObject.Find("AsteroidController").GetComponent<AsteroidController>();
        asteroidOutlines = this.gameObject.transform.Find("AsteroidOutline").gameObject;
        mainAsteroidClass = mainAsteroid.GetComponent<MainAsteroid>();
        rigid_body = this.GetComponent<Rigidbody2D>();
        this.asteroidPack = asteroidPack;
        asteroidgo = this.gameObject;
        this.velocity = velocity;
        this.rigid_body.velocity = velocity;
        this.worldSize = Reference.worldController.worldSize;
        this.location = location;
        rigid_body.centerOfMass = new Vector2(0,0);
        PolygonCollider2D polygonCollider = this.gameObject.GetComponent<PolygonCollider2D>();

        offset = new Vector2(location.x * worldSize.x, location.y * worldSize.y);

        CloneAsteroid(mainAsteroid);




    }

    // Update is called once per frame
    void Update()
    {
        UpdateAsteroidPosition();
    }

    void UpdateAsteroidPosition()
    {
        rigid_body.position = mainAsteroidClass.rigid_body.position + offset;
        rigid_body.rotation = mainAsteroidClass.rigid_body.rotation;
    }



    void CloneAsteroid(GameObject mainAsteroid)
    {
        Asteroid mainAst = mainAsteroid.GetComponent<Asteroid>();
        meshVertices = mainAst.meshVertices;
        meshTriangles = mainAst.meshTriangles;
        meshIndices = mainAst.meshIndices;
        DrawMesh(meshVertices, meshTriangles, meshIndices);
        DrawCollider(meshVertices, meshTriangles);
        
    }
}
