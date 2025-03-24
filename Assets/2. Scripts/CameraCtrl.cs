using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraCtrl : MonoBehaviour
{
    private CinemachineCamera cam;
    // Enable되면 함수 구독 및 UI 초기화
    void OnEnable()
    {
        SceneManager.sceneLoaded += CameraInit;
    }
    // Disable되면 함수 구독 해제
    void OnDisable()
    {
        SceneManager.sceneLoaded -= CameraInit;
    }

    private void CameraInit(Scene scene, LoadSceneMode mode)
    {
        cam = GetComponent<CinemachineCamera>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            cam.Follow = player.transform;
        }
    }
}
