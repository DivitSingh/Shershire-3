using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text npcName;
    public Text dialogue;
    public Button nextSentence;
    public bool sentencesComplete; 

    public Queue<string> sentences;

    public GameObject instructions;

	// Use this for initialization
	void Start () {
        sentences = new Queue<string>();
        instructions.SetActive(false);
	}

    public void StartDialogue(Dialogue dialogue)
    {
        npcName.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        if (sentences.Count > 1)
        {
            nextSentence.image.color = Color.white;
            nextSentence.interactable = true;
        }
        else
        {
            nextSentence.image.color = Color.grey;
            nextSentence.interactable = false;
        }

        DisplayNextSentence();

    }

    public void DisplayNextSentence ()
    {
        if (sentences.Count == 1)
        {
            nextSentence.image.color = Color.grey;
            nextSentence.interactable = false;
            sentencesComplete = true;
        }
        else if (sentences.Count > 1)
        {
            nextSentence.image.color = Color.white;
            nextSentence.interactable = true;
            sentencesComplete = false;
        }

        if (sentences.Count == 0)
        {
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    public void DisplayInstructions()
    {
        instructions.SetActive(!instructions.activeSelf);
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogue.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogue.text += letter;
            yield return null;
        }
    }
}
