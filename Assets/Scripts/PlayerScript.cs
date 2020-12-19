using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] float speed = 7f, damage = 1f;
    [SerializeField] bool isAbleToAttack = false;
    [SerializeField] public float healthpoints = 10;
    [SerializeField] public float healthpointsMax = 10;
    HealthBarScript healthBarScript;

    GameManagerScript gameManagerScript;
    public bool CanMoove = true;

    public Transform m_spriteTransform;
    bool lookRight = true, alreadyHurt = false;
    public bool OnAttack = false;

    //animations
    Animator animator;

    //Sounds
    [SerializeField] AudioClip sndAttack, sndItemPickup, sndHurt, sndDead;
    AudioSource audioSource;

    //Inventory
    InventoryManager inventoryManager;

    //UI
    UIScript uiScript;

    //Physic
    Rigidbody2D m_rigidBody2D;
    CircleCollider2D circleCollider2D;
    float colliderRadius;

    void Awake()
    {
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        healthBarScript = GetComponentInChildren<HealthBarScript>();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        uiScript = GameObject.Find("MainCanvas").GetComponent<UIScript>();
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiScript.PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uiScript.PanelInventory.SetActive(!uiScript.PanelInventory.activeInHierarchy);
            uiScript.UpdateInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyAI>().ShouldUpdatePlayerArray = true;
            }
            gameManagerScript.SwapBetweenRobotAndPlayer();
        }
    }

    void FixedUpdate()
    {
        if (CanMoove)
        {
            //movements
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            float actualSpeed = speed;
            if (moveX != 0 && moveY != 0)
                actualSpeed = speed / Mathf.Sqrt(2);
            m_rigidBody2D.MovePosition(new Vector2(m_rigidBody2D.position.x + moveX * actualSpeed * Time.deltaTime, m_rigidBody2D.position.y + moveY * actualSpeed * Time.deltaTime));
            animator.SetFloat("SpeedX", Mathf.Abs(moveX));
            animator.SetFloat("SpeedY", moveY);

            if (moveX > 0 && !lookRight)
            {
                Flip();
            }
            else if (moveX < 0 && lookRight)
            {
                Flip();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && !OnAttack && isAbleToAttack)
            {
                OnAttack = true;
                colliderRadius = circleCollider2D.radius;
                circleCollider2D.radius = 0.7f;
                animator.SetTrigger("Attack");
                audioSource.PlayOneShot(sndAttack);
                StartCoroutine(AttackBool());
            }
        }
    }


    void Flip()
    {
        lookRight = !lookRight;
        Vector2 theScale =  m_spriteTransform.localScale;
        theScale.x *= -1;
        m_spriteTransform.localScale = theScale;
    }

    IEnumerator AttackBool()
    {
        yield return new WaitForSeconds(0.5f);
        OnAttack = false;
        circleCollider2D.radius = colliderRadius;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Item":
                ItemScript itemScript = collision.gameObject.GetComponent<ItemScript>();
                string itemName = itemScript.ItemName;
                int itemValue = itemScript.ItemValue;
                inventoryManager.UpdateItemNumber(itemName, itemValue);
                Destroy(collision.gameObject);
                audioSource.PlayOneShot(sndItemPickup);

                if (uiScript.PanelInventory.activeInHierarchy)
                {
                    uiScript.UpdateInventoryUI();
                }
                if (itemName == "RobotPlan")
                {
                    RobotPlanScript robotPlanScript = collision.gameObject.GetComponent<RobotPlanScript>();
                    inventoryManager.AddNewPlan(robotPlanScript.CurrentRobot);
                }
                break;
            case "Enemy":
                EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
                if (OnAttack && enemyAI.healthpoints >= 0)
                {
                    enemyAI.healthpoints -= damage;
                    collision.gameObject.GetComponentInChildren<EnemyHealthBarScript>().UpdateHealthPoints();
                    enemyAI.Hurt();
                }
                if (enemyAI.OnAttack)
                {
                    if (!alreadyHurt)
                    {
                        alreadyHurt = true;
                        audioSource.PlayOneShot(sndHurt);
                        Hurt(enemyAI.Damage, collision);
                        StartCoroutine(HurtBool(enemyAI.AttackCouldown));
                    }
                }
                break;
            default:
                break;
        }
    }

    void DeathPlayer()
    {
        Destroy(gameObject.GetComponent<CircleCollider2D>());
        audioSource.PlayOneShot(sndDead);
        animator.SetTrigger("Fall");
        CanMoove = false;
        GetComponent<PlayerScript>().enabled = false;
        StartCoroutine(DefeatPanel());
    }

    IEnumerator DefeatPanel()
    {
        yield return new WaitForSeconds(1f);
        uiScript.ShowDefeatPanel();
        StartCoroutine(RestartCurrentScene());
    }

    IEnumerator RestartCurrentScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Hurt(float damage, Collision2D col)
    {
        //animator.SetTrigger("Hurt");
        Vector2 move = col.transform.position - transform.position;
        m_rigidBody2D.AddForce(move.normalized * -200);
        //audioSource.PlayOneShot(sndHurt);
        healthpoints -= damage;
        healthBarScript.UpdateHealthPoints();
        if (healthpoints <= 0) DeathPlayer();
    }

    IEnumerator HurtBool(float enemyAttackCd)
    {
        yield return new WaitForSeconds(enemyAttackCd);
        alreadyHurt = false;
    }
}

