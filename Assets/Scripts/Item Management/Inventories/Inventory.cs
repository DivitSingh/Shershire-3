using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public InventorySlot[] Container = new InventorySlot[10];

    public void AddItem (Item item, int amount)
    {
        bool hasItem = false;

        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].item == item)
            {
                Container[i].amount += amount;
                hasItem = true;
                break;
            }            
        }

        if (!hasItem)
        {
            SetEmptySlot(item, amount);
            return;
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].item == item)
            {
                Container[i].amount -= amount;                

                if (Container[i].amount < 1)
                {
                    Container[i].item = null;
                    Container[i].amount = 0;
                }    

                break;
            }
        }
    }

    public InventorySlot SetEmptySlot(Item item, int amount)
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].item == null)
            {                
                Container[i].item = item;                
                Container[i].amount = amount;
                return Container[i];
            }
        }

        return null;
    }
}


[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int amount;

    public InventorySlot(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}