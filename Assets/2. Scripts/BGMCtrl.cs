using UnityEngine;

public class BGMCtrl : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip stageBGM;
    [SerializeField] private AudioClip dieBGM;
    private bool isDie;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = stageBGM;
        audioSource.Play();

        isDie = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.readIsGameOver == true && isDie == false)
        {
            isDie = true;
            audioSource.clip = dieBGM;
            audioSource.Play();
        }
    }
}
