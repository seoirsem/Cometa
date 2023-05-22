using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
 
    List<AudioSource> audioSourceList;
    AudioSource audioSource;
    AudioSource musicSource;
    AudioSource bulletAudioSource;
    AudioSource rocketAudioSource;
    AudioSource playerAudioSource;
    AudioSource playerTurningAudioSource;

    public float masterVolume;
    public float musicVolume;

    AudioClip asteroidCollision;
    AudioClip explosion;
    AudioClip music;
    AudioClip playerDeadSound;
    AudioClip blasterMultiple;
    AudioClip music80s;
    AudioClip shieldBeginCharge;
    AudioClip laserShoot;
    AudioClip laserSingleShot;
    AudioClip rockDestroy;
    AudioClip beepWarning;
    AudioClip longWhoosh;
    AudioClip shortWhoosh;
    AudioClip rocketShoot;
    AudioClip shieldImpact;
    AudioClip rocketBoostLow;
    AudioClip rocketMedium;
    AudioClip rocketMediumLow;
    AudioClip points;
    AudioClip shieldCharged;
    AudioClip coinDrop;
    AudioClip success;
    AudioClip doorSound;
    AudioClip snare;
    AudioClip glassBreak;
    static int choice = 1;

    float musicMaxVolume = 0.45f;

    float asteroidCollisionCooldown = 0.5f;
    float lastAsteroidCollision;

    float shieldImpactCooldown = 0.19f;
    float lastShieldImpact;

    float lastPointScored;
    float pointScoreCooldown = 0.19f;


    // Start is called before the first frame update
    void Start()
    {
        audioSourceList = new List<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        audioSourceList.Add(audioSource);
        musicSource = transform.Find("MusicController").GetComponent<AudioSource>();
        audioSourceList.Add(musicSource);
        bulletAudioSource = transform.Find("BulletAudioSource").GetComponent<AudioSource>();
        audioSourceList.Add(bulletAudioSource);
        rocketAudioSource = transform.Find("RocketAudioSource").GetComponent<AudioSource>();
        audioSourceList.Add(rocketAudioSource);
        playerTurningAudioSource = transform.Find("PlayerTurningAudioSource").GetComponent<AudioSource>();
        audioSourceList.Add(playerTurningAudioSource);
        playerAudioSource = GameObject.Find("Player").GetComponent<AudioSource>();
        audioSourceList.Add(playerAudioSource);
        
        
        
        
        asteroidCollision = Resources.Load<AudioClip>("Sounds/rockImpact");
        playerDeadSound = Resources.Load<AudioClip>("Sounds/player_dead_sound");
        explosion = Resources.Load<AudioClip>("Sounds/explosion_big");
        music = Resources.Load<AudioClip>("Sounds/song_pixabay");
        music80s = Resources.Load<AudioClip>("Sounds/80s_song_pixabay");
        blasterMultiple = Resources.Load<AudioClip>("Sounds/blaster_multiple");
        shieldBeginCharge = Resources.Load<AudioClip>("Sounds/shield_charge");
        laserShoot = Resources.Load<AudioClip>("Sounds/firing_many");
        laserSingleShot = Resources.Load<AudioClip>("Sounds/single_shot");
        rockDestroy = Resources.Load<AudioClip>("Sounds/rock_destroy");
        beepWarning = Resources.Load<AudioClip>("Sounds/beep_warning");
        longWhoosh = Resources.Load<AudioClip>("Sounds/whoosh_long");
        shortWhoosh = Resources.Load<AudioClip>("Sounds/short_whoosh");
        rocketShoot = Resources.Load<AudioClip>("Sounds/rocket_shoot");
        shieldImpact = Resources.Load<AudioClip>("Sounds/shield_impact");
        rocketBoostLow = Resources.Load<AudioClip>("Sounds/rocket_boost_low");
        rocketMedium = Resources.Load<AudioClip>("Sounds/rocket_medium");
        rocketMediumLow = Resources.Load<AudioClip>("Sounds/rocket_medium_low");
        shieldCharged = Resources.Load<AudioClip>("Sounds/fully_charged");
        points = Resources.Load<AudioClip>("Sounds/points");
        coinDrop = Resources.Load<AudioClip>("Sounds/coin_drop");
        success = Resources.Load<AudioClip>("Sounds/success");
        doorSound = Resources.Load<AudioClip>("Sounds/door_sound");
        snare = Resources.Load<AudioClip>("Sounds/snare");
        glassBreak = Resources.Load<AudioClip>("Sounds/glass_break");
        
        InitialiseVolumes(OptionsParameters.MusicVolume,OptionsParameters.MasterVolume);
        //ToDo: menu and game music. Music ramps up source you play
        StartMusic();

        lastAsteroidCollision = Time.time;
        lastShieldImpact = Time.time;
        lastPointScored = Time.time;

        bulletAudioSource.clip = laserShoot;

    }

    void Update()
    {
        //Debug.Log(OptionsParameters.MusicVolume);
    }

    void StartMusic()
    {
        //int musicChoice = Random.Range(0,2);
        //Debug.Log($"Music choice: {musicChoice}");
        if(choice == 0)
        {
            choice = 1;
            musicSource.clip = music;
        }
        else
        {
            choice = 0;
            musicSource.clip = music80s;
        }
        musicSource.loop = true;
        musicSource.Play();
    }

    void AdjustMasterVolume(float volume)
    {
        foreach(AudioSource source in audioSourceList)
        {
            if(source == musicSource && volume > musicMaxVolume)
            {
                source.volume = musicMaxVolume;
            }
            else
            {
                source.volume = volume;
            }
        }
    }

    public void PlayerDeadSound()
    {
        GameOver();
        audioSource.PlayOneShot(playerDeadSound);
    }

    public void InitialiseVolumes(float musicVolumeSet, float masterVolumeSet)
    {
        if(musicVolumeSet >= musicMaxVolume)
        {
            musicVolume = musicMaxVolume;
            musicSource.volume = musicMaxVolume;
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
            musicVolume = musicVolumeSet*musicMaxVolume;
            musicSource.volume = musicVolumeSet*musicMaxVolume;
            OptionsParameters.MusicVolume = musicVolumeSet;
        }
        if(masterVolumeSet >= 1)
        {
            masterVolume = 1;
            SetMasterVolume(1);
            OptionsParameters.MasterVolume = 1;
        }
        else if(masterVolumeSet <= 0)
        {
            masterVolume = 0;
            SetMasterVolume(0);
            OptionsParameters.MasterVolume = 0;
        }
        else
        {
            masterVolume = masterVolumeSet;
            SetMasterVolume(masterVolumeSet);
            OptionsParameters.MasterVolume = masterVolumeSet;
        }
    }
    public void ShieldFullyCharged()
    {
        audioSource.PlayOneShot(shieldCharged);
    }
    public void ShieldBroken()
    {
        audioSource.PlayOneShot(glassBreak);
    }
    public void asteroidCollisionSound()
    {
        if(Time.time - lastAsteroidCollision > asteroidCollisionCooldown)
        {
            audioSource.PlayOneShot(asteroidCollision);
            lastAsteroidCollision = Time.time;
        }
    }
    public void ScorePoints()
    {
        if(Time.time - lastPointScored > pointScoreCooldown)
        {
            lastPointScored = pointScoreCooldown;
//        audioSource.PlayOneShot(points);
            audioSource.PlayOneShot(coinDrop,masterVolume*0.65f);
//        audioSource.PlayOneShot(success);
//        audioSource.PlayOneShot(doorSound);
            audioSource.PlayOneShot(snare,masterVolume*0.65f);
        }
    }

    public void PlayExplosionSound()
    {
        rocketAudioSource.PlayOneShot(explosion);
    }

    public void playBeginShieldCharge()
    {
        AudioClip charge = MakeSubclip(shieldBeginCharge,3f,3.5f);
        audioSource.PlayOneShot(shieldBeginCharge,audioSource.volume/0.75f);
    }

    public void StartShootingBullets()
    {
        bulletAudioSource.Play();
    }

    public void PlayshieldImpact()
    {
        if(Time.time - lastShieldImpact > shieldImpactCooldown)
        {
            lastShieldImpact = Time.time;
            audioSource.PlayOneShot(shieldImpact);
        }
    }

    public void StartRocketBoost()
    {
        if(playerAudioSource.clip != rocketMedium)
        {
            playerAudioSource.clip = rocketMedium;
            playerAudioSource.volume = 0.75f*masterVolume;
        }
        playerAudioSource.Play();
    }

    public void StopRocketBoost()
    {
        playerAudioSource.Pause();
    }

    public void StartTurningRocketBoost()
    {
        if(playerTurningAudioSource.clip != rocketMedium)
        {
            playerTurningAudioSource.clip = rocketMedium;
            playerTurningAudioSource.volume = 0.5f*masterVolume;
        }
        playerTurningAudioSource.Play();
    }

    public void StopTurningRocketBoost()
    {
        playerTurningAudioSource.Pause();
    }

    public void StopShootingBullets()
    {
        bulletAudioSource.Pause();
    }

    public void PlayLongWhoosh()
    {
//        rocketAudioSource.PlayOneShot(longWhoosh);
        rocketAudioSource.clip = rocketShoot;
        rocketAudioSource.Play();
    }
    public void PlayShortWhoosh()
    {
        rocketAudioSource.PlayOneShot(shortWhoosh);
    }

    public void FireBullet()
    {
        bulletAudioSource.PlayOneShot(laserSingleShot);
    }
    public void PlayWarning()
    {
        audioSource.PlayOneShot(beepWarning);
    }

    public void RockDestroy()
    {
        audioSource.PlayOneShot(rockDestroy,audioSource.volume/5f);
    }

    public void RocketDestroyed()
    {
        rocketAudioSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        if(volume >= 1)
        {
            masterVolume = 1;
            musicVolume = musicMaxVolume;
            audioSource.volume = 1;
            musicSource.volume = musicMaxVolume;
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
        if(volume >= musicMaxVolume)
        {
            musicVolume = musicMaxVolume;
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
            musicVolume = volume*musicMaxVolume;
            musicSource.volume = volume;
            OptionsParameters.MusicVolume = volume;
        }
    }

    public void ShieldCollisionSound()
    { 
        int randomShot = Random.Range(0,11);
        AudioClip blaster = MakeSubclip(blasterMultiple,3f*randomShot + 0.1f,3f*randomShot + 1.4f);
        audioSource.PlayOneShot(blaster);
    }

    public void GameOver()
    {
        foreach(AudioSource source in audioSourceList)
        {
            source.Pause();
        }
    }

     /**
  * Creates a sub clip from an audio clip based off of the start time
  * and the stop time. The new clip will have the same frequency source
  * the original.
  */
    private AudioClip MakeSubclip(AudioClip clip, float start, float stop)
    {
        /* Create a new audio clip */
        int frequency = clip.frequency;
        float timeLength = stop - start;
        int samplesLength = (int)(frequency * timeLength);
        AudioClip newClip = AudioClip.Create(clip.name + "-sub", samplesLength, 1, frequency, false);
        /* Create a temporary buffer for the samples */
        float[] data = new float[samplesLength];
        /* Get the data from the original clip */
        clip.GetData(data, (int)(frequency * start));
        /* Transfer the data to the new clip */
        newClip.SetData(data, 0);
        /* Return the sub clip */
        return newClip;
    }


}
