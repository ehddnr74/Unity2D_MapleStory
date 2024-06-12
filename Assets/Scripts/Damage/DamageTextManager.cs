using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public GameObject damageTextPrefab; // ������ �ؽ�Ʈ ������
    public Transform canvasTransform; // UI ĵ���� Ʈ������
    public Sprite[] normalDigits; // �Ϲ� ������ ���� �̹���
    public Sprite[] criticalDigits; // ũ��Ƽ�� ������ ���� �̹���
    public int poolSize = 20; // Ǯ ũ��

    private Queue<DamageText> damageTextPool = new Queue<DamageText>();

    public static DamageTextManager instance;

    private void Awake()
    {
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
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject instance = Instantiate(damageTextPrefab, canvasTransform);
            instance.SetActive(false);
            damageTextPool.Enqueue(instance.GetComponent<DamageText>());
        }
    }
    public void ShowDamage(Vector3 position, int damage, bool isCritical)
    {
        if (damageTextPool.Count > 0)
        {
            DamageText damageText = damageTextPool.Dequeue();
            damageText.gameObject.SetActive(true);
            damageText.transform.position = Camera.main.WorldToScreenPoint(position);
            damageText.SetDamage(damage, isCritical, normalDigits, criticalDigits);
        }
        else
        {
            GameObject instance = Instantiate(damageTextPrefab, canvasTransform);
            DamageText damageText = instance.GetComponent<DamageText>();
            damageText.SetDamage(damage, isCritical, normalDigits, criticalDigits);
            instance.transform.position = Camera.main.WorldToScreenPoint(position);
        }
    }

    public void ReturnToPool(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
        damageTextPool.Enqueue(damageText);
    }
}