using System.Collections;
using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] private float breakTime;
    public IEnumerator StartBreak()
    {
        PlayerCtrl.player.canMove = false;
        yield return new WaitForSeconds(breakTime);
        PlayerCtrl.player.canMove = true;
        gameObject.SetActive(false);
    }
}
