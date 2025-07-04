using UnityEngine;

public class SavePoint : MonoBehaviour
{
    // stay 중 꾹 눌러서 프레임마다 저장되는 것 막기, 사운드 너무 이상함
    private bool isSaved = false;
    [SerializeField] AudioClip saveSfx;
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && PlayerCtrl.player.isSubmit == true && isSaved == false)
        {
            // 스탯 저장 후 스폰 포인트를 해당 포인트로 설정
            isSaved = true;

            PlayerCtrl.player.currentHP = PlayerCtrl.player.maxHP;
            PlayerCtrl.player.currentMana = PlayerCtrl.player.maxMana;
            PlayerCtrl.player.DisplayHP();
            PlayerCtrl.player.DisplayMana();
            
            GameManager.instance.SetSpawnPos(gameObject.transform.position);
            DataManager.dataManager.Save();
            UtilityManager.utility.PlaySFX(saveSfx);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isSaved = false;
        }
    }
}
