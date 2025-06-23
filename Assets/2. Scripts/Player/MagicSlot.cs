using UnityEngine;
using UnityEngine.UI;

public class MagicSlot : MonoBehaviour
{
    [SerializeField] private Image magicIcon;
    private Color hiddenIcon = new Color32(255, 255, 255, 0);
    private Color visibleIcon = new Color32(255, 255, 255, 255);

    private void OnEnable()
    {
        PlayerCtrl.player.SelectMagic += ChangeMagic;
    }
    private void OnDisable()
    {
        PlayerCtrl.player.SelectMagic -= ChangeMagic;
    }


    private void ChangeMagic(int idx)
    {
        if (idx < 0 || idx >= PlayerCtrl.player.playerAttack.usingMagic.Length)
        {
            return;
        }

        if (PlayerCtrl.player.playerAttack.usingMagic[idx] == null || PlayerCtrl.player.playerAttack.usingMagic.Length == 0)
        {
            return;
        }

        var magic = PlayerCtrl.player.playerAttack.usingMagic[idx].GetComponent<PlayerMagicBase>();
        RefreshMagicIcon(magic);
    }
    private void RefreshMagicIcon(PlayerMagicBase magic)
    {
        if (magic == null)             // null 방어
        {
            magicIcon.enabled = false;
            magicIcon.color = hiddenIcon;
            return;
        }

        magicIcon.enabled = true;
        magicIcon.color = visibleIcon;
        magicIcon.sprite = magic.icon;
    }
}
