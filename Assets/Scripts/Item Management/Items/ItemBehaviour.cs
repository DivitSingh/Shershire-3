using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBehaviour : MonoBehaviour
{
    public Item clickedItem;
    public static bool itemActive;
    KnightBehaviour knight;
    GameObject skillsGrid;
    GameObject skillSlot;    

    private void Start()
    {
        knight = GameObject.Find("Knight").GetComponent<KnightBehaviour>();
        skillsGrid = GameObject.Find("Skill Panel/Grid");
        skillSlot = GameObject.Find("Power & Equipment Panel/Slots 02/Skill");
    }

    public void OnItemSlotClick()
    {
        if (KnightBehaviour.canEquip && !KnightBehaviour.canUnequip)
        {            
            if (this.transform.parent.parent.gameObject.name.Contains("Shop"))
            {
                if (clickedItem.Type == Type.Skill && !knight.skills.Contains(clickedItem))
                {
                    if (KnightBehaviour.Trinkets >= clickedItem.Value)
                    {
                        GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(knight.Purchase);
                        KnightBehaviour.Trinkets -= clickedItem.Value;
                        knight.skills.Add(clickedItem);
                        var obj = Instantiate(clickedItem.Sprite, skillsGrid.transform);
                    }
                    else
                    {
                        GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(knight.Failure);
                    }
                }
                else if (clickedItem.Type != Type.Skill)
                {
                    if (KnightBehaviour.Sherlings >= clickedItem.Value)
                    {
                        GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(knight.Purchase);
                        KnightBehaviour.Sherlings -= clickedItem.Value;
                        knight.inventory.AddItem(clickedItem, 1);
                        this.GetComponentInParent<InventoryBehaviour>().inventory.RemoveItem(clickedItem, 1);
                    }
                    else
                    {
                        GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(knight.Failure);
                    }
                }
            }
            else if (clickedItem.Type == Type.Heart)
            {
                if ((KnightBehaviour.currentHP + clickedItem.Value) <= KnightBehaviour.HP)
                {
                    GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(knight.Consume);
                    KnightBehaviour.currentHP += clickedItem.Value;

                    this.GetComponentInParent<InventoryBehaviour>().inventory.RemoveItem(clickedItem, 1);
                }
            }
            else if (clickedItem.Type == Type.Crystal & !itemActive)
            {
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(knight.Consume);
                StartCoroutine(CrystalBuff());
            }
            else if (clickedItem.Type == Type.Equipment)
            {
                string equipmentType = "";

                if (this.gameObject.name.Contains("Weapon"))
                {
                    equipmentType = "Weapon";                    
                }
                else if (this.gameObject.name.Contains("Shield"))
                {
                    equipmentType = "Shield";
                }
                else if (this.gameObject.name.Contains("Helm"))
                {
                    equipmentType = "Helm";
                }
                else if (this.gameObject.name.Contains("Armor"))
                {
                    equipmentType = "Armor";
                }
                else if (this.gameObject.name.Contains("Gauntlets"))
                {
                    equipmentType = "Gauntlets";
                }
                else if (this.gameObject.name.Contains("Greaves"))
                {
                    equipmentType = "Greaves";
                }

                EquipmentSwap(equipmentType);
                KnightBehaviour.HP = knight.currentEquipment["Helm"].HP + knight.currentEquipment["Armor"].HP + knight.currentEquipment["Gauntlets"].HP + knight.currentEquipment["Greaves"].HP
                    + knight.currentEquipment["Weapon"].HP + knight.currentEquipment["Shield"].HP;
                KnightBehaviour.Damage = knight.currentEquipment["Helm"].Damage + knight.currentEquipment["Armor"].Damage + knight.currentEquipment["Gauntlets"].Damage + knight.currentEquipment["Greaves"].Damage
                    + knight.currentEquipment["Weapon"].Damage + knight.currentEquipment["Shield"].Damage;
                KnightBehaviour.currentHP = knight.currentEquipment["Helm"].HP + knight.currentEquipment["Armor"].HP + knight.currentEquipment["Gauntlets"].HP + knight.currentEquipment["Greaves"].HP
                    + knight.currentEquipment["Weapon"].HP + knight.currentEquipment["Shield"].HP;
                KnightBehaviour.Resistance = knight.currentEquipment["Helm"].Resistance + knight.currentEquipment["Armor"].Resistance + knight.currentEquipment["Gauntlets"].Resistance + knight.currentEquipment["Greaves"].Resistance
                    + knight.currentEquipment["Weapon"].Resistance + knight.currentEquipment["Shield"].Resistance;
                KnightBehaviour.Strength = knight.currentEquipment["Helm"].Strength + knight.currentEquipment["Armor"].Strength + knight.currentEquipment["Gauntlets"].Strength + knight.currentEquipment["Greaves"].Strength
                    + knight.currentEquipment["Weapon"].Strength + knight.currentEquipment["Shield"].Strength;
                KnightBehaviour.Recovery = knight.currentEquipment["Helm"].Recovery + knight.currentEquipment["Armor"].Recovery + knight.currentEquipment["Gauntlets"].Recovery + knight.currentEquipment["Greaves"].Recovery
                    + knight.currentEquipment["Weapon"].Recovery + knight.currentEquipment["Shield"].Recovery;
                KnightBehaviour.Speed = knight.currentEquipment["Helm"].Speed + knight.currentEquipment["Armor"].Speed + knight.currentEquipment["Gauntlets"].Speed + knight.currentEquipment["Greaves"].Speed
                    + knight.currentEquipment["Weapon"].Speed + knight.currentEquipment["Shield"].Speed;
            }
            else if (clickedItem.Type == Type.Skill)
            {
                skillSlot.GetComponent<Image>().sprite = clickedItem.Sprite.GetComponent<Image>().sprite;
                KnightBehaviour.skill = clickedItem;
            }
        }
    }

    private void Update()
    {
        if (clickedItem.Type == Type.Heart && !this.gameObject.name.Contains("Slot"))
        {
            this.transform.Rotate(0.0f, 1.00f, 0.00f);
        }
        else if (this.gameObject.name.Contains("Sherling"))
        {
            this.transform.Rotate(0.0f, 0.00f, 1.00f);
        }
    }

    private void EquipmentSwap(string equipmentType)
    {
        if (this.transform.parent.parent.name.Contains("Inventory Panel (Knight)"))
        {
            knight.inventory.RemoveItem(clickedItem, 1);
            knight.inventory.AddItem(knight.currentEquipment[equipmentType], 1);
            knight.currentEquipment[equipmentType] = clickedItem;
            knight.currentSlots[equipmentType].GetComponent<ItemBehaviour>().clickedItem =
                knight.currentEquipment[equipmentType];
            knight.currentSlots[equipmentType].GetComponent<Image>().sprite =
                knight.currentEquipment[equipmentType].Sprite.GetComponent<Image>().sprite;
        }
        else if (this.transform.parent.parent.name.Contains("Power & Equipment Panel"))
        {
            foreach (InventorySlot inventoryItem in knight.inventory.Container)
            {
                if (inventoryItem.item != null && inventoryItem.item.Sprite.gameObject.name.Contains(equipmentType))
                {
                    Item temp = inventoryItem.item;
                    knight.inventory.RemoveItem(inventoryItem.item, 1);
                    knight.inventory.AddItem(knight.currentEquipment[equipmentType], 1);
                    knight.currentEquipment[equipmentType] = temp;
                    knight.currentSlots[equipmentType].GetComponent<ItemBehaviour>().clickedItem =
                        knight.currentEquipment[equipmentType];
                    knight.currentSlots[equipmentType].GetComponent<Image>().sprite =
                        knight.currentEquipment[equipmentType].Sprite.GetComponent<Image>().sprite;

                    break;
                }
            }
        }

        if (equipmentType == "Weapon")
        {
            knight.weapon.GetComponent<MeshRenderer>().enabled = false;
            knight.weapon = GameObject.Find(knight.currentEquipment["Weapon"].name);
            knight.weapon.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (equipmentType == "Shield")
        {
            knight.shield.GetComponent<MeshRenderer>().enabled = false;
            knight.shield = GameObject.Find(knight.currentEquipment["Shield"].name);
            knight.shield.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private IEnumerator CrystalBuff()
    {
        if (this.gameObject.name.Contains("Resistance"))
        {
            KnightBehaviour.Resistance += clickedItem.Buff;
            itemActive = true;

            yield return new WaitForSeconds(10.00f);

            KnightBehaviour.Resistance -= clickedItem.Buff;
            itemActive = false;
            this.GetComponentInParent<InventoryBehaviour>().inventory.RemoveItem(clickedItem, 1);
        }
        else if (this.gameObject.name.Contains("Strength"))
        {
            KnightBehaviour.Strength += clickedItem.Buff;
            itemActive = true;

            yield return new WaitForSeconds(10.00f);

            KnightBehaviour.Strength -= clickedItem.Buff;
            itemActive = false;
            this.GetComponentInParent<InventoryBehaviour>().inventory.RemoveItem(clickedItem, 1);
        }                
        else if (this.gameObject.name.Contains("Recovery"))
        {
            KnightBehaviour.Recovery += clickedItem.Buff;
            itemActive = true;

            yield return new WaitForSeconds(10.00f);

            KnightBehaviour.Recovery -= clickedItem.Buff;
            itemActive = false;
            this.GetComponentInParent<InventoryBehaviour>().inventory.RemoveItem(clickedItem, 1);
        }
        else if (this.gameObject.name.Contains("Speed"))
        {
            KnightBehaviour.Speed += clickedItem.Buff;
            itemActive = true;

            yield return new WaitForSeconds(10.00f);

            KnightBehaviour.Speed -= clickedItem.Buff;
            itemActive = false;
            this.GetComponentInParent<InventoryBehaviour>().inventory.RemoveItem(clickedItem, 1);
        }
    }
}