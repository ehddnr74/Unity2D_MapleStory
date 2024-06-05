using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoceneController : MonoBehaviour
{
    public string nextScene;

    public float animationLength = 4.7f;

    private void Start()
    {
        Invoke("NextScene", animationLength);
    }

    void NextScene()
    {
        // 다음 씬으로 전환
        SceneManager.LoadScene(nextScene); // 여기에 다음 씬의 이름을 입력하세요
    }
}
