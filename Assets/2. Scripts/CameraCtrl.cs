using Unity.Cinemachine;
using UnityEngine;
<<<<<<< HEAD

public class CameraCtrl : MonoBehaviour
{
    CinemachineCamera cam;
    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
            
=======
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
>>>>>>> parent of 25e2e44 (이전 버전으로 되돌리기)
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            cam.Follow = player.transform;
        }
    }
}
