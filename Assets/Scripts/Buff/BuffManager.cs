using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{
    public GameObject buffPanel;
    public GameObject buffIconPrefab; // 버프 아이콘 프리팹
    private Dictionary<string, GameObject> activeBuffs = new Dictionary<string, GameObject>(); // 활성화된 버프 관리

    private Player player; 
    private QuickSlot quickSlot;

    //private TextMeshProUGUI durationText;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
    }

    public void ActivateBuff(string buffName, Sprite icon, float duration)
    {
        if (!activeBuffs.ContainsKey(buffName))
        {
            GameObject newBuffIcon = Instantiate(buffIconPrefab, buffPanel.transform);
            Image buffImage = newBuffIcon.GetComponent<Image>();
            buffImage.sprite = icon;

            // 자식 텍스트 찾기
            TextMeshProUGUI durationText = newBuffIcon.GetComponentInChildren<TextMeshProUGUI>();
            durationText.text = Mathf.CeilToInt(duration).ToString();

            activeBuffs[buffName] = newBuffIcon;
            StartCoroutine(UpdateText(buffName, durationText, duration));
        }
    }

    private IEnumerator UpdateText(string buffName, TextMeshProUGUI durationText, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = duration - elapsedTime;
            durationText.text = Mathf.CeilToInt(remainingTime).ToString();
            yield return null;
        }

        if (activeBuffs.ContainsKey(buffName))
        {
            Destroy(activeBuffs[buffName]);
            activeBuffs.Remove(buffName);
            if (buffName == "Heist")
            {
                player.moveSpeed = quickSlot.playerOriginMoveSpeed;
                player.jumpForce = quickSlot.playerOriginJumpForce;
            }
            if (buffName == "WindBooster")
            {
                player.attackCoolDown = quickSlot.playerOriginAttackSpeed;
            }
        }
    }
}