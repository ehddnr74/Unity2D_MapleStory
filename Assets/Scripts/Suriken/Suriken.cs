using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public Player player;
    public ShurikenManager shurikenManager; 

    public float speed = 10f;
    public float lifetime = 0.5f; // 수리검이 발사된 후 풀에 반환되기까지의 시간

    private void Start()
    {
        player = FindObjectOfType<Player>();
        shurikenManager = FindObjectOfType<ShurikenManager>();

        // 일정 시간이 지난 후 수리검을 풀에 반환하는 코루틴 시작
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    void Update()
    {
        this.transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(lifetime); // 일정 시간(1초) 대기

        // 수리검을 풀에 반환
        shurikenManager.ReturnSurikenToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            shurikenManager.ReturnSurikenToPool(gameObject);
        }
    }
}
