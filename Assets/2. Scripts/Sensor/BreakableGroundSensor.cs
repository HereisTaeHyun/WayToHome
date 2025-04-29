using UnityEngine;

public class BreakableGroundSensor : PlayerSensor
{
    [SerializeField] private GameObject targetBreakableGround;
    private BreakableGround breakableGround;
    protected override void Start()
    {
        // isEntered 초기화는 부모에서
        base.Start();
        breakableGround = targetBreakableGround.GetComponent<BreakableGround>();
    }

    // 플레이어가 센서 진입 땅 파괴
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")&& isEntered == false)
        {
            isEntered = true;
            StartCoroutine(breakableGround.StartBreak());
        }
    }
}
