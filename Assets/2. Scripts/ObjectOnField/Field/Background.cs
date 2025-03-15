using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField][Range(-1.0f, 1.0f)] private float parellaxSpeed = 0.1f;
    private Vector3 cameraStartPos;
    private float distance;
    private Material[] materials;
    private float[] layerMoveSpeed;
    void Awake()
    {
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

    void LateUpdate()
    {
        distance = cameraTransform.position.x - cameraStartPos.x;
        transform.position = new Vector3(cameraTransform.position.x, transform.position.y, 0);

        for(int i = 0; i < materials.Length; i++)
        {
            float speed = layerMoveSpeed[i] * parellaxSpeed;
            materials[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
        }
    }
}
