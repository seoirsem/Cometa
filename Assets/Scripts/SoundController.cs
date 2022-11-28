using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
 
    AudioSource audioSource;
    AudioClip asteroidCollision;
    AudioClip explosion;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        asteroidCollision = Resources.Load<AudioClip>("Sounds/rockImpact");
        explosion = Resources.Load<AudioClip>("Sounds/explosion");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void asteroidCollisionSound()
    {
        audioSource.PlayOneShot(asteroidCollision);
    }

    public void playExplosionSound()
    {
        audioSource.PlayOneShot(explosion);
    }
}
