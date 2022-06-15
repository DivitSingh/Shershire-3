using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBehaviour : MonoBehaviour
{
    public GameObject panel;    
    public AudioClip Collect;
    public Inventory inventory;
    GameObject powerPanel;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < inventory.Container.Length; i++)
        {
            if (inventory.Container[i].item != null)
            {
                var obj = Instantiate(inventory.Container[i].item.Sprite, this.transform);
                obj.GetComponentInChildren<Text>().text = inventory.Container[i].amount.ToString("n0");
                obj.GetComponentInChildren<ItemBehaviour>().clickedItem = inventory.Container[i].item;
                itemsDisplayed.Add(inventory.Container[i], obj);
            }
        }

        foreach (Transform child in GameObject.Find("Power & Equipment Panel").transform)
        {
            if (child.gameObject.name == "Panel")
            {
                powerPanel = child.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ItemBehaviour.itemActive || (!KnightBehaviour.canEquip && KnightBehaviour.canUnequip))
        {
            panel.SetActive(true);
            powerPanel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
            powerPanel.SetActive(false);
        }

        for (int i = 0; i < inventory.Container.Length; i++)
        {
            if (itemsDisplayed.ContainsKey(inventory.Container[i]))
            {
                if (inventory.Container[i].amount > 0 && itemsDisplayed[inventory.Container[i]] != null)
                {
                    itemsDisplayed[inventory.Container[i]].GetComponent<Image>().sprite = inventory.Container[i].item.Sprite.GetComponent<Image>().sprite;
                     
                    if (itemsDisplayed[inventory.Container[i]].GetComponent<ItemBehaviour>().clickedItem == null ||
                        itemsDisplayed[inventory.Container[i]].GetComponent<ItemBehaviour>().clickedItem.Type != Type.Skill)
                        itemsDisplayed[inventory.Container[i]].GetComponentInChildren<Text>().text = inventory.Container[i].amount.ToString("n0");
                    else
                        itemsDisplayed[inventory.Container[i]].GetComponentInChildren<Text>().text = "";
                    itemsDisplayed[inventory.Container[i]].GetComponentInChildren<ItemBehaviour>().clickedItem = inventory.Container[i].item;
                }
                else
                {
                    GameObject obj = itemsDisplayed[inventory.Container[i]].gameObject;
                    itemsDisplayed.Remove(inventory.Container[i]);
                    Destroy(obj);
                }
            }
            else
            {
                if (inventory.Container[i].item != null)
                {
                    var obj = Instantiate(inventory.Container[i].item.Sprite, this.transform);                    
                    obj.GetComponentInChildren<Text>().text = inventory.Container[i].amount.ToString("n0");
                    itemsDisplayed.Add(inventory.Container[i], obj);
                }
            }
        }
    }
}
