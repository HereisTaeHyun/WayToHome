using UnityEngine;

public class KillZone : MonoBehaviour
{
    // 진입 시 사망하는 곳
    // 일단 Destroy로 구현 이후 PalyerCtrl이나 PlayerMove에 PlayerDie 메서드 추가하면 해당 메서드 실행으로 변경
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"OnKillZone : {other}");
        Destroy(other.gameObject);    
    }
}
