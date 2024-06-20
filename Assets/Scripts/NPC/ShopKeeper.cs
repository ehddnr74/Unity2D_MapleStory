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
    public LayerMask layerMask; // Raycast가 적용될 레이어 마스크

    private string npcName = "물건팝니덩";
    public Transform nameTagPosition; // 네임태그를 표시할 위치
    private GameObject nameTagInstance; // 생성된 네임태그 인스턴스
    private TextMeshProUGUI nameTagText; // 네임태그의 텍스트 컴포넌트
    private NameTagPool nameTagPool; // 네임태그 풀링 시스템


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

        // 씬 변경 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 네임태그 생성
        CreateNameTag();
    }

    private void CreateNameTag()
    {
        // 이미 네임태그 인스턴스가 있는지 확인
        if (nameTagInstance == null)
        {
            // 네임태그 풀링 시스템 초기화
            nameTagInstance = nameTagPool.GetNameTag();
            nameTagText = nameTagInstance.GetComponentInChildren<TextMeshProUGUI>();
            nameTagText.text = npcName;

            // 네임태그 위치 업데이트
            UpdateNameTagPosition();

            // 네임태그 오브젝트를 씬 전환 시에도 유지
            DontDestroyOnLoad(nameTagInstance);
        }
    }

    private void UpdateNameTagPosition()
    {
        if (nameTagInstance != null)
        {
            // 네임태그 위치 업데이트
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(nameTagPosition.position);
            nameTagInstance.transform.position = screenPosition;
        }
    }

    private void OnDestroy()
    {
        // 씬 변경 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateNameTagVisibility(scene.name);
    }

    private void UpdateNameTagVisibility(string sceneName)
    {
        // 특정 씬에서만 네임태그를 보이게 설정
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
            // 네임태그 위치 업데이트
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
        // 현재 마우스 위치를 기반으로 PointerEventData 객체를 생성
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            // 현재 마우스의 화면 좌표를 설정합니다.
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        // UI 요소에 대한 Raycast 결과를 저장할 리스트 생성
        List<RaycastResult> results = new List<RaycastResult>();
        // 현재 마우스 위치에서 UI 요소에 대한 Raycast를 수행합니다.
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        // Raycast 결과가 하나라도 있으면, 즉 UI 요소가 감지되었으면 true를 반환
        // 그렇지 않으면 false를 반환.
        return results.Count > 0;
    }
}
