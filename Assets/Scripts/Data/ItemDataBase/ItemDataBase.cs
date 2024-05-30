using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    private List<Item> dataBase = new List<Item>();
    private ItemData itemData;

    void Start()
    {
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadItemData();
            itemData = DataManager.instance.itemData;
            ConstructItemDataBase();
        }
    }

    public Item FetchItemByID(int id)
    {
        for (int i = 0; i < dataBase.Count; i++)
        {
            if (dataBase[i].ID == id)
            {
                return dataBase[i];
            }
        }
        return null;
    }
    
  

    void ConstructItemDataBase()
    {
        foreach (Item item in itemData.items)
        {
            dataBase.Add(new Item(item.ID, item.Name, item.Type, item.Description, item.Price, item.IconPath, item.Stackable));
        }
    }
}



public class Item
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public string IconPath { get; set; }


    [JsonIgnore]
    public Sprite Icon { get; set; }
    public bool Stackable { get; set; }
    public Item(int id, string name, string type, string description, int price, string iconPath, bool stackable)
    {
        this.ID = id;
        this.Name = name;
        this.Type = type;
        this.Description = description;
        this.Price = price;
        this.Icon = Resources.Load<Sprite>("Items/" + iconPath);
        this.IconPath = iconPath;
        this.Stackable = stackable;

    }

    public Item()
    {
        this.ID = -1;
    }
}

