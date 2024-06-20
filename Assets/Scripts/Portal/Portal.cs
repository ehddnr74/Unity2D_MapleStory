using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad; // 이동할 씬의 이름
    public Transform spawnPositionObject; // 스폰 위치 오브젝트

    private bool playerIsInPortal = false;
    private SceneFader sceneFader;

    private AudioSource audioSource;
    public AudioClip PortalSound;


    private void Start()
    {
        sceneFader = FindObjectOfType<SceneFader>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // 플레이어가 포탈 안에 있고 위쪽 방향키를 눌렀을 때 씬 이동
        if (playerIsInPortal && Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(PlaySoundAndFadeOut());
        }
    }

    private IEnumerator PlaySoundAndFadeOut()
    {
        if (PortalSound != null)
        {
            audioSource.volume = 0.2f;
            audioSource.PlayOneShot(PortalSound);

            if (sceneFader != null)
            {
                yield return sceneFader.FadeOut();
            }

            yield return new WaitForSeconds(PortalSound.length);

            // 플레이어 위치 저장
            if (spawnPositionObject != null)
            {
                Vector3 spawnPosition = spawnPositionObject.position;
                PlayerPrefs.SetFloat("SpawnPosX", spawnPosition.x);
                PlayerPrefs.SetFloat("SpawnPosY", spawnPosition.y);
                PlayerPrefs.SetFloat("SpawnPosZ", spawnPosition.z);
                PlayerPrefs.Save();
            }

            // 씬 전환
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInPortal = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInPortal = false;
        }
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            audioSource.volume = volume;
        }
    }
}
