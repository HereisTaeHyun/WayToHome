using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    private Collider2D portalColl;
    [SerializeField] private GameObject UIPanel;
    private Image UIImange;
    private float alphaChangeTime = 1.5f;
    private const float FADE_OUT_ALPHA = 1.0f;

    void Start()
    {
        portalColl = GetComponent<Collider2D>();
        UIImange = UIPanel.GetComponent<Image>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player 감지, Submit(E에 할당)입력시 포탈 이동
        if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            Debug.Log("포탈 사용");
            StartCoroutine(UsePortal());
        }
    }

    IEnumerator UsePortal()
    {
        // ChangeAlpha로 페이드 아웃 후 로드 씬
        UIImange.enabled = true;
        StartCoroutine(UtilityManager.utility.ChangeAlpha(UIImange, FADE_OUT_ALPHA, alphaChangeTime));
        yield return new WaitForSeconds(alphaChangeTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        UIImange.enabled = false;
    }
}
