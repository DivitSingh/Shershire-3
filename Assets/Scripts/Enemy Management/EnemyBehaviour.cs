using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    #region Animations and AI
    public Animator animator;
    NavMeshAgent agent;
    Transform knight;

    public GameObject specialObject;
    Vector3 originalPos;

    public List<Transform> waypoints;
    public Enemy enemy;    

    public int currentHP;
    bool canAttack = true;    
    float hitboxElapsedTime;
    float animLength;
    
    int destPoint = 0;
    float cooldownMod = 0.00f;
    bool phase1Counter = false;
    bool phase2Counter = false;
    #endregion

    #region Sounds    
    [System.NonSerialized] 
    public AudioClip Hit;
    [System.NonSerialized]
    public AudioClip Block;
    [System.NonSerialized]
    public AudioClip Hurt;
    [System.NonSerialized]
    public AudioClip Death;
    #endregion

    #region UI
    public Slider healthBar;
    public Text healthText;
    public Text nameText;
    #endregion

    // Use this for initialization
    void Start()
    {       
        animator = GetComponent<Animator>();        
        agent = GetComponent<NavMeshAgent>();
        knight = GameObject.Find("Knight").transform;

        currentHP = enemy.HP;
        agent.speed = (float) enemy.Speed * 0.10f;
        
        Block = enemy.Block;
        Hurt = enemy.Hurt;
        Death = enemy.Death;        

        nameText.text = enemy.Name;
        cooldownMod = 1.00f - ((float)enemy.Recovery / 100.00f);

        if (enemy.name == "Warrock")
        {            
            specialObject.SetActive(false);
            originalPos = specialObject.transform.localPosition;
        }
    }

    public void HitCalculation(GameObject enemyObject)
    {
        float damageMod = (100.00f + Mathf.Clamp(KnightBehaviour.Strength - enemy.Resistance, -100.00f, 1000.00f)) / 100.00f;
        currentHP -= Mathf.RoundToInt(Mathf.Clamp(damageMod * KnightBehaviour.attackMod * KnightBehaviour.Damage, 1.00f, 1000.00f));

        if (currentHP > 0)
        {
            GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Hurt);            
        }
        else if (!this.enemy.name.Contains("Projectile"))
        {
            animator.Play("Death");
            GameObject.Find("Audio Manager").GetComponent<AudioManager>().audioManager.PlayOneShot(Death);
            StartCoroutine("DeathDelay");
        }        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.transform.LookAt(GameObject.Find("vThirdPersonCamera").transform);
        float size = (Camera.main.transform.position - healthBar.transform.position).magnitude * 0.0025f;
        healthBar.transform.localScale = new Vector3(size, size, size);

        healthBar.maxValue = enemy.HP;
        healthBar.value = currentHP;

        if (currentHP < 0)
            currentHP = 0;

        healthText.text = currentHP.ToString("n0") + "/" + enemy.HP.ToString("n0");

        if (currentHP > 0)
        {
            if (enemy.name.Contains("Maw") || enemy.name == "Warrock")
            {
                if (currentHP < (0.25 * enemy.HP))
                {
                    if (enemy.name.Contains("Maw"))
                    {
                        enemy.Resistance = 50;
                        enemy.Strength = 75;
                        nameText.text = "Maw (Enraged)";
                    }
                    else if (enemy.name == "Warrock" && !phase2Counter)
                    {
                        knight.position = new Vector3(285.00f, 21.17f, 180.00f);
                        knight.rotation = new Quaternion(0, 1, 0, 0);                        
                        phase2Counter = true;
                    }

                    if (phase2Counter && knight.position.y > 1.00f)
                    {
                        bool enemyCounter = true;

                        foreach (Transform child in GameObject.Find("Goblin Party").transform)
                        {
                            if (child.gameObject.activeSelf)
                            {
                                enemyCounter = false;
                                break;
                            }
                        }

                        if (enemyCounter)
                        {
                            knight.position = new Vector3(285.00f, 0.00f, 200.00f);
                            enemyCounter = false;
                        }
                    }
                }
                else if (currentHP < (0.75 * enemy.HP))
                {
                    if (enemy.name.Contains("Maw"))
                    {
                        enemy.Resistance = 75;
                        enemy.Strength = 50;
                        nameText.text = "Maw (Fortified)";
                    }
                    else if (enemy.name == "Warrock" && !phase1Counter)
                    {
                        knight.position = new Vector3(259.25f, 22.87f, 196.68f);
                        knight.rotation = new Quaternion(0, 1, 0, 0);
                        phase1Counter = true;
                    }

                    if (phase1Counter && knight.position.y > 1.00f)
                    {
                        bool enemyCounter = true;

                        foreach (Transform child in GameObject.Find("Dwarf Party").transform)
                        {
                            if (child.gameObject.activeSelf)
                            {
                                enemyCounter = false;
                                break;
                            }
                        }

                        if (enemyCounter)
                        {
                            knight.position = new Vector3(285.00f, 0.00f, 200.00f);
                            enemyCounter = false;
                        }                        
                    }
                }
                else if (currentHP > (0.75 * enemy.HP))
                {
                    if (enemy.name.Contains("Maw"))
                    {
                        enemy.Resistance = 50;
                        enemy.Strength = 50;
                        nameText.text = "Maw";
                    }                    
                }                
            }

            Vector3 direction = knight.position - this.transform.position;            

            if (((Vector3.Distance(knight.position, this.transform.position) < 5.00f &&
                Vector3.Distance(knight.position, this.transform.position) > 1.50f) || this.enemy.name.Contains("Projectile")) && agent.enabled)
            {
                if (!this.enemy.name.Contains("Projectile"))
                {
                    healthBar.gameObject.SetActive(true);
                    animator.SetBool("IsWalking", true);
                    animator.SetBool("IsIdle", false);
                    animator.SetBool("IsAttacking", false);

                    if (enemy.name == "Warrock")
                        animator.SetBool("IsSpecial", false);
                }                 

                agent.destination = knight.transform.position;
                agent.isStopped = false;
            }
            else if (Vector3.Distance(knight.position, this.transform.position) <= 1.50f && agent.enabled)
            {
                direction.y = 0;
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                    Quaternion.LookRotation(direction), 0.10f);
                agent.isStopped = true;
                
                if (canAttack)
                {
                    int attackNumber = Random.Range(0, 3);

                    if (enemy.name == "Warrock" && !specialObject.activeSelf && attackNumber == 2)
                    {                        
                        StartCoroutine(Special());
                        animator.SetBool("IsSpecial", true);
                        animator.SetBool("IsWalking", false);
                        animator.SetBool("IsIdle", false);
                    }
                    else
                    {
                        StartCoroutine(Attack());
                        animator.SetBool("IsAttacking", true);
                        animator.SetBool("IsWalking", false);
                        animator.SetBool("IsIdle", false);
                    }
                }
            }
            else if (Vector3.Distance(knight.position, this.transform.position) >= 5.00f && agent.enabled)
            {
                healthBar.gameObject.SetActive(false);

                if (waypoints.Capacity > 1 && !agent.pathPending && agent.remainingDistance < 1.50f)
                {                    
                    animator.SetBool("IsWalking", true);
                    animator.SetBool("IsIdle", false);
                    animator.SetBool("IsAttacking", false);

                    if (enemy.name == "Warrock")
                        animator.SetBool("IsSpecial", false);

                    agent.destination = waypoints[destPoint].position;
                    destPoint = (destPoint + 1) % waypoints.Count;
                }
                else if (waypoints.Capacity == 0)
                {                    
                    animator.SetBool("IsIdle", true);
                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsAttacking", false);

                    if (enemy.name == "Warrock")
                        animator.SetBool("IsSpecial", false);
                }                
            }

            if (this.gameObject.name.Contains("Projectile"))
                print(Vector3.Distance(knight.position, this.transform.position));

            if (this.gameObject.name.Contains("Projectile") && Vector3.Distance(knight.position, this.transform.position) <= 2.00f)
            {
                knight.gameObject.GetComponent<KnightBehaviour>().HitCalculation(this.gameObject);
                this.gameObject.SetActive(false);                
            }            
        }
    }

    private IEnumerator Special()
    {
        canAttack = false;
        agent.enabled = false;
        Vector3 currentPos = this.transform.localPosition;
        this.transform.position = waypoints[0].position;

        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (animator.runtimeAnimatorController.animationClips[i].name == "Special")
            {
                animLength = animator.runtimeAnimatorController.animationClips[i].length;
            }
        }

        yield return new WaitForSeconds(animLength / 2);
                
        specialObject.SetActive(true);        
        specialObject.transform.position = waypoints[0].position;

        yield return new WaitForSeconds(animLength / 2);

        animator.SetBool("IsIdle", true);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsSpecial", false);                        

        yield return new WaitForSeconds(10.00f);

        if (specialObject.activeSelf)
        {            
            specialObject.SetActive(false);
            specialObject.transform.localPosition = originalPos;
        }

        this.transform.localPosition = new Vector3(currentPos.x, currentPos.y, currentPos.z);
        agent.enabled = true;
        canAttack = true;
    }

    private IEnumerator Attack()
    {       
        canAttack = false;

        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (animator.runtimeAnimatorController.animationClips[i].name == "Attack")
            {
                animLength = animator.runtimeAnimatorController.animationClips[i].length;
            }
        }

        yield return new WaitForSeconds(animLength / 2);
                
        knight.gameObject.GetComponent<KnightBehaviour>().HitCalculation(this.gameObject);        

        yield return new WaitForSeconds(animLength / 2);
        
        animator.SetBool("IsIdle", true);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);

        yield return new WaitForSeconds(5.00f * cooldownMod);

        canAttack = true;
    }

    private IEnumerator DeathDelay()
    {   
        agent.enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        yield return new WaitForSeconds(2.50f);

        int objectIndex = Random.Range(0, enemy.itemDrops.Count);
        GameObject spawnedObject = Instantiate(enemy.itemDrops[objectIndex], 
            new Vector3(transform.position.x, 
            enemy.itemDrops[objectIndex].transform.position.y + transform.position.y, transform.position.z),
            enemy.itemDrops[objectIndex].transform.rotation, GameObject.Find("Items").transform);
        spawnedObject.name = enemy.itemDrops[objectIndex].name;        
        this.gameObject.SetActive(false);
    }
}