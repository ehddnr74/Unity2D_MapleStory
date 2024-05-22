using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public GameObject target; // ī�޶� ���� ���
    public float moveSpeed; // ī�޶� �󸶳� ���� �ӵ���
    private Vector3 targetPosition; // ����� ���� ��ġ ��

    public Vector2 center;
    public Vector2 size;
    float height;
    float width;

    void Start()
    {
       height = Camera.main.orthographicSize;
       width = height * Screen.width / Screen.height;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center,size);
    }

    // Update is called once per frame
    void Update()
    {
        if(target.gameObject != null)
        {
            targetPosition.Set(target.transform.position.x, target.transform.position.y, transform.position.z);

            this.transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            float lx = size.x * 0.5f - width;
            float clampX = Mathf.Clamp(transform.position.x, -lx + center.x, lx + center.x);

            float ly = size.y * 0.5f - height;
            float clampY = Mathf.Clamp(transform.position.y, -ly + center.y, ly + center.y);

            transform.position = new Vector3(clampX, clampY, -10f);
        }
    }
}
