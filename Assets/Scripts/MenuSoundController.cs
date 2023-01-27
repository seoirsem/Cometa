using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundController : MonoBehaviour
{
    AudioSource audioSource;
    AudioSource musicSource;

    public float masterVolume;
    public float musicVolume;

    AudioClip menuMusic;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        musicSource = GameObject.Find("MusicSoundSource").GetComponent<AudioSource>();
        Debug.Log(musicSource);
        menuMusic = Resources.Load<AudioClip>("Sounds/menuSongPixabay");

        InitialiseVolumes(OptionsParameters.MusicVolume,OptionsParameters.MasterVolume);
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

    public void InitialiseVolumes(float musicVolumeSet, float masterVolumeSet)
    {
        if(musicVolumeSet >= 1)
        {
            musicVolume = 1;
            musicSource.volume = 1;
            OptionsParameters.MusicVolume = 1;
        }
        else if(musicVolumeSet <= 0)
        {
            musicVolume = 0;
            musicSource.volume = 0;    
            OptionsParameters.MusicVolume = 0;
        }
        else
        {
            musicVolume = musicVolumeSet;
            musicSource.volume = musicVolumeSet;
            OptionsParameters.MusicVolume = musicVolumeSet;
        }
        if(masterVolumeSet >= 1)
        {
            masterVolume = 1;
            audioSource.volume = 1;
            OptionsParameters.MasterVolume = 1;
        }
        else if(masterVolumeSet <= 0)
        {
            masterVolume = 0;
            audioSource.volume = 0;
            OptionsParameters.MasterVolume = 0;
        }
        else
        {
            masterVolume = masterVolumeSet;
            audioSource.volume = masterVolumeSet;
            OptionsParameters.MasterVolume = masterVolumeSet;
        }
    }


    public void SetMasterVolume(float volume)
    {
        if(volume >= 1)
        {
            masterVolume = 1;
            musicVolume = 1;
            audioSource.volume = 1;
            musicSource.volume = 1;
            OptionsParameters.MasterVolume = 1;
            OptionsParameters.MusicVolume = 1;
        }
        else if(volume <= 0)
        {
            masterVolume = 0;
            musicVolume = 0;
            audioSource.volume = 0;
            musicSource.volume = 0;    
            OptionsParameters.MasterVolume = 0;
            OptionsParameters.MusicVolume = 0;
        }
        else
        {
            masterVolume = volume;
            musicVolume = volume;
            audioSource.volume = volume;
            musicSource.volume = volume;    
            OptionsParameters.MusicVolume = volume;
            OptionsParameters.MasterVolume = volume;
        }
    }

    public void SetMusicVolume(float volume)
    {//a float between 0 and 1
        if(volume >= 1)
        {
            musicVolume = 1;
            musicSource.volume = 1;
            OptionsParameters.MusicVolume = 1;
        }
        else if(volume <= 0)
        {
            musicVolume = 0;
            musicSource.volume = 0;    
            OptionsParameters.MusicVolume = 0;
        }
        else
        {
            musicVolume = volume;
            musicSource.volume = volume;
            OptionsParameters.MusicVolume = volume;
        }
    }

    // public void playExplosionSound()
    // {
    //     audioSource.PlayOneShot(explosion);
    // }
}
