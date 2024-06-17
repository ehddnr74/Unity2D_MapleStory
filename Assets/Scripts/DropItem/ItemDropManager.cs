//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ItemDropManager : MonoBehaviour
//{
//    public GameObject itemPrefab; // 아이템 프리팹
//    public Transform dropPoint; // 아이템이 드랍될 위치

//    private ItemDataBase itemDataBase;

//    private void Start()
//    {
//        itemDataBase = FindObjectOfType<ItemDataBase>();
//    }

//    public void DropItem(int itemId)
//    {
//        Item item = itemDataBase.FetchItemByID(itemId);
//        if (item != null)
//        {
//            GameObject droppedItem = Instantiate(itemPrefab, dropPoint.position, Quaternion.identity);

//            // DropItemData 스크립트를 통해 아이템 속성 설정
//            DropItemData dropItemData = droppedItem.GetComponent<DropItemData>();
//            if (dropItemData != null)
//            {
//                dropItemData.Initialize(item,itemPool);
//            }
//        }
//    }
//}
