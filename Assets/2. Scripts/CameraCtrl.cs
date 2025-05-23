using Unity.Cinemachine;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private CinemachineCamera cam;
    private CinemachineConfiner2D confiner;

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        confiner = cam.GetComponent<CinemachineConfiner2D>();

        cam.Follow = PlayerCtrl.player.transform;
    }

    public void ConfinerChanger(Collider2D newConfiner)
    {
        confiner.BoundingShape2D = newConfiner;
    }
}
