using UnityEngine;

public class MeteorOff : MonoBehaviour
{
    // 폭파 담담 오브젝트
    // 기본 상태는 투명으로 두다가 폭파시 활성화
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Meteor meteor;
    private Color color;
    public readonly int explodeHash = Animator.StringToHash("Explode");
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        meteor = GetComponentInParent<Meteor>();

        color = spriteRenderer.color;
    }

    public void Explode()
    {
        color.a = 1.0f;
        spriteRenderer.color = color;
        anim.SetTrigger(explodeHash);
    }

    // 중계 함수로서 부모의 동명 함수를 부르기 위한 역할
    private void ReturnAfterAnim()
    {
        color.a = 0.0f;
        spriteRenderer.color = color;
        meteor.ReturnAfterAnim();
    }
}
