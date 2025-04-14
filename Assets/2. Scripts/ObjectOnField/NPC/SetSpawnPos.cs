using UnityEngine;

public class SetSpawnPos : MonoBehaviour
{
    [SerializeField] AudioClip saveSfx;
    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            GameManager.instance.SetSpawnPos(gameObject.transform.position);
            UtilityManager.utility.PlaySFX(saveSfx);
        }
    }
}
