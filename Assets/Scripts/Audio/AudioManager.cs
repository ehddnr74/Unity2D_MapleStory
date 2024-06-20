using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip, float volume)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
            Debug.Log($"Playing music: {clip.name}");
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
        Debug.Log("Music stopped");
    }
}
