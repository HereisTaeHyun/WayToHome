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
            
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            cam.Follow = player.transform;
        }
    }

    public void ConfinerChanger(PolygonCollider2D newConfiner)
    {
        confiner.BoundingShape2D = newConfiner;
    }
}
