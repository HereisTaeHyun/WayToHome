using UnityEngine;
using Unity.Cinemachine;

public class ConfinerChanger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    private CameraCtrl cameraCtrl;
    private Collider2D Collider2D;
    void Start()
    {
        cameraCtrl = cam.GetComponent<CameraCtrl>();
        Collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            cameraCtrl.ConfinerChanger(Collider2D);
        }
    }
}
