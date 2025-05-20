using System;
using System.Collections;
using UnityEngine;

public class Home : MonoBehaviour
{
    private bool isPlayerNear;
    private float distance;
    private GameObject endPoint;
    [SerializeField] private AudioClip getInSFX;
    
    void Start()
    {
        isPlayerNear = false;
        endPoint = transform.Find("EndPoint").gameObject;
    }

    // 플레이어와 집이 충분히 가까워지면 End 메서드 활성화
    void Update()
    {
        distance = Vector2.Distance(transform.position, PlayerCtrl.player.transform.position);

        if(distance <= 10.0f && isPlayerNear == false)
        {
            StartCoroutine(End());
        }
    }

    private IEnumerator End()
    {
        isPlayerNear = true;

        PlayerCtrl.player.canMove = false;
        PlayerCtrl.player.playerMove.ForceIdle();
        yield return new WaitForSeconds(2.0f);

        // 엔딩 포인트인 문으로 이동
        while (Mathf.Abs(PlayerCtrl.player.transform.position.x - endPoint.transform.position.x) > 0.1f)
        {
            Vector2 targetPoint = new Vector2(endPoint.transform.position.x, PlayerCtrl.player.transform.position.y);
            
            PlayerCtrl.player.playerMove.playerAnim.SetFloat("Speed", 1.0f);
            PlayerCtrl.player.state = PlayerCtrl.State.Move;
            PlayerCtrl.player.transform.position = Vector2.MoveTowards(PlayerCtrl.player.transform.position, targetPoint, Time.deltaTime * 3.0f);
            yield return null;
        }

        // 엔딩 포인트에 도착
        PlayerCtrl.player.playerMove.playerAnim.SetFloat("Speed", 0.0f);
        yield return new WaitForSeconds(2.0f);
        UtilityManager.utility.PlaySFX(getInSFX);
        PlayerCtrl.player.spriteRenderer.sortingOrder = -1;

        // 플레이어가 집에 들어간 후 엔딩
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.End();
    }
}
