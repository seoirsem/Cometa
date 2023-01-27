using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    float time;
    float cooldown = 0.2f;//s
    GameObject rocketPrefab;
    GameObject bulletPrefab;
    List<GameObject> projectileList = new List<GameObject>();
    List<List<GameObject>> projectilePacks = new List<List<GameObject>>();
    Vector2 worldSize;
    void Start()
    {
        time = Time.time;
        rocketPrefab = Resources.Load("Prefabs/Rocket") as GameObject;
        bulletPrefab = Resources.Load("Prefabs/Bullet") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShootProjectile(Vector3 position, Quaternion rotation, string projectileType)
    {
        // NOTE the other 4 are commented out because they are not needed. Projectioles can hit the virtual asteroid objects instead of needing to wrap neatly
        worldSize = Reference.worldController.worldSize;
        //5 times for the 5 screens
        GameObject projectilego = new GameObject();
        if(projectileType == "Rocket")
        {
            projectilego = SimplePool.Spawn(rocketPrefab, position, rotation);
        }
        else if (projectileType == "Bullet")
        {
            projectilego = SimplePool.Spawn(bulletPrefab, position, rotation);
        }
        else
        {
            Debug.LogError("No projectile type exists with name " + projectileType);
        }
        
        List<GameObject> projectilePack = new List<GameObject>();
        projectilePack.Add(projectilego);
        
        projectilego.GetComponent<Projectile>().mainProjectile = true;
        projectilego.transform.SetParent(this.gameObject.transform);
        projectilego.GetComponent<Projectile>().OnFired(new Vector2(0, 0),projectilePack,projectileType);
        projectileList.Add(projectilego);

        
        projectilePacks.Add(projectilePack);



        ///   Instead of calling Instantiate(), use this:
        ///       SimplePool.Spawn(somePrefab, somePosition, someRotation);
        /// 
        ///   Instead of destroying an object, use this:
        ///       SimplePool.Despawn(myGameObject);
        /// 
        ///   If desired, you can preload the pool with a number of instances:
        ///       SimplePool.Preload(somePrefab, 20);
    }
    

    public void DespawnProjectile(GameObject go, List<GameObject> objectPack) // called by the projectile itself
    {
        //go is the object that actually triggered the destruction
        if (projectilePacks.Contains(objectPack))
        {
            foreach(GameObject projectileObject in objectPack)
            {
                if (projectileList.Contains(projectileObject))
                {
                    if(projectileObject.GetComponent<Projectile>().projectileType == "Rocket")
                    {
                        Reference.animationController.SpawnExplosionAnimation(projectileObject.transform.position);
                        Reference.soundController.playExplosionSound();
                    }
                    SimplePool.Despawn(projectileObject);
                    projectileList.Remove(projectileObject);
                }
            }
            projectilePacks.Remove(objectPack);
        }
        else
        {
            // Debug.LogError("This projectile object set is not present in the list");
        }


    }

}
