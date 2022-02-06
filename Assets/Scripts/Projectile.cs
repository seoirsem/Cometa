using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector2 screenCenter;// (0,-1,1 in each of x and y to determine which of the 5 screens it is on)
    ProjectileController projectileController;
    GameObject go;
    float timeFired;
    float lifespan = 2;//s
    Vector2 worldSize;
    float projectileSpeed = 11;
    List<GameObject> objectPack;
    void Start()
    {
        
    }

    public void OnFired(Vector2 screenCenter, List<GameObject> objectPack)
    {
        projectileController = Reference.projectileController;
        go = this.gameObject;
        this.objectPack = objectPack;
        this.screenCenter = screenCenter;
        timeFired = Time.time;
        worldSize = Reference.worldController.worldSize;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateMotion();
        if(Time.time - timeFired > lifespan)
        {
            DestroySelf();
        }
    }

    void UpdateMotion()
    {
        go.transform.position += Quaternion.Euler(0, 0, -90) * transform.up * projectileSpeed * Time.deltaTime; 

        if(go.transform.position.x - screenCenter.x * worldSize.x > worldSize.x/2)
        {
            go.transform.position = new Vector3(go.transform.position.x - worldSize.x, go.transform.position.y, go.transform.position.z);
        }
        if(go.transform.position.x - screenCenter.x * worldSize.x < -worldSize.x/2)
        {
            go.transform.position = new Vector3(go.transform.position.x + worldSize.x, go.transform.position.y, go.transform.position.z);
        }
        if(go.transform.position.y - screenCenter.y * worldSize.y > worldSize.y/2)
        {
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y - worldSize.y, go.transform.position.z);
        }
        if(go.transform.position.y - screenCenter.y * worldSize.y < -worldSize.y/2)
        {
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + worldSize.y, go.transform.position.z);
        }

    }

    void DestroySelf()
    {
        projectileController.DespawnProjectile(go,objectPack);
    }
}
