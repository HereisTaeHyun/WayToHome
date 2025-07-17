using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelayTime;

    public GameObject ghost;
    public bool makeGhost;

    private void Start()
    {
        ghostDelayTime = ghostDelay;
    }

    void FixedUpdate()
    {
        if (makeGhost == true)
        {
            if (ghostDelayTime > 0)
            {
                ghostDelayTime -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                ghostDelayTime = ghostDelay;
                Destroy(currentGhost, 1.0f);
            }
        }
    }
}
