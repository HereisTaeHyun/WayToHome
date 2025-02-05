using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // 퍼블릭 변수
    public Transform target;
    public float damping = 0.5f;

    // 프라이빗 변수
    private Vector3 vevlocity = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref vevlocity, damping);
    }
}
