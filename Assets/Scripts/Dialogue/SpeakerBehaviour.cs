using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerBehaviour : MonoBehaviour
{
    #region Animations
    Animator animator;    
    Transform knight;
    #endregion

    SpeakerBehaviour[] speakers;
    bool speakerActive;    

    #region Dialogue
    public GameObject inventoryObject;
    public Inventory inventory;
    public Text titleText;
    public GameObject textPanel;
    public DialogueManager dialogueManager;
    public Dialogue dialogueData;
    public string title;   
    public List<string> dialogue = new List<string>();
    #endregion

    #region Sound    
    public AudioClip Talk;
    #endregion    

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();     
        knight = GameObject.Find("Knight").transform;
        speakers = FindObjectsOfType<SpeakerBehaviour>() as SpeakerBehaviour[];
    }

    // Update is called once per frame
    void Update()
    {                
        Vector3 direction = knight.position - this.transform.position;

        foreach (SpeakerBehaviour speaker in speakers)
        {
            if (Vector3.Distance(knight.position, speaker.gameObject.transform.position) <= 1.50f)
            {
                speakerActive = true;
                break;
            }
            else
            {
                speakerActive = false;
            }
        }

        if (!speakerActive)
        {                                    
            animator.SetBool("IsInteracting", false);
            inventoryObject.SetActive(false);

            foreach (Transform child in inventoryObject.transform)
            {
                if (child.name == "Grid")
                {
                    foreach (Transform grandchild in child)
                    {
                        Destroy(grandchild.gameObject);
                    }
                }
            }

            textPanel.SetActive(false);
        }
        else if (Vector3.Distance(knight.position, this.transform.position) <= 1.50f &&
            Input.GetMouseButtonUp(2) && KnightBehaviour.canEquip && !KnightBehaviour.canUnequip)
        {
            GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Talk);
            animator.SetBool("IsInteracting", true);
            direction.y = 0;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                Quaternion.LookRotation(direction), 0.10f);

            if (inventory != null)
            {
                inventoryObject.GetComponentInChildren<InventoryBehaviour>().inventory = inventory;
                inventoryObject.SetActive(true);
                titleText.text = title;
            }
            
            textPanel.SetActive(true);
            
            dialogueData.name = name;            
            dialogueData.sentences = dialogue;            

            dialogueManager.StartDialogue(dialogueData);
        }     
    }
}