using UnityEngine;

public class VendingMachineCtrl : MonoBehaviour
{
    public GameObject vendingText;
    void Start()
    {
        vendingText.SetActive(false);
    }
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            vendingText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            vendingText.SetActive(false);
        }
    }
}
