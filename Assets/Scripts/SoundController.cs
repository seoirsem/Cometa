using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
 
    AudioSource audioSource;
    AudioSource musicSource;

    AudioClip asteroidCollision;
    AudioClip explosion;
    AudioClip music;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        musicSource = transform.Find("MusicController").GetComponent<AudioSource>();
        asteroidCollision = Resources.Load<AudioClip>("Sounds/rockImpact");
        explosion = Resources.Load<AudioClip>("Sounds/explosion");
        music = Resources.Load<AudioClip>("Sounds/song_pixabay");

        //ToDo: menu and game music. Music ramps up as you play
        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartMusic()
    {
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
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
