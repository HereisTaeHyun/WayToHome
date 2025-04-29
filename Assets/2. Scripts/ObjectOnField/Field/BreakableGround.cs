using System.Collections;
using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] private float breakTime;
    public IEnumerator StartBreak()
    {
        Debug.Log("붕괴 시작");
        yield return new WaitForSeconds(breakTime);
        Debug.Log("붕괴 종료");
    }
}
