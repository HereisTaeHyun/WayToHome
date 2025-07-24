using System.Collections;
using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] private float breakTime;

    public IEnumerator StartBreak(Collider2D collision)
    {
        PlayerCtrl.player.playerMove.ForceIdle();
        PlayerCtrl.player.canMove = false;
        yield return new WaitForSeconds(breakTime);
        PlayerCtrl.player.canMove = true;
        gameObject.SetActive(false);
    }
}
