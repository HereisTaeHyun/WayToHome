using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    Collider2D portalColl;
    void Start()
    {
        portalColl = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player 감지, Submit(E에 할당)입력시 포탈 이동
        if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            UsePortal();
        }
    }

    private void UsePortal()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
