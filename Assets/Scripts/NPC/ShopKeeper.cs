using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ShopKeeper : MonoBehaviour
{
    private Shop shop;

    private AudioSource audioSource;
    public AudioClip OpenShopSound;
    public LayerMask layerMask; // Raycast�� ����� ���̾� ����ũ

    private string npcName = "�����˴ϵ�";
    public Transform nameTagPosition; // �����±׸� ǥ���� ��ġ
    private GameObject nameTagInstance; // ������ �����±� �ν��Ͻ�
    private TextMeshProUGUI nameTagText; // �����±��� �ؽ�Ʈ ������Ʈ
    private NameTagPool nameTagPool; // �����±� Ǯ�� �ý���


    public static ShopKeeper instance;
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

    void Start()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        nameTagPool = GameObject.Find("NameTagCanvas").GetComponent<NameTagPool>();
        audioSource = gameObject.AddComponent<AudioSource>();

        // �� ���� �̺�Ʈ ���
        SceneManager.sceneLoaded += OnSceneLoaded;

        // �����±� ����
        CreateNameTag();
    }

    private void CreateNameTag()
    {
        // �̹� �����±� �ν��Ͻ��� �ִ��� Ȯ��
        if (nameTagInstance == null)
        {
            // �����±� Ǯ�� �ý��� �ʱ�ȭ
            nameTagInstance = nameTagPool.GetNameTag();
            nameTagText = nameTagInstance.GetComponentInChildren<TextMeshProUGUI>();
            nameTagText.text = npcName;

            // �����±� ��ġ ������Ʈ
            UpdateNameTagPosition();

            // �����±� ������Ʈ�� �� ��ȯ �ÿ��� ����
            DontDestroyOnLoad(nameTagInstance);
        }
    }

    private void UpdateNameTagPosition()
    {
        if (nameTagInstance != null)
        {
            // �����±� ��ġ ������Ʈ
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(nameTagPosition.position);
            nameTagInstance.transform.position = screenPosition;
        }
    }

    private void OnDestroy()
    {
        // �� ���� �̺�Ʈ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateNameTagVisibility(scene.name);
    }

    private void UpdateNameTagVisibility(string sceneName)
    {
        // Ư�� �������� �����±׸� ���̰� ����
        if (sceneName == "Henesis")
        {
            if (nameTagInstance != null)
            {
                nameTagInstance.SetActive(true);
                gameObject.SetActive(true);
            }
        }
        else
        {
            if (nameTagInstance != null)
            {
                nameTagInstance.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (nameTagInstance != null && nameTagInstance.activeSelf)
        {
            // �����±� ��ġ ������Ʈ
            UpdateNameTagPosition();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIObject())
            {
                return;
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                HandleShopInteraction();
            }
        }
    }

    private void HandleShopInteraction()
    {
        if (shop != null)
        {
            PlaySound(OpenShopSound);
            audioSource.volume = 0.2f;
            shop.visibleShop = !shop.visibleShop;
            shop.shopParentPanel.SetActive(shop.visibleShop);
            shop.UpdateShopInventory();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private bool IsPointerOverUIObject()
    {
        // ���� ���콺 ��ġ�� ������� PointerEventData ��ü�� ����
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            // ���� ���콺�� ȭ�� ��ǥ�� �����մϴ�.
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        // UI ��ҿ� ���� Raycast ����� ������ ����Ʈ ����
        List<RaycastResult> results = new List<RaycastResult>();
        // ���� ���콺 ��ġ���� UI ��ҿ� ���� Raycast�� �����մϴ�.
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        // Raycast ����� �ϳ��� ������, �� UI ��Ұ� �����Ǿ����� true�� ��ȯ
        // �׷��� ������ false�� ��ȯ.
        return results.Count > 0;
    }
}
