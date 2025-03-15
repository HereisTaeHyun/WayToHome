using System.Collections;
using Cainos.PixelArtPlatformer_VillageProps;
using UnityEngine;

public class ChestItemSpawn : MonoBehaviour
{
        public GameObject itemInChest;
        private Chest chest;
        private GameObject itemSpawnPoint;

        void Start()
        {
            chest = GetComponent<Chest>();
            itemSpawnPoint = transform.Find("ItemSpawnPoint").gameObject;
        }
        private bool isOpened;

        private void OnTriggerStay2D(Collider2D other)
        {
            if(chest.IsOpened == true)
            {
                return;
            }
            if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
            {
                chest.Open();
                ItemOut();
            }
        }
        private void ItemOut()
        {
            StartCoroutine(ItemDrop());
        }

        IEnumerator ItemDrop()
        {
            yield return new WaitForSeconds(0.3f);
            Instantiate(itemInChest, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
        }
}
