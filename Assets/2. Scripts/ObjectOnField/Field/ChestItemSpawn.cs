using System.Collections;
using Cainos.PixelArtPlatformer_VillageProps;
using UnityEngine;

public class ChestItemSpawn : MonoBehaviour
{
        public GameObject itemInChest;
        private Chest chest;
        private GameObject itemSpawnPoint;
        private bool isOpened = false;

        void Start()
        {
            chest = GetComponent<Chest>();
            itemSpawnPoint = transform.Find("ItemSpawnPoint").gameObject;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit") && isOpened == false)
            {
                isOpened = true;
                chest.Open();
                if(itemInChest != null)
                {
                    StartCoroutine(ItemDrop());
                }
            }
        }

        IEnumerator ItemDrop()
        {
            yield return new WaitForSeconds(0.3f);
            Instantiate(itemInChest, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
        }
}
