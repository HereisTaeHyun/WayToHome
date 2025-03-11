using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.LucidEditor;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class Chest : MonoBehaviour
    {
        [FoldoutGroup("Reference")]
        public Animator animator;
        public GameObject itemInChest;
        private GameObject itemSpawnPoint;

        [FoldoutGroup("Runtime"), ShowInInspector, DisableInEditMode]

        void Start()
        {
            itemSpawnPoint = transform.Find("ItemSpawnPoint").gameObject;
        }
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                animator.SetBool("IsOpened", isOpened);
            }
        }
        private bool isOpened;

        [FoldoutGroup("Runtime"),Button("Open"), HorizontalGroup("Runtime/Button")]
        public void Open()
        {
            IsOpened = true;
        }

        [FoldoutGroup("Runtime"), Button("Close"), HorizontalGroup("Runtime/Button")]
        public void Close()
        {
            IsOpened = false;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(IsOpened == true)
            {
                return;
            }
            if(other.gameObject.CompareTag("Player") && Input.GetButton("Submit"))
            {
                Open();
                ItemOut();
            }
        }
        private void ItemOut()
        {
            Instantiate(itemInChest, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
        }
    }
}
