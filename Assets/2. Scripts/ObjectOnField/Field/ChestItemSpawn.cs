using System.Collections;
using Cainos.PixelArtPlatformer_VillageProps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestItemSpawn : MonoBehaviour
{
        public GameObject itemInChest;

        [SerializeField] private string chestID;
        private Chest chest;
        private GameObject itemSpawnPoint;
        private bool isOpened = false; // OnTriggerStay2D기에 프레임 연속 입력 방지 위해 플래그 필요

        void Start()
        {
            chestID = $"{SceneManager.GetActiveScene().name}_{gameObject.name}";
            chest = GetComponent<Chest>();
            itemSpawnPoint = transform.Find("ItemSpawnPoint").gameObject;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Player") && PlayerCtrl.player.isSubmit == true && isOpened == false)
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
            DataManager.dataManager.playerData.openedChests.Add(chestID);
            UtilityManager.utility.SetItemFromPool(itemSpawnPoint.transform, itemInChest);
        }
}
