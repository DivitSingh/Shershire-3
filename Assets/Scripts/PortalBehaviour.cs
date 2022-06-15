using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
    public GameObject spotLight;
    public Vector3 warpLocation;
    public Quaternion warpRotation;
    public string questName;
    public List<AudioClip> music;

    Transform knight;

    void Start()
    {
        knight = GameObject.Find("Knight").transform;
    }

    private void Update()
    {        
        if (KnightBehaviour.quests.ContainsKey(questName))
        {
            if (Input.GetMouseButtonUp(2))
            {
                if (Vector3.Distance(knight.position, this.transform.position) < 3.00f && !this.gameObject.name.Contains("Exit"))
                {
                    knight.gameObject.transform.localPosition = warpLocation;
                    knight.gameObject.transform.rotation = warpRotation;
                    GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.clip = music[int.Parse(gameObject.name[10].ToString()) - 1];
                    GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.Play();
                }
                else if (Vector3.Distance(knight.position, this.transform.position) < 3.00f && KnightBehaviour.quests[questName]
                    && this.gameObject.name.Contains("Exit"))
                {
                    knight.gameObject.transform.position = new Vector3(150, 0, 200);
                    KnightBehaviour.currentHP = KnightBehaviour.HP;
                    GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.clip = music[0];                    
                    GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.Play();
                }
            }

            if (!this.gameObject.name.Contains("Exit"))
            {
                GetComponentInChildren<Light>().enabled = true;

                if (KnightBehaviour.quests[questName])
                {
                    GetComponentInChildren<Light>().color = Color.green;
                }
                else
                {
                    GetComponentInChildren<Light>().color = Color.red;
                }
            }
            else
            {
                if (KnightBehaviour.quests[questName])
                {
                    foreach (Transform child in this.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
        else if (!this.gameObject.name.Contains("Exit"))
        {
            GetComponentInChildren<Light>().enabled = false;
        }
        else
        {            
            foreach (Transform child in this.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
