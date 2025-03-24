using Unity.Cinemachine;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private CinemachineCamera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            cam.Follow = player.transform;
        }
    }
}
