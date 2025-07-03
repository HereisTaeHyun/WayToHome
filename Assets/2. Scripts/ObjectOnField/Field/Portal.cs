using Image = UnityEngine.UI.Image;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    private GameObject screenUI;
    private GameObject screenPanel;
    private Image screenImage;
    private bool usePortal;
    private float fadeOutTime = 1.5f;
    private const float FADE_OUT_ALPHA = 1.0f;
    [SerializeField] AudioClip usePortalSFX;

    void Start()
    {
        usePortal = false;

        screenUI = UIManager.uIManager.transform.Find("ScreenUI").gameObject;
        screenPanel = screenUI.transform.Find("ScreenPanel").gameObject;
        screenImage = screenPanel.GetComponent<Image>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player 감지, Submit(E에 할당)입력시 포탈 이동
        if(other.gameObject.CompareTag("Player") && PlayerCtrl.player.isSubmit == true && usePortal == false)
        {
            StartCoroutine(UsePortal());
        }
    }

    IEnumerator UsePortal()
    {
        // 포탈 사용 플래그 true
        usePortal = true;
        GameManager.instance.usePortal = usePortal;

        // ChangeAlpha로 페이드 아웃 및 SFX 재생
        screenPanel.SetActive(true);
        UtilityManager.utility.PlaySFX(usePortalSFX);
        StartCoroutine(UtilityManager.utility.ChangeAlpha(screenImage, FADE_OUT_ALPHA, fadeOutTime));

        //  플레이어 스탯 저장 후 로드 씬
        DataManager.dataManager.Save();
        yield return new WaitForSeconds(fadeOutTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        screenPanel.SetActive(false);
    }
}
