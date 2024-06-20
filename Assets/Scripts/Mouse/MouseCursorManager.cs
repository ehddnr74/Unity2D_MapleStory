using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    public Texture2D normalCursor;  // �⺻ ���콺 Ŀ�� �̹���
    public Texture2D clickCursor;   // ���콺 Ŭ�� �� Ŀ�� �̹���
    public Vector2 cursorHotspot = Vector2.zero; // Ŀ���� �ֽ��� ��ġ

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
        // �⺻ Ŀ���� �ʱ�ȭ
        SetCursor(normalCursor);
    }

    private void Update()
    {
        // ���콺 ���� ��ư�� ���� �� Ŀ���� Ŭ�� Ŀ���� ����
        if (Input.GetMouseButtonDown(0))
        {
            SetCursor(clickCursor);
        }

        // ���콺 ���� ��ư�� �� �� Ŀ���� �⺻ Ŀ���� ����
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
