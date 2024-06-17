using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public GameObject damageTextPrefab; // 데미지 텍스트 프리팹
    public Transform canvasTransform; // UI 캔버스 트랜스폼
    public Sprite[] normalDigits; // 일반 데미지 숫자 이미지
    public Sprite[] criticalDigits; // 크리티컬 데미지 숫자 이미지
    public Sprite[] playerTakeDamageDigits; // 플레이어 타격 숫자 이미지
    public int poolSize = 20; // 풀 크기

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
            damageText.Initialize(position, damage, isCritical, normalDigits, criticalDigits);
        }
        else
        {
            GameObject instance = Instantiate(damageTextPrefab, canvasTransform);
            DamageText damageText = instance.GetComponent<DamageText>();
            damageText.Initialize(position, damage, isCritical, normalDigits, criticalDigits);
        }
    }

    public void ShowPlayerDamage(Vector3 position, int damage)
    {
        if (damageTextPool.Count > 0)
        {
            DamageText damageText = damageTextPool.Dequeue();
            damageText.gameObject.SetActive(true);
            damageText.Initialize(position, damage, false, playerTakeDamageDigits, null);
        }
        else
        {
            GameObject instance = Instantiate(damageTextPrefab, canvasTransform);
            DamageText damageText = instance.GetComponent<DamageText>();
            damageText.Initialize(position, damage, false, playerTakeDamageDigits, null);
        }
    }

    public void ReturnToPool(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
        damageTextPool.Enqueue(damageText);
    }
}
