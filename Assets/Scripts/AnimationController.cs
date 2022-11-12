using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    GameObject explosionAnimationPrefab;

    void Start()
    {
        explosionAnimationPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
    }

    void Update()
    {
        
    }

    public void SpawnExplosionAnimation(Vector3 collisionPoint)
    {
        GameObject explosionAnimationGo = SimplePool.Spawn(explosionAnimationPrefab, collisionPoint, new Quaternion(0, 0, 0, 0));
        explosionAnimationGo.transform.SetParent(this.gameObject.transform);
    }
}
