using UnityEngine;
using UnityEngine.UI;

public class MagicSlot : MonoBehaviour
{
    [SerializeField] private Image magicIcon;

    public void RefreshMagicIcon(PlayerMagicBase magic)
    {
        if (magic == null)             // null 방어
        {
            magicIcon.enabled = false;
            return;
        }

        magicIcon.enabled = true;
        magicIcon.sprite   = magic.icon;
    }
}
