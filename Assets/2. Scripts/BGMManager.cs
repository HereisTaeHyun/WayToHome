using UnityEngine;

public class BGMManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip stage1BGM;
    [SerializeField] private AudioClip dieBGM;
    private bool isDie;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = stage1BGM;
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
