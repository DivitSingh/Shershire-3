using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatLog : MonoBehaviour {    
    public Slider HealthBar;     
    public Text healthText;
    public Text damageText;
    public Text statsText;
    public Text sherlingsText;
    public Text trinketsText;

    // Update is called once per frame
    void Update () {        
        HealthBar.maxValue = KnightBehaviour.HP;
        HealthBar.value = KnightBehaviour.currentHP;
        healthText.text = KnightBehaviour.currentHP.ToString("n0") + "/" + KnightBehaviour.HP.ToString("n0");
        damageText.text = "Damage: " + KnightBehaviour.Damage;

        statsText.text = "Strength: " + KnightBehaviour.Strength +
            "\nResistance: " + KnightBehaviour.Resistance +
            "\nRecovery: " + KnightBehaviour.Recovery + "%" +
            "\nSpeed: " + KnightBehaviour.Speed + "%";

        sherlingsText.text = KnightBehaviour.Sherlings.ToString();
        trinketsText.text = KnightBehaviour.Trinkets.ToString();
	}
}
