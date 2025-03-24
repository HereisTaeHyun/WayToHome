using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        // Player 감지, Submit(E에 할당)입력시 획득
        if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            GameManager.instance.SaveState();
        }
    }
}
