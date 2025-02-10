using UnityEngine;

public class TrackingMine : MonoBehaviour
{
    // 인근을 스캐닝하여 Player Layer 감지 시 가까이 가는 방식의 적 엔티티
    // OnCollisionEnter2D하여 Player 충돌시 attack 만큼 데미지
    // attack으로 사살하는 경우 item 랜덤 드랍 기능 필요
    // overlapcicle 사용하여 스캐닝 or 스캐닝 객체 추가? 어디가 나을지 아직 생각 중
    public float MaxHP = 10.0f;
    public float currentHP;
    public float attack = 1.0f;
    public float scanningRadius = 5.0f;
    
    void Start()
    {
        currentHP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
