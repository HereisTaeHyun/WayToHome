using UnityEngine;

public class WaterSplashVisual : MonoBehaviour
{
    private WaterSplash waterSplash;

    void Awake()  // 한 번만 캐싱
    {
        waterSplash = GetComponentInParent<WaterSplash>();
    }

    // === AnimationEvent용 메서드 ===
    public void EnableAttackCollider() => waterSplash?.EnableAttackCollider();
    public void ReturnAfterAnim() => waterSplash?.ReturnAfterAnim();
}
