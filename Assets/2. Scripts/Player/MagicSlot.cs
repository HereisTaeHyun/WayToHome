using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MagicSlot : MonoBehaviour
{
    [SerializeField] private Image magicIcon;

    private void OnEnable()
    {
        PlayerCtrl.player.SelectMagic += ChangeMagic;          // 이벤트 구독
    }
    private void OnDisable()
    {
        PlayerCtrl.player.SelectMagic -= ChangeMagic;          // 해제
    }


    private void ChangeMagic(int idx)
    {
        if (idx < 0 || idx >= PlayerCtrl.player.playerAttack.usingMagic.Length)
        {
            return;
        }

        if (PlayerCtrl.player.playerAttack.usingMagic[idx] == null || PlayerCtrl.player.playerAttack.usingMagic.Length == 0)              // 아직 미구매
        {
            return;
        }

        var magic = PlayerCtrl.player.playerAttack.usingMagic[idx].GetComponent<PlayerMagicBase>();
        RefreshMagicIcon(magic);
    }
    public void RefreshMagicIcon(PlayerMagicBase magic)
    {
        if (magic == null)             // null 방어
        {
            magicIcon.enabled = false;
            return;
        }

        magicIcon.enabled = true;
        magicIcon.sprite = magic.icon;
    }
}
