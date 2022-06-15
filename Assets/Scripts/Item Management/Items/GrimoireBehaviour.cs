using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireBehaviour : MonoBehaviour
{    
    Transform knight;
    GrimoireBehaviour[] grimoires;
    bool grimoireActive;

    public Text titleText;
    public GameObject textPanel;
    public DialogueManager dialogueManager;
    public Dialogue dialogueData;
    public string title;
    public List<string> dialogue = new List<string>();

    // Use this for initialization
    void Start()
    {       
        knight = GameObject.Find("Knight").transform;
        textPanel.SetActive(false);
        grimoires = FindObjectsOfType<GrimoireBehaviour>() as GrimoireBehaviour[];
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GrimoireBehaviour grimoire in grimoires)
        {
            if (Vector3.Distance(knight.position, grimoire.gameObject.transform.position) <= 3.00f)
            {
                grimoireActive = true;
                break;
            }
            else
            {
                grimoireActive = false;
            }
        }

        if (!grimoireActive)
        {
            textPanel.SetActive(false);
        }

        if (this.gameObject.name == "Grimoire (Tutorial)" && Vector3.Distance(knight.position, this.transform.position) < 3.00f && 
            KnightBehaviour.canEquip && !KnightBehaviour.canUnequip)
        {
            if (!textPanel.activeSelf)
            {
                dialogueData.name = title;
                dialogueData.sentences = dialogue;
                dialogueManager.StartDialogue(dialogueData);
            }

            textPanel.SetActive(true);

            if (GameObject.Find("Skill Panel/Grid").transform.childCount == 0)
            {
                KnightBehaviour.Trinkets = 2;
            }
        }
        else if (Vector3.Distance(knight.position, this.transform.position) < 3.00f &&
            Input.GetMouseButtonUp(2) && KnightBehaviour.canEquip && !KnightBehaviour.canUnequip)                
        {
            textPanel.SetActive(true);

            dialogueData.name = title;
            dialogueData.sentences = dialogue;

            dialogueManager.StartDialogue(dialogueData);           
        }       
    }
}
