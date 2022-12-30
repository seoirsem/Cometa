using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundController : MonoBehaviour
{
    AudioSource audioSource;
    AudioSource musicSource;

    AudioClip menuMusic;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        musicSource = gameObject.GetComponent<AudioSource>();
        menuMusic = Resources.Load<AudioClip>("Sounds/menuSongPixabay");

        //ToDo: menu and game music. Music ramps up as you play
        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartMusic()
    {
        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    // public void playExplosionSound()
    // {
    //     audioSource.PlayOneShot(explosion);
    // }
}
