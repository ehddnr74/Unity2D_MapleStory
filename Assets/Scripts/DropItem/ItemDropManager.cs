//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ItemDropManager : MonoBehaviour
//{
//    public GameObject itemPrefab; // ������ ������
//    public Transform dropPoint; // �������� ����� ��ġ

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

//            // DropItemData ��ũ��Ʈ�� ���� ������ �Ӽ� ����
//            DropItemData dropItemData = droppedItem.GetComponent<DropItemData>();
//            if (dropItemData != null)
//            {
//                dropItemData.Initialize(item,itemPool);
//            }
//        }
//    }
//}
