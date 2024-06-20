using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneMusicManager : MonoBehaviour
{
    public AudioClip heneMusic;
    public AudioClip heneHuntingGrounds;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMusicForCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "Henesis":
                clipToPlay = heneMusic;
                break;
            case "Henesys Hunting Grounds1":
                clipToPlay = heneHuntingGrounds;
                break;
            default:
                Debug.LogWarning($"No music assigned for scene: {sceneName}");
                break;
        }

        if (clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 0.1f);
        }
    }
}
