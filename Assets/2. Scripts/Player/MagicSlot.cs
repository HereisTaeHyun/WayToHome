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
        PlayerCtrl.player.ToggleAttackModeEvent += ToggleAttackMode;
    }
    private void OnDisable()
    {
        PlayerCtrl.player.SelectMagic -= ChangeMagic;
        PlayerCtrl.player.ToggleAttackModeEvent -= ToggleAttackMode;
    }


    private void ChangeMagic(int idx)
    {
        // 마법 활성화 상태에서만 진입 가능
        if (PlayerCtrl.player.isMagic == false)
        {
            return;
        }
        var magicObject = PlayerCtrl.player.playerAttack.usingMagic[idx];
        PlayerMagicBase magic = null;
        if (magicObject != null)
        {
            magic = magicObject.GetComponent<PlayerMagicBase>();
        }
        RefreshMagicIcon(magic);
    }
    private void RefreshMagicIcon(PlayerMagicBase magic)
    {
        if (magic == null)
        {
            magicIcon.enabled = false;
            magicIcon.color = hiddenIcon;
            return;
        }

        magicIcon.enabled = true;
        magicIcon.color = visibleIcon;
        magicIcon.sprite = magic.icon;
    }

    // 마법이 false면 Icon 비활성화
    private void ToggleAttackMode(bool isMagic)
    {
        magicIcon.enabled = PlayerCtrl.player.isMagic;
    }
}
