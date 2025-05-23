using Image = UnityEngine.UI.Image;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    private Image UIImange;
    private float fadeOutTime = 1.5f;
    private const float FADE_OUT_ALPHA = 1.0f;
    [SerializeField] AudioClip usePortalSFX;

    void Start()
    {
        UIImange = GameObject.FindGameObjectWithTag("GamePlayUI").GetComponent<Image>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player 감지, Submit(E에 할당)입력시 포탈 이동
        if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
        {
            StartCoroutine(UsePortal());
        }
    }

    IEnumerator UsePortal()
    {
        // 포탈 사용 플래그 true
        GameManager.instance.usePortal = true;
        
        // ChangeAlpha로 페이드 아웃 및 SFX 재생
        UIImange.enabled = true;
        UtilityManager.utility.PlaySFX(usePortalSFX);
        StartCoroutine(UtilityManager.utility.ChangeAlpha(UIImange, FADE_OUT_ALPHA, fadeOutTime));

        //  플레이어 스탯 저장 후 로드 씬
        DataManager.dataManager.Save();
        yield return new WaitForSeconds(fadeOutTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        UIImange.enabled = false;
    }
}
