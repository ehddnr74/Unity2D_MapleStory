using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviour
{
    public Transform player;
    public Vector3 offset; // 네임태그의 오프셋 (플레이어의 발 아래 위치)

    private TextMeshProUGUI nameTagText; // 네임태그의 텍스트 컴포넌트

    private void Awake()
    {
        nameTagText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>().transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            // 플레이어 위치에 오프셋을 더하여 네임태그 위치 설정
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position + offset);
            transform.position = screenPosition;
        }
    }

    // 네임태그 텍스트 설정 메서드
    public void SetName(string playerName)
    {
        if (nameTagText != null)
        {
            nameTagText.text = playerName;
        }
    }
}
