using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuesterBehaviour : MonoBehaviour
{
    #region Animations
    Animator animator;
    Transform knight;
    #endregion

    QuesterBehaviour[] questers;
    bool questerActive;

    #region Dialogue    
    public Text titleText;
    public GameObject textPanel;
    public DialogueManager dialogueManager;
    public Dialogue dialogueData;
    public string title;
    
    public List<string> questNames = new List<string>();
    [TextArea(1, 100)]
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
        questers = FindObjectsOfType<QuesterBehaviour>() as QuesterBehaviour[];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = knight.position - this.transform.position;

        foreach (QuesterBehaviour quester in questers)
        {
            if (Vector3.Distance(knight.position, quester.gameObject.transform.position) <= 1.50f)
            {
                questerActive = true;
                break;
            }
            else
            {
                questerActive = false;
            }
        }

        if (!questerActive)
        {
            animator.SetBool("IsInteracting", false);                        
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
            
            textPanel.SetActive(true);
            
            dialogueData.name = name;
            dialogueData.sentences.Clear();

            if (KnightBehaviour.quests.Count == 0)
            {
                KnightBehaviour.quests.Add(questNames[0], false);
            }

            int questIndex = KnightBehaviour.quests.Count - 1;

            if (!KnightBehaviour.quests[questNames[questIndex]])
            {
                dialogueData.sentences.Add(dialogue[questIndex]);
            }
            else
            {
                dialogueData.sentences.Add(dialogue[questIndex + 1]);
                KnightBehaviour.quests.Add(questNames[questIndex + 1], false);
            }
            
            dialogueManager.StartDialogue(dialogueData);
        }
    }
}