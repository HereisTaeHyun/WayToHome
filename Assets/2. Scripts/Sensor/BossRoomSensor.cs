using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRoomSensor : PlayerSensor
{
    [SerializeField] AudioClip encounterSFX;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject[] rewards;
    [SerializeField] BGMCtrl bGMCtrl;
    private BossCtrl bossCtrl;
    private int enemyID;
    public int readEnemyID {get {return enemyID;}}

    // 보스룸 오브젝트는 기본적으로 비활성화
    protected override void Start()
    {
        // isEntered 초기화는 부모에서
        base.Start();
        enemyID = Animator.StringToHash($"{SceneManager.GetActiveScene().name}_{gameObject.name}");

        bossCtrl = boss.GetComponent<BossCtrl>();
        boss.SetActive(false);

        if (leftWall != null && rightWall != null)
        {
            leftWall.SetActive(false);
            rightWall.SetActive(false);
        }

        foreach (var reward in rewards)
        {
            reward.SetActive(false);
        }

        if (DataManager.dataManager.playerData.diedEnemy.Contains(enemyID))
        {
            gameObject.SetActive(false);
        }
    }

     // 보스가 사망했을 때 보상 오브젝트 활성화
    public void SetBossClear()
    {
        bGMCtrl.PlayStageBGM();
        DataManager.dataManager.playerData.diedEnemy.Add(enemyID);
        if (leftWall != null && rightWall != null)
        {
            leftWall.SetActive(false);
            rightWall.SetActive(false);
        }
        foreach (var reward in rewards)
        {
            reward.SetActive(true);
        }
    }

    // 플레이어가 센서 진입 시 보스룸 요소들 활성화
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && boss != null && isEntered == false)
        {
            isEntered = true;

            UtilityManager.utility.PlaySFX(encounterSFX);
            bGMCtrl.PlayBossBGM();

            boss.SetActive(true);
            bossCtrl.PlayerEntered();
            if (leftWall != null && rightWall != null)
            {
                leftWall.SetActive(true);
                rightWall.SetActive(true);
            }
        }
    }
}
