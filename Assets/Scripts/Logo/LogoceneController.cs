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
        // ���� ������ ��ȯ
        SceneManager.LoadScene(nextScene); // ���⿡ ���� ���� �̸��� �Է��ϼ���
    }
}
