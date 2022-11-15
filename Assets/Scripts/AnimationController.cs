using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    GameObject explosionAnimationPrefab;
    GameObject dustCloudAnimationPrefab;
    GameObject blueFlameAnimationPrefab;

    void Start()
    {
        explosionAnimationPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
        dustCloudAnimationPrefab = Resources.Load("Prefabs/Dust_Cloud") as GameObject;
        blueFlameAnimationPrefab = Resources.Load("Prefabs/Blue_Flame") as GameObject;
    }

    void Update()
    {
        
    }

    public void SpawnExplosionAnimation(Vector3 collisionPoint)
    {
        GameObject explosionAnimationGo = SimplePool.Spawn(explosionAnimationPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        explosionAnimationGo.transform.SetParent(this.gameObject.transform);
    }

    public void SpawnDustCloudAnimation(Vector3 collisionPoint, GameObject asteroidgo)
    {
        GameObject dustCloudAnimationGo = SimplePool.Spawn(dustCloudAnimationPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        dustCloudAnimationGo.transform.SetParent(asteroidgo.transform);
    }

    public GameObject SpawnBlueFlameAnimation(Vector3 collisionPoint, GameObject rocketgo)
    {
        GameObject blueFlameAnimationGo = SimplePool.Spawn(blueFlameAnimationPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        blueFlameAnimationGo.transform.SetParent(rocketgo.transform);
        blueFlameAnimationGo.transform.Rotate(new Vector3( 0, 0, 180));
        return blueFlameAnimationGo;
    }
}
