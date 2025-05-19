using UnityEngine;

public class BossRoomSensor : PlayerSensor
{
    [SerializeField] AudioClip encounterSFX;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject[] rewards;
    private IDie bossCtrl;

    // 보스룸 오브젝트는 기본적으로 비활성화
    protected override void Start()
    {
        // isEntered 초기화는 부모에서
        base.Start();

        bossCtrl = boss.GetComponent<IDie>();
        boss.SetActive(false);
        leftWall.SetActive(false);
        rightWall.SetActive(false);

        foreach (var reward in rewards)
        {
            reward.SetActive(false);   
        }
    }

     // 보스가 사망했을 때 보상 오브젝트 활성화
    void Update()
    {
        if(bossCtrl.isDie == true)
        {
            leftWall.SetActive(false);
            rightWall.SetActive(false);
            foreach (var reward in rewards)
            {
                reward.SetActive(true);
            }
        }
    }

    // 플레이어가 센서 진입 시 보스룸 요소들 활성화
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && boss != null && isEntered == false)
        {
            isEntered = true;
            UtilityManager.utility.PlaySFX(encounterSFX);
            boss.SetActive(true);
            leftWall.SetActive(true);
            rightWall.SetActive(true);
        }
    }
}
