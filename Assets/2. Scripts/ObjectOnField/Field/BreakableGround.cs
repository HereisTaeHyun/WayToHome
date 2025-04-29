using System.Collections;
using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] private float breakTime;

    // todo 해당 코루틴을 player를 인자로 받아와 속도 등도 처리 가능하게 변경할 것
    // playerMove를 받아와 그 속도롤 0으로 하는 로직이 필요할 듯
    public IEnumerator StartBreak()
    {
        PlayerCtrl.player.canMove = false;
        yield return new WaitForSeconds(breakTime);
        PlayerCtrl.player.canMove = true;
        gameObject.SetActive(false);
    }
}
