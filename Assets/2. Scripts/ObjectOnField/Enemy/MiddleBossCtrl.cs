using System.Collections.Generic;
using UnityEngine;

public class MiddleBossCtrl : EnemyCtrl
{
    // public 변수
    public float distance;
    // private 변수
    [SerializeField] private Transform warpPointSet;
    [SerializeField] private List<Transform> warpPoints = new List<Transform>();
    void Start()
    {
        Init();

        foreach(Transform warpPoint in warpPointSet)
        {
            warpPoints.Add(warpPoint);
        }
        warpPoints.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(target.position, transform.position);

        if(distance > scanningRadius)
        {
            Debug.Log("Warp");
        }
        else
        {
            Debug.Log("Attack");
        }
    }
}
