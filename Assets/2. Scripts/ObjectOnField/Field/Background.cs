using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Background : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField][Range(-1.0f, 1.0f)] private float parellaxSpeed = 0.1f;
    private Vector3 cameraStartPos;
    private float distance;
    private Material[] materials;
    private float[] layerMoveSpeed;

    // Background 오브젝트, 마테리얼 받아오기
    void Awake()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("Camera").transform;
        cameraStartPos = cameraTransform.position;

        int backgroundCount = transform.childCount;
        GameObject[] backgrounds = new GameObject[backgroundCount];

        materials = new Material[backgroundCount];
        layerMoveSpeed = new float[backgroundCount];

        for(int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            materials[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        CalMoveSpeedLayer(backgrounds, backgroundCount);
    }

    // 가장 먼 거리로부터 계산하여 가까울 수록 높은 속도를 줌
    private void CalMoveSpeedLayer(GameObject[] backgrounds, int count)
    {
        float farthestDistance = 0;
        for(int i = 0; i < count; i++)
        {
            if(backgrounds[i].transform.position.z - cameraTransform.position.z > farthestDistance)
            {
                farthestDistance = backgrounds[i].transform.position.z - cameraTransform.position.z;
            }
        }

        for(int i = 0; i < count; i++)
        {
            layerMoveSpeed[i] = 1 - (backgrounds[i].transform.position.z - cameraTransform.position.z) / farthestDistance;
        }
    }

    // 마테리얼에 계산 값 적용
    void LateUpdate()
    {
        distance = cameraTransform.position.x - cameraStartPos.x;
        transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, 0);

        for(int i = 0; i < materials.Length; i++)
        {
            float speed = layerMoveSpeed[i] * parellaxSpeed;
            materials[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
        }
    }
}
