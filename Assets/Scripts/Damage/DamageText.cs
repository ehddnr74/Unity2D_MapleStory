using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Image[] digitImages; // �� �ڸ����� ǥ���� Image ������Ʈ �迭
    public float lifetime = 1f; // ������ �ؽ�Ʈ�� ���� �ð�
    public float fadeDuration = 0.5f; // ���İ��� �پ��� �ð�

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        // �ִϸ��̼� �ڷ�ƾ ����
        StartCoroutine(FloatAndFade());
    }

    private IEnumerator FloatAndFade()
    {
        // 0.5�� ���� ����
        yield return new WaitForSeconds(lifetime - fadeDuration);

        // ������ 0.5�� ���� ���İ� ���̱�
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // �������� �ؽ�Ʈ�� ��Ȱ��ȭ
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

        // �̹��� �迭 ũ�� ���� �� �ʱ�ȭ
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