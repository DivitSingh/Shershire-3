using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnightBehaviour : MonoBehaviour
{
    #region Animations
    public GameObject chest;
    public GameObject weapon;
    public GameObject shield;
    public GameObject r_wrist;
    public GameObject l_wrist;
            
    Vector3 shieldOriginChest;
    Vector3 shieldAttackWrist;

    private Animator animator;

    float animLength;
    float rightMouseHeldTime = 0.00f;
    float leftMouseHeldTime = 0.00f;
    bool rightClick = false;
    bool leftClick = false;    

    public static bool canEquip;
    public static bool canUnequip;
    public static bool canMove;
    public static float attackMod;
    float cooldownMod = 0.00f;
    #endregion

    #region Skills
    public Text skillTimer;
    public static bool canSkill;
    public GameObject skillPanel;
    private GameObject fortification;
    private GameObject instincts;
    private GameObject agility;
    #endregion

    #region Sounds    
    public AudioClip Collect;
    public AudioClip Purchase;
    public AudioClip Failure;
    public AudioClip Consume;
    public AudioClip Equip;
    public AudioClip Unequip;
    public AudioClip Hit;
    public AudioClip Skill;
    public AudioClip Block;
    public AudioClip Hurt;
    public AudioClip Death;
    #endregion

    #region Knight Stats
    public static int HP;
    public static int currentHP;
    public static int Damage;
    public static int Resistance;
    public static int Strength;   
    public static int Recovery;
    public static int Speed;

    public static int Sherlings;
    public static int Trinkets;
    #endregion

    #region Inventory Management
    public Inventory inventory;    
    public Dictionary<string, Item> currentEquipment = new Dictionary<string, Item>();
    public Dictionary<string, GameObject> currentSlots = new Dictionary<string, GameObject>();
    public List<Item> skills = new List<Item>();
    public static Item skill;
    #endregion

    public static Dictionary<string, bool> quests = new Dictionary<string, bool>();

    // Use this for initialization
    void Start()
    {        
        currentEquipment.Add("Weapon", GameObject.Find("#1 Weapon (Slot)").GetComponent<ItemBehaviour>().clickedItem);
        currentEquipment.Add("Shield", GameObject.Find("#1 Shield (Slot)").GetComponent<ItemBehaviour>().clickedItem);    
        currentEquipment.Add("Helm", GameObject.Find("#1 Helm (Slot)").GetComponent<ItemBehaviour>().clickedItem);
        currentEquipment.Add("Armor", GameObject.Find("#1 Armor (Slot)").GetComponent<ItemBehaviour>().clickedItem);
        currentEquipment.Add("Gauntlets", GameObject.Find("#1 Gauntlets (Slot)").GetComponent<ItemBehaviour>().clickedItem);
        currentEquipment.Add("Greaves", GameObject.Find("#1 Greaves (Slot)").GetComponent<ItemBehaviour>().clickedItem);

        currentSlots.Add("Weapon", GameObject.Find("#1 Weapon (Slot)"));
        currentSlots.Add("Shield", GameObject.Find("#1 Shield (Slot)"));
        currentSlots.Add("Helm", GameObject.Find("#1 Helm (Slot)"));
        currentSlots.Add("Armor", GameObject.Find("#1 Armor (Slot)"));
        currentSlots.Add("Gauntlets", GameObject.Find("#1 Gauntlets (Slot)"));
        currentSlots.Add("Greaves", GameObject.Find("#1 Greaves (Slot)"));

        HP = currentEquipment["Helm"].HP + currentEquipment["Armor"].HP + currentEquipment["Gauntlets"].HP + currentEquipment["Greaves"].HP 
            + currentEquipment["Weapon"].HP + currentEquipment["Shield"].HP;
        Damage = currentEquipment["Helm"].Damage + currentEquipment["Armor"].Damage + currentEquipment["Gauntlets"].Damage + currentEquipment["Greaves"].Damage
            + currentEquipment["Weapon"].Damage + currentEquipment["Shield"].Damage;
        currentHP = currentEquipment["Helm"].HP + currentEquipment["Armor"].HP + currentEquipment["Gauntlets"].HP + currentEquipment["Greaves"].HP
            + currentEquipment["Weapon"].HP + currentEquipment["Shield"].HP;
        Resistance = currentEquipment["Helm"].Resistance + currentEquipment["Armor"].Resistance + currentEquipment["Gauntlets"].Resistance + currentEquipment["Greaves"].Resistance
            + currentEquipment["Weapon"].Resistance + currentEquipment["Shield"].Resistance;
        Strength = currentEquipment["Helm"].Strength + currentEquipment["Armor"].Strength + currentEquipment["Gauntlets"].Strength + currentEquipment["Greaves"].Strength
            + currentEquipment["Weapon"].Strength + currentEquipment["Shield"].Strength;        
        Recovery = currentEquipment["Helm"].Recovery + currentEquipment["Armor"].Recovery + currentEquipment["Gauntlets"].Recovery + currentEquipment["Greaves"].Recovery
            + currentEquipment["Weapon"].Recovery + currentEquipment["Shield"].Recovery;
        Speed = currentEquipment["Helm"].Speed + currentEquipment["Armor"].Speed + currentEquipment["Gauntlets"].Speed + currentEquipment["Greaves"].Speed
            + currentEquipment["Weapon"].Speed + currentEquipment["Shield"].Speed;

        Sherlings = 0;
        Trinkets = 0;


        animator = GetComponent<Animator>();
        weapon = GameObject.Find(currentEquipment["Weapon"].name);
        shield = GameObject.Find(currentEquipment["Shield"].name);

        shieldOriginChest = new Vector3(0.0240242798f, 0.343337595f, -0.109592095f);
        shieldAttackWrist = new Vector3(0.0520000011f, 0.115000002f, -0.0879999995f);
        weapon.transform.SetParent(chest.transform); //chest becomes the parent of the maul object
        weapon.transform.localPosition = currentEquipment["Weapon"].originChest; //localPosition so that it is relative to chest
        weapon.transform.localRotation = Quaternion.Euler(new Vector3(-0.624f, 111.357f, 86.635f));//localRotation so that it is relative to chest             
        shield.transform.SetParent(chest.transform);
        shield.transform.localPosition = shieldOriginChest;
        shield.transform.localRotation = Quaternion.Euler(new Vector3(-90.00f, 0.00f, 0.00f));
        
        canEquip = true;
        canUnequip = false;
        canMove = true;
        canSkill = true;

        skillPanel.SetActive(false);
        fortification = GameObject.Find("Fortification");
        instincts = GameObject.Find("Instincts");
        agility = GameObject.Find("Agility");

        fortification.SetActive(false);
        instincts.SetActive(false);
        agility.SetActive(false);

        //quests.Add("The Royal Treasury", true);
        //quests.Add("Return of the Maw", true);
        //quests.Add("Plague of the Castle", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ItemBehaviour>())
        {
            if (other.gameObject.name.Contains("Sherling"))
            {
                Sherlings++;
            }
            else if (other.gameObject.name.Contains("Trinket"))
            {
                Trinkets++;
            }
            else
            {
                inventory.AddItem(other.gameObject.GetComponent<ItemBehaviour>().clickedItem, 1);
            }

            GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Collect);
            Destroy(other.gameObject);
        }
    }

    public void HitCalculation(GameObject enemyObject)
    {                
        float angle = Vector3.Angle(enemyObject.transform.position - this.transform.position, this.transform.forward);

        if (angle >= 0.00f && angle <= 50.00f && animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
        {
            GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Block);
        }
        else
        {
            float damageMod = (100.00f + Mathf.Clamp(enemyObject.GetComponentInParent<EnemyBehaviour>().enemy.Strength - Resistance, -100.00f, 1000.00f)) / 100.00f;
            currentHP -= Mathf.RoundToInt(Mathf.Clamp(damageMod * enemyObject.GetComponentInParent<EnemyBehaviour>().enemy.Damage, 1.00f, 1000.00f));

            if (currentHP > 0)
            {
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(enemyObject.GetComponentInParent<EnemyBehaviour>().Hit);
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Hurt);
            }
            else
            {
                animator.Play("Death");
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Death);
                StartCoroutine("DeathDelay");
            }
        }        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<vThirdPersonMotor>().moveSpeed = (float) Speed / 10.00f;
        cooldownMod = 1.00f - ((float)Recovery / 100.00f);

        if (Input.GetMouseButtonDown(1) && canMove && canUnequip && !canEquip)
        {            
            rightMouseHeldTime = Time.time;
            rightClick = true;            
        }

        if (canMove && rightClick && (Time.time - rightMouseHeldTime) > 0.50f)
        {            
            animator.Play("Block");            
            StartCoroutine(Delay("Block"));
        }

        if (Input.GetMouseButtonUp(1))
        {            
            if ((Time.time - rightMouseHeldTime) < 0.50f &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Light Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy Attack"))
            {                
                animator.Play("Unequip");
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Unequip);

                weapon.transform.SetParent(chest.transform); //chest becomes the parent of the maul object
                weapon.transform.localPosition = currentEquipment["Weapon"].originChest; //localPosition so that it is relative to chest
                weapon.transform.localRotation = Quaternion.Euler(new Vector3(-0.624f, 111.357f, 86.635f));//localRotation so that it is relative to chest                
                shield.transform.SetParent(chest.transform);
                shield.transform.localPosition = shieldOriginChest;
                shield.transform.localRotation = Quaternion.Euler(new Vector3(-90.00f, 0.00f, 0.00f));

                StartCoroutine(EquipDelay("Unequip"));
            }
            else if (canEquip && !canUnequip) //Making sure position is set properly
            {               
                animator.Play("Equip");
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Equip);

                weapon.transform.SetParent(r_wrist.transform); //wrist becomes parent
                weapon.transform.localPosition = currentEquipment["Weapon"].originWrist; //localPosition so that it is relative to wrist
                weapon.transform.localRotation = Quaternion.Euler(new Vector3(61.5f, 139.08f, 2.6f));//localRotation so that it is relative to wrist
                shield.transform.SetParent(l_wrist.transform);
                shield.transform.localPosition = shieldAttackWrist;
                shield.transform.localRotation = Quaternion.Euler(new Vector3(289.652893f, 69.3560486f, 0.24100709f));

                StartCoroutine(EquipDelay("Equip"));
            }            
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit rayHit, 100.0f))
            {
                if (rayHit.collider.tag == "EnemyHurtbox")
                {
                    Debug.Log("Enemy seen:" + rayHit.collider.gameObject.name);
                    transform.LookAt(rayHit.collider.gameObject.transform);
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && canMove && canUnequip && !canEquip)
        {            
            leftMouseHeldTime = Time.time;
            leftClick = true;
        }
        
        if (canMove && leftClick && (Time.time - leftMouseHeldTime) > 0.50f 
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Light Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
        {           
            animator.Play("Heavy Attack");
            attackMod = 2.50f;
            StartCoroutine(Delay("Heavy Attack"));
        }    
         
        if (Input.GetMouseButtonUp(0) && canMove
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Light Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
        {                        
            if ((Time.time - leftMouseHeldTime) < 0.50f)
            {               
                animator.Play("Light Attack");
                attackMod = 1.00f;
                StartCoroutine(Delay("Light Attack"));
            }
        }

        if (Input.GetMouseButtonUp(2) && canUnequip && !canEquip
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Light Attack")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Block"))
        {            
            if (skill != null && canSkill)
            {
                GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Skill);

                if (skill.name != "Bash" && skill.name != "Burst")
                {
                    StartCoroutine("SkillBuff");
                }
                else if (canUnequip && !canEquip)
                {
                    canSkill = false;
                    skillPanel.SetActive(true);

                    if (skill.name == "Bash")
                    {                        
                        animator.Play("Bash");
                        attackMod = 4.00f;
                        StartCoroutine(Delay("Bash"));
                    }
                    else if (skill.name == "Burst")
                    {
                        animator.Play("Burst");
                        attackMod = 2.00f;
                        StartCoroutine(Delay("Burst"));
                    }
                }
            }
        }
    }

    private IEnumerator EquipDelay(string clipName)
    {
        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (animator.runtimeAnimatorController.animationClips[i].name == clipName)
            {
                animLength = animator.runtimeAnimatorController.animationClips[i].length;
            }
        }

        this.gameObject.GetComponent<vThirdPersonInput>().enabled = false;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;        

        rightMouseHeldTime = 0.00f;
        leftMouseHeldTime = 0.00f;
        leftClick = false;
        rightClick = false;

        if (clipName == "Equip")
        {
            canEquip = false;
        }
        else
        {
            canUnequip = false;
        }

        yield return new WaitForSeconds(animLength);

        this.gameObject.GetComponent<vThirdPersonInput>().enabled = true;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;

        if (clipName == "Equip")
        {
            canUnequip = true;
        }
        else
        {
            canEquip = true;
        }       
    }

    private IEnumerator Delay(string clipName)
    {        
        this.gameObject.GetComponent<vThirdPersonInput>().enabled = false;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        canMove = false;

        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (animator.runtimeAnimatorController.animationClips[i].name == clipName)
            {
                animLength = animator.runtimeAnimatorController.animationClips[i].length * 0.75f;
            }
        }               

        yield return new WaitForSeconds(animLength / 2);

        if (clipName != "Block")
        {
            float radius = 3.00f;

            if (clipName == "Burst")
                radius = 7.50f;

            foreach (Collider collider in Physics.OverlapSphere(this.transform.position, radius))
            {
                float angle = Vector3.Angle(collider.transform.position - this.transform.position, this.transform.forward);

                if (clipName != "Burst" && angle >= 0.00f && angle <= 50.00f && collider.gameObject.tag == "EnemyHurtbox")                             
                {
                    collider.gameObject.GetComponent<EnemyBehaviour>().HitCalculation(collider.gameObject);
                }
                else if (clipName == "Burst" && collider.gameObject.tag == "EnemyHurtbox")
                {
                    collider.gameObject.GetComponent<EnemyBehaviour>().HitCalculation(collider.gameObject);
                }
            }
        }

        rightMouseHeldTime = 0.00f;
        leftMouseHeldTime = 0.00f;        
        leftClick = false;
        rightClick = false;

        yield return new WaitForSeconds(animLength / 2);
        
        if (clipName != "Bash" && clipName != "Burst")
        {
            if (clipName == "Heavy Attack")
            {
                yield return new WaitForSeconds(2.50f * cooldownMod);
            }
            else
            {
                yield return new WaitForSeconds(1.00f * cooldownMod);
            }
        }
        
        this.gameObject.GetComponent<vThirdPersonInput>().enabled = true;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        canMove = true;

        if (clipName == "Bash" || clipName == "Burst")
        {
            yield return new WaitForSeconds(15.00f * cooldownMod);
            canSkill = true;
            skillPanel.SetActive(false);
        }
    }

    private IEnumerator SkillBuff()
    {        
        var color = skillTimer.transform.parent.GetComponent<Image>().color;
        color.a = 0.50f;
        skillTimer.transform.parent.GetComponent<Image>().color = color;
        skillPanel.SetActive(true);
        canSkill = false;        

        if (skill.name == "Fortification")
        {
            fortification.SetActive(true);
            Resistance += 25;

            yield return new WaitForSeconds(10.00f);

            fortification.SetActive(false);            
            Resistance -= 25;            
        }
        else if (skill.name == "Instincts")
        {
            instincts.SetActive(true);
            Recovery += 50;

            yield return new WaitForSeconds(10.00f);

            instincts.SetActive(false);            
            Recovery -= 50;            
        }
        else if (skill.name == "Agility")
        {
            agility.SetActive(true);
            Speed += 50;

            yield return new WaitForSeconds(10.00f);

            agility.SetActive(false);            
            Speed -= 50;            
        }

        cooldownMod = 1.00f - ((float)Recovery / 100.00f);
        
        int time = Mathf.RoundToInt(15 * cooldownMod);

        while (time > 0)
        {
            skillTimer.text = time.ToString();
            yield return new WaitForSeconds(1.00f);            
            time--;
        }

        skillTimer.text = "";
        color.a = 1.00f;
        skillTimer.transform.parent.GetComponent<Image>().color = color;
        skillPanel.SetActive(false);
        canSkill = true;
    }

    private IEnumerator DeathDelay()
    {
        UnityEditor.EditorApplication.isPlaying = false;

        yield return new WaitForSeconds(2.50f);
        gameObject.SetActive(false);
    }
}