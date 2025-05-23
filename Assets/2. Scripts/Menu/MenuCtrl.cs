using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

public class MenuCtrl : MonoBehaviour
{
    [SerializeField] private GameObject runningPlayer;
    [SerializeField] private GameObject fade;
    private Image fadeImage;
    private float fadeOutTime = 1.5f;
    private const float FADE_OUT_ALPHA = 1.0f;
    private void Start()
    {
        fadeImage = fade.GetComponent<Image>();
    }

    public void StartButton()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        StartCoroutine(ChangeAlpha(fadeImage, FADE_OUT_ALPHA, fadeOutTime));
        yield return new WaitForSeconds(fadeOutTime);
        SceneManager.LoadScene(1);
    }

    public void LoadButton()
    {
        StartCoroutine(LoadGame());
    }

    public IEnumerator LoadGame()
    {
        if(DataManager.dataManager.Load())
        {
            StartCoroutine(ChangeAlpha(fadeImage, FADE_OUT_ALPHA, fadeOutTime));
            yield return new WaitForSeconds(fadeOutTime);
            SceneManager.LoadScene(DataManager.dataManager.playerData.currentStage);
        }
    }


    public void ExitButton()
    {
        StartCoroutine(ExitGame());
    }

    private IEnumerator ExitGame()
    {
        StartCoroutine(ChangeAlpha(fadeImage, FADE_OUT_ALPHA, fadeOutTime));
        yield return new WaitForSeconds(fadeOutTime);
        Application.Quit();
    }
    
    // fade 아웃용 코루틴
    public IEnumerator ChangeAlpha(Image changeTarget, float targetAlpah, float changeTime)
    {
        Color currentColor = changeTarget.color;
        float time = 0.0f;

        // 현재 알파가 목표 알파보다 작은 동안 점진적으로 알파 값 변경
        while (currentColor.a <= targetAlpah)
        {
            time += Time.deltaTime / changeTime;
            currentColor.a = Mathf.Lerp(0.0f, targetAlpah, time);
            changeTarget.color = currentColor;
            yield return null;
        }
    }
}
