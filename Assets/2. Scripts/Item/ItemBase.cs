using System.Collections;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    // public 변수

    // private 변수
    protected PlayerCtrl playerCtrl;
    protected SpriteRenderer spriteRenderer; // spriteRenderer는 부모 객체에 있음
    [SerializeField] protected float LIFESPAN;
    protected float remainLifespan;
    protected static float BLINK_TIME = 0.3f;
    protected bool isBlink;

    // 생성 후 30초 동안 필드에 존재
    protected virtual void Start()
    {
        remainLifespan = LIFESPAN;
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        isBlink = false;
    }
    protected virtual void Update()
    {
        remainLifespan -= Time.deltaTime;

        // 남은 수명이 10초 이하라면 깜빡거리기
        if(remainLifespan <= 10 && isBlink == false)
        {
            isBlink = true;
            StartCoroutine(BlinkUntilDestroy());
        }

        // 남은 수명이 0 이하면 파괴
        if(remainLifespan <= 0)
        {
            // Destroy(transform.parent.gameObject);
            transform.parent.gameObject.SetActive(false);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        // 충돌 물체가 플레이어일 경우 획득
        if(other.gameObject.CompareTag("Player"))
        {
            playerCtrl = other.GetComponent<PlayerCtrl>();
        }
    }

    // 수명이 얼마 남지 않았다면 깜빡거리기 시작
    protected IEnumerator BlinkUntilDestroy()
    {
        bool isBlink = false;
        Color color = spriteRenderer.color;
        // 아직 시간이 남아 있지만 남은 시간이 적을 경우
        while(remainLifespan >= 0)
        {
            // 이전 상태 깜빡이면 되돌리기, 일반이면 깜빡임 반복시켜서 효과 적용
            if(isBlink == true)
            {
                color.a = 0.0f;
                spriteRenderer.color = color;
                isBlink = false;
            }
            else if(isBlink == false)
            {
                color.a = 1.0f;
                spriteRenderer.color = color;
                isBlink = true;
            }
            yield return new WaitForSeconds(BLINK_TIME);
        }
        // 기본 상태로 초기화
        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
