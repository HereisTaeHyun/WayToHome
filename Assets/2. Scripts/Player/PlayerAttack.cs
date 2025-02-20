using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // 공격에 관한 메서드 모음
    // 근접 공격, 원거리 직선 공격, 원거리 포물선 공격 계획

    // public 변수
    public float attackPower;

    // private 변수
    private PlayerCtrl playerCtrl;
    private GameObject meleeAttackRange;

    [SerializeField] private float baseAttackPower = -1.0f;

    void Start()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        attackPower = baseAttackPower;

        meleeAttackRange = transform.Find("MeleeAttack").gameObject;
        meleeAttackRange.SetActive(false);
    }

    // 근접 공격, 코루틴으로 공격 범위 콜라이더 생성 후 일정 시간 후 종료, 현재는 0.2초
    public void MeleeAttack()
    {
        if(Input.GetButtonDown("Fire1") && meleeAttackRange.activeSelf == false)
        {
            StartCoroutine(MeleeAttackOn());
        }
    }
    private IEnumerator MeleeAttackOn()
    {
        meleeAttackRange.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        meleeAttackRange.SetActive(false);
    }
}
