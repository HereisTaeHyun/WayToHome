using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    public GameObject ghost;
    public bool makeGhost;

    private float ghostDelayTime;
    private ObjectPool<GameObject> ghostPool;
    private int maxGhost = 20;
    private float ghostFadeTime = 0.3f;
    private Vector2 dashDir;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        ghostDelayTime = ghostDelay;
        spriteRenderer = GetComponent<SpriteRenderer>();

        UtilityManager.utility.CreateDoNotDestroyPool(ref ghostPool, ghost, maxGhost, maxGhost);
    }

    void Update()
    {
        if (makeGhost == true)
        {
            if (ghostDelayTime > 0)
            {
                ghostDelayTime -= Time.deltaTime;
            }
            else
            {
                spriteRenderer.flipX = PlayerCtrl.player.lastMoveDir.x < 0;

                GameObject currentGhost = UtilityManager.utility.GetFromPool(ghostPool, maxGhost);
                SpriteRenderer ghostSprite = currentGhost.GetComponent<SpriteRenderer>();

                ghostSprite.transform.position = PlayerCtrl.player.transform.position;
                ghostSprite.transform.rotation = PlayerCtrl.player.transform.rotation;

                ghostSprite.sprite = spriteRenderer.sprite;
                ghostSprite.flipX = spriteRenderer.flipX;

                ghostSprite.color = new Color(1, 1, 1, 1.0f);
                StartCoroutine(UtilityManager.utility.ChangeAlpha(ghostSprite, 0f, ghostFadeTime));

                ghostDelayTime = ghostDelay;
                StartCoroutine(GhostReturn(currentGhost));
            }
        }
    }

    private IEnumerator GhostReturn(GameObject currentGhost)
    {
        yield return new WaitForSeconds(ghostFadeTime);
        UtilityManager.utility.ReturnToPool(ghostPool, currentGhost);
    }
}
