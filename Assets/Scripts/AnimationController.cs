using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    GameObject explosionAnimationPrefab;
    GameObject dustCloudAnimationPrefab;

    void Start()
    {
        explosionAnimationPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
        dustCloudAnimationPrefab = Resources.Load("Prefabs/Dust_Cloud") as GameObject;
    }

    void Update()
    {
        
    }

    public void SpawnExplosionAnimation(Vector3 collisionPoint)
    {
        GameObject explosionAnimationGo = SimplePool.Spawn(explosionAnimationPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        explosionAnimationGo.transform.SetParent(this.gameObject.transform);
    }

    public void SpawnDustCloudAnimation(Vector3 collisionPoint)
    {
        GameObject dustCloudAnimationGo = SimplePool.Spawn(dustCloudAnimationPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        dustCloudAnimationGo.transform.SetParent(this.gameObject.transform);

    }
}
