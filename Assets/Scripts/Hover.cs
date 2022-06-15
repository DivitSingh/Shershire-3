using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    [System.NonSerialized]
    public GameObject tooltipPanel;
    [System.NonSerialized]
    public GameObject titleText;
    [System.NonSerialized]
    public GameObject statsText;
    [System.NonSerialized]
    public GameObject descriptionText;
    [System.NonSerialized]
    public GameObject skillText;

    private void Awake()
    {
        if (this.name == "Tooltip Manager")
        {
            tooltipPanel = GameObject.Find("Tooltip Panel");
            titleText = GameObject.Find("Tooltip Panel/Title/Text");
            statsText = GameObject.Find("Tooltip Panel/Stats");
            descriptionText = GameObject.Find("Tooltip Panel/Description");
            skillText = GameObject.Find("Tooltip Panel/Skill Text");
            tooltipPanel.SetActive(false);
        }
    }

    private void Start()
    { 
        if (this.name != "Tooltip Manager")
        {
            tooltipPanel = GameObject.Find("Tooltip Manager").GetComponent<Hover>().tooltipPanel;
            titleText = GameObject.Find("Tooltip Manager").GetComponent<Hover>().titleText;
            statsText = GameObject.Find("Tooltip Manager").GetComponent<Hover>().statsText;
            descriptionText = GameObject.Find("Tooltip Manager").GetComponent<Hover>().descriptionText;
            skillText = GameObject.Find("Tooltip Manager").GetComponent<Hover>().skillText;
        }
    }    

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipPanel.SetActive(true);        

        if (gameObject.GetComponent<ItemBehaviour>() != null)
        {                                
            titleText.GetComponent<Text>().text = gameObject.GetComponent<ItemBehaviour>().clickedItem.name;
            descriptionText.GetComponent<Text>().text = "\n" + gameObject.GetComponent<ItemBehaviour>().clickedItem.Description;

            if (gameObject.GetComponent<ItemBehaviour>().clickedItem.Type == Type.Equipment)
            {
                statsText.GetComponent<Text>().text = GetStats(gameObject.GetComponent<ItemBehaviour>().clickedItem);
            }
            else if (gameObject.GetComponent<ItemBehaviour>().clickedItem.Type == Type.Heart || gameObject.GetComponent<ItemBehaviour>().clickedItem.Type == Type.Crystal)
            {
                if (gameObject.transform.parent.parent.name.Contains("Shop"))
                    statsText.GetComponent<Text>().text = "Value: " + gameObject.GetComponent<ItemBehaviour>().clickedItem.Value + " Sherling(s)" + "\n" +
                    "Buff: " + gameObject.GetComponent<ItemBehaviour>().clickedItem.Buff;
                else
                    statsText.GetComponent<Text>().text = "Buff: " + gameObject.GetComponent<ItemBehaviour>().clickedItem.Buff;
            }   
            else if (gameObject.GetComponent<ItemBehaviour>().clickedItem.Type == Type.Skill)
            {
                if (gameObject.transform.parent.parent.name.Contains("Shop"))
                {
                    skillText.GetComponent<Text>().text = "Value: " + gameObject.GetComponent<ItemBehaviour>().clickedItem.Value + " Trinket(s)" 
                        + "\n\n" + gameObject.GetComponent<ItemBehaviour>().clickedItem.Description;
                    statsText.GetComponent<Text>().text = "";
                    descriptionText.GetComponent<Text>().text = "";
                }
                else
                {
                    skillText.GetComponent<Text>().text = gameObject.GetComponent<ItemBehaviour>().clickedItem.Description;
                    statsText.GetComponent<Text>().text = "";
                    descriptionText.GetComponent<Text>().text = "";
                }
            }
        }
        else
        {
            if (gameObject.name.Contains("Attack"))
            {
                titleText.GetComponent<Text>().text = gameObject.name;

                if (gameObject.name == "Light Attack")
                {
                    statsText.GetComponent<Text>().text = "A swift strike that deals reasonable damage.\nAttack Modifier: 1x Base Damage";
                }
                else
                {
                    statsText.GetComponent<Text>().text = "A hefty blow that deals significant damage.\nAttack Modifier: 2.5x Base Damage";
                }                
            }
            else if (gameObject.name == "Block")
            {
                titleText.GetComponent<Text>().text = gameObject.name;
                statsText.GetComponent<Text>().text = "A defensive action with the shield that prevents damage for the duration it is active.";
            }
            else if (KnightBehaviour.skill != null)
            {                
                titleText.GetComponent<Text>().text = KnightBehaviour.skill.name;                
                skillText.GetComponent<Text>().text = KnightBehaviour.skill.Description;
            }
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
        titleText.GetComponent<Text>().text = "";
        statsText.GetComponent<Text>().text = "";
        descriptionText.GetComponent<Text>().text = "";
        skillText.GetComponent<Text>().text = "";
    }

    private string GetStats(Item clickedItem)
    {
        List<string> statsList = new List<string>();

        if (gameObject.transform.parent.parent.name.Contains("Shop"))
        {            
            statsList.Add("Value: " + clickedItem.Value + " Sherling(s)");            
        }

        if (clickedItem.HP != 0)
        {
            statsList.Add("HP: " + clickedItem.HP);
        }

        if (clickedItem.Damage != 0)
        {
            statsList.Add("Damage: " + clickedItem.Damage);
        }

        if (clickedItem.Resistance != 0)
        {
            statsList.Add("Resistance: " + clickedItem.Resistance);
        }

        if (clickedItem.Strength != 0)
        {
            statsList.Add("Strength: " + clickedItem.Strength);
        }

        if (clickedItem.Recovery != 0)
        {
            statsList.Add("Recovery: " + clickedItem.Recovery);
        }

        if (clickedItem.Speed != 0)
        {
            statsList.Add("Speed: " + clickedItem.Speed);
        }

        return string.Join("\n", statsList);
    }
}
