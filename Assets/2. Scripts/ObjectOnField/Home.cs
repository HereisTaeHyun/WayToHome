using System.Collections;
using UnityEngine;

public class Home : MonoBehaviour
{
    private bool isEnd;
    private float distance;
    private GameObject endPoint;
    [SerializeField] private AudioClip getInSFX;
    void Start()
    {
        isEnd = false;
        endPoint = transform.Find("EndPoint").gameObject;
    }

    // 플레이어와 집이 충분히 가까워지면 End 메서드 활성화
    void Update()
    {
        distance = Vector2.Distance(transform.position, PlayerCtrl.player.transform.position);

        if(distance <= 9.0f && isEnd == false)
        {
            StartCoroutine(End());
        }
    }

    private IEnumerator End()
    {
        isEnd = true;
        Debug.Log("End");

        // 플레이어가 집에 들어간 후 페이드 아웃 후 엔딩씬으로
        yield return new WaitForSeconds(2.0f);
    }
}
