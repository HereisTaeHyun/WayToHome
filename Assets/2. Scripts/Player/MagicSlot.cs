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
}
