using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] AudioClip saveSfx;
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            // 스탯 저장 후 스폰 포인트를 해당 포인트로 설정
            GameManager.instance.SavePlayerStat();
            GameManager.instance.SetSpawnPos(gameObject.transform.position);
            UtilityManager.utility.PlaySFX(saveSfx);
        }
    }
}
