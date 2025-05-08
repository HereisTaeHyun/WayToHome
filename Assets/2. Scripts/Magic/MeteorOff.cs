using UnityEngine;

public class MeteorOff : MonoBehaviour
{
    private Animator anim;
    private Meteor meteor;
    public readonly int explodeHash = Animator.StringToHash("Explode");
    void Start()
    {
        anim = GetComponent<Animator>();
        meteor = GetComponentInParent<Meteor>();
    }

    public void Explode()
    {
        anim.SetTrigger(explodeHash);
    }

    // 중계 함수로서 부모의 동명 함수를 부르기 위한 역할
    private void ReturnAfterAnim()
    {
        meteor.ReturnAfterAnim();
    }
}
