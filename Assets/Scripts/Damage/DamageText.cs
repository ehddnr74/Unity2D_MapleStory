using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Image[] digitImages; // 각 자리수를 표시할 Image 컴포넌트 배열
    public float lifetime = 1f; // 데미지 텍스트의 지속 시간
    public float fadeDuration = 0.5f; // 알파값이 줄어드는 시간

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        // 애니메이션 코루틴 시작
        StartCoroutine(FloatAndFade());
    }

    private IEnumerator FloatAndFade()
    {
        // 0.5초 동안 유지
        yield return new WaitForSeconds(lifetime - fadeDuration);

        // 나머지 0.5초 동안 알파값 줄이기
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // 마지막에 텍스트를 비활성화
        canvasGroup.alpha = 0f;
        Deactivate();
    }


    private void OnDisable()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetDamage(int damage, bool isCritical, Sprite[] normalDigits, Sprite[] criticalDigits)
    {
        string damageStr = damage.ToString();
        Sprite[] digits = isCritical ? criticalDigits : normalDigits;

        // 이미지 배열 크기 조정 및 초기화
        for (int i = 0; i < digitImages.Length; i++)
        {
            if (i < damageStr.Length)
            {
                digitImages[i].sprite = digits[int.Parse(damageStr[i].ToString())];
                digitImages[i].gameObject.SetActive(true);
            }
            else
            {
                digitImages[i].gameObject.SetActive(false);
            }
        }
    }
}