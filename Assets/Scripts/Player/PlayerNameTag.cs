using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviour
{
    public Transform player;
    public Vector3 offset; // �����±��� ������ (�÷��̾��� �� �Ʒ� ��ġ)

    private TextMeshProUGUI nameTagText; // �����±��� �ؽ�Ʈ ������Ʈ

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
            // �÷��̾� ��ġ�� �������� ���Ͽ� �����±� ��ġ ����
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position + offset);
            transform.position = screenPosition;
        }
    }

    // �����±� �ؽ�Ʈ ���� �޼���
    public void SetName(string playerName)
    {
        if (nameTagText != null)
        {
            nameTagText.text = playerName;
        }
    }
}
