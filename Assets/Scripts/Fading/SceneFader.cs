using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    //public CanvasGroup canvasGroup;
    public static SceneFader instance;
    public Image fadeImage;
    public float fadeDuration = 1.0f; // 페이딩 시간

    private void Awake()
    {
        // 초기 설정
        //canvasGroup.blocksRaycasts = false;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //StartCoroutine(FadeIn());
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 등록
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 해제
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }
    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1.0f - (elapsedTime / fadeDuration));
            fadeImage.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}