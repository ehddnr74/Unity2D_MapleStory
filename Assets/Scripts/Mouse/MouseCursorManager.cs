using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    public Texture2D normalCursor;  // 기본 마우스 커서 이미지
    public Texture2D clickCursor;   // 마우스 클릭 시 커서 이미지
    public Vector2 cursorHotspot = Vector2.zero; // 커서의 핫스팟 위치

    public static MouseCursorManager instance;

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
        // 기본 커서로 초기화
        SetCursor(normalCursor);
    }

    private void Update()
    {
        // 마우스 왼쪽 버튼을 누를 때 커서를 클릭 커서로 변경
        if (Input.GetMouseButtonDown(0))
        {
            SetCursor(clickCursor);
        }

        // 마우스 왼쪽 버튼을 뗄 때 커서를 기본 커서로 변경
        if (Input.GetMouseButtonUp(0))
        {
            SetCursor(normalCursor);
        }
    }

    private void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}
