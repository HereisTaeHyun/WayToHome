using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // public 변수
    private Transform target;
    private float damping = 0.3f;

    // private 변수
    private Vector3 vevlocity = Vector3.zero;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // 플레이어 이동 따라가기
    void FixedUpdate() // update, lateupdate는 카메라 상에서 플레이어 떨림 발생하는 것처럼 보임, FixedUpdate()사용
    {
        if(target != null)
        {
            Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref vevlocity, damping);
        }
    }
}
