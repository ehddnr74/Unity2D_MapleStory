using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public GameObject target; // 카메라가 따라갈 대상
    public float moveSpeed; // 카메라가 얼마나 빠른 속도로
    private Vector3 targetPosition; // 대상의 현재 위치 값

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
