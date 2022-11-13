using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    float time;
    float cooldown = 1.2f;//s
    GameObject projectilePrefab;
    List<GameObject> projectileList = new List<GameObject>();
    List<List<GameObject>> projectilePacks = new List<List<GameObject>>();
    Vector2 worldSize;
    void Start()
    {
        time = Time.time;
        projectilePrefab = Resources.Load("Prefabs/Projectile") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShootProjectile(Vector3 position, Quaternion rotation)
    {
        // NOTE the other 4 are commented out because they are not needed. Projectioles can hit the virtual asteroid objects instead of needing to wrap neatly
        worldSize = Reference.worldController.worldSize;
        //5 times for the 5 screens
        GameObject projectilego = SimplePool.Spawn(projectilePrefab, position, rotation);
        // GameObject projectilego1 = SimplePool.Spawn(projectilePrefab, position, rotation);
        // GameObject projectilego2 = SimplePool.Spawn(projectilePrefab, position, rotation);
        // GameObject projectilego3 = SimplePool.Spawn(projectilePrefab, position, rotation);
        // GameObject projectilego4 = SimplePool.Spawn(projectilePrefab, position, rotation);

        List<GameObject> projectilePack = new List<GameObject>();
        projectilePack.Add(projectilego);
        // projectilePack.Add(projectilego1);
        // projectilePack.Add(projectilego2);
        // projectilePack.Add(projectilego3);
        // projectilePack.Add(projectilego4);

        projectilego.GetComponent<Projectile>().mainProjectile = true;
        projectilego.transform.SetParent(this.gameObject.transform);
        projectilego.GetComponent<Projectile>().OnFired(new Vector2(0, 0),projectilePack);
        projectileList.Add(projectilego);

        // projectilego1.GetComponent<Projectile>().mainProjectile = false;
        // projectilego1.transform.SetParent(this.gameObject.transform);
        // projectilego1.GetComponent<Projectile>().OnFired(new Vector2(0, -1), projectilePack);
        // projectileList.Add(projectilego1);

        // projectilego2.GetComponent<Projectile>().mainProjectile = false;
        // projectilego2.transform.SetParent(this.gameObject.transform);
        // projectilego2.GetComponent<Projectile>().OnFired(new Vector2(0, 1), projectilePack);
        // projectileList.Add(projectilego2);

        // projectilego3.GetComponent<Projectile>().mainProjectile = false;
        // projectilego3.transform.SetParent(this.gameObject.transform);
        // projectilego3.GetComponent<Projectile>().OnFired(new Vector2(-1, 0), projectilePack);
        // projectileList.Add(projectilego3);

        // projectilego4.GetComponent<Projectile>().mainProjectile = false;
        // projectilego4.transform.SetParent(this.gameObject.transform);
        // projectilego4.GetComponent<Projectile>().OnFired(new Vector2(1, 0), projectilePack);
        // projectileList.Add(projectilego4);


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
                    Reference.animationController.SpawnExplosionAnimation(projectileObject.transform.position);
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
