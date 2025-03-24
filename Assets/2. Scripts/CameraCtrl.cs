using Unity.Cinemachine;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    CinemachineCamera cam;
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
