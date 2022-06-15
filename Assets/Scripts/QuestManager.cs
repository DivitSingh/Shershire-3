using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public string questName;
    public AudioClip Victory;
    public AudioClip Phase;   
    
    public GameObject mawParty;
    public GameObject maw;
    public GameObject warrock;

    private bool phase1Complete = false;
    private bool phase2Complete = false;

    private void Awake()
    {
        mawParty = GameObject.Find("Maw Party");
        maw = GameObject.Find("Maw");
        warrock = GameObject.Find("Warrock");
    }

    // Update is called once per frame
    void Update()
    {
        if (KnightBehaviour.quests.Count == 0)
        {
            GameObject.Find("Boundaries (Internal)").SetActive(true);

            if (KnightBehaviour.skill != null)
            {
                GameObject.Find("Boundaries (Internal)").SetActive(false);
                GameObject.Find("Grimoire (Tutorial)").GetComponent<GrimoireBehaviour>().enabled = false;
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Victory);
            }
        }
        if (questName == "The Royal Treasury" && KnightBehaviour.quests.ContainsKey("The Royal Treasury")
            && !KnightBehaviour.quests["The Royal Treasury"])
        {
            bool questComplete = true;

            foreach (Transform enemy in GameObject.Find("The Royal Treasury/Enemies").transform)
            {
                if (enemy.gameObject.activeSelf)
                {
                    questComplete = false;
                    break;
                }
            }

            if (questComplete)
            {
                KnightBehaviour.quests["The Royal Treasury"] = true;
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Victory);
            }
        }
        else if (questName == "Return of the Maw" && KnightBehaviour.quests.ContainsKey("Return of the Maw")
            && !KnightBehaviour.quests["Return of the Maw"])
        {
            bool minionsDefeated = true;

            foreach (Transform enemy in GameObject.Find("Shershire Fields/Enemies").transform)
            {                
                if (enemy.gameObject.activeSelf && enemy.gameObject != mawParty)
                {
                    minionsDefeated = false;

                    foreach (Transform mawPartyMember in mawParty.transform)
                    {
                        mawPartyMember.gameObject.SetActive(false);
                    }

                    break;
                }
            }

            if (minionsDefeated)
            {                
                if (maw.GetComponent<EnemyBehaviour>().currentHP > 0)
                {
                    foreach (Transform mawPartyMember in mawParty.transform)
                    {
                        if (mawPartyMember.gameObject.GetComponent<EnemyBehaviour>().currentHP > 0)
                            mawPartyMember.gameObject.SetActive(true);                           
                    }

                    if (!phase1Complete && maw.GetComponent<EnemyBehaviour>().currentHP < (0.75 * maw.GetComponent<EnemyBehaviour>().enemy.HP))
                    {
                        GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Phase);
                        phase1Complete = true;
                    }

                    if (!phase2Complete && maw.GetComponent<EnemyBehaviour>().currentHP < (0.25 * maw.GetComponent<EnemyBehaviour>().enemy.HP))
                    {
                        GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Phase);
                        phase2Complete = true;
                    }
                }

                if (maw.GetComponent<EnemyBehaviour>().currentHP <= 0)
                {
                    KnightBehaviour.quests["Return of the Maw"] = true;
                    GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Victory);
                }
            }
        }
        else if (questName == "Plague of the Castle" && KnightBehaviour.quests.ContainsKey("Plague of the Castle")
        && !KnightBehaviour.quests["Plague of the Castle"])
        {
            if (warrock.GetComponent<EnemyBehaviour>().currentHP <= 0)
            {
                KnightBehaviour.quests["Plague of the Castle"] = true;
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Victory);
            }
        }        
    }
}
