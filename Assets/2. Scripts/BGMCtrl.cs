using UnityEngine;

public class BGMCtrl : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip stageBGM;
    [SerializeField] private AudioClip gameoverBGM;
    [SerializeField] private AudioClip bossBGM;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayStageBGM();
    }

    public void PlayStageBGM()
    {
        audioSource.clip = stageBGM;
        audioSource.Play();
    }

    public void PlayGameOverBGM()
    {
        audioSource.clip = gameoverBGM;
        audioSource.Play();
    }

    public void PlayBossBGM()
    {
        audioSource.clip = bossBGM;
        audioSource.Play();
    }
}
