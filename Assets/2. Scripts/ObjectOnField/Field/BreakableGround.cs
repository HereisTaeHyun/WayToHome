using System.Collections;
using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] private float breakTime;
    private PlayerMove playerMove;

    public IEnumerator StartBreak(Collider2D collision)
    {
        playerMove = collision.GetComponent<PlayerMove>();

        playerMove.ForceIdle();
        PlayerCtrl.player.canMove = false;
        yield return new WaitForSeconds(breakTime);
        PlayerCtrl.player.canMove = true;
        gameObject.SetActive(false);
    }
}
