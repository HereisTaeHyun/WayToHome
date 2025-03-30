using UnityEngine;
using Unity.Cinemachine;

public class ConfinerChanger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    private CameraCtrl cameraCtrl;
    private PolygonCollider2D polygonCollider2D;
    void Start()
    {
        cameraCtrl = cam.GetComponent<CameraCtrl>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            cameraCtrl.ConfinerChanger(polygonCollider2D);
        }
    }
}
