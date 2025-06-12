using UnityEngine;

public class WaterSplashVisual : MonoBehaviour
{
    private WaterSplash waterSplash;

    void Awake()
    {
        waterSplash = GetComponentInParent<WaterSplash>();
    }

    // === AnimationEvent용 메서드 ===
    public void EnableAttackCollider() => waterSplash?.EnableAttackCollider();
    public void ReturnAfterAnim() => waterSplash?.ReturnAfterAnim();
    public void DisableAttackCollider() => waterSplash?.DisableAttackCollider();
}
