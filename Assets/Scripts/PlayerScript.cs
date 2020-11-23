using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] float speed = 7f;
    [SerializeField] bool isAbleToAttack = false;
<<<<<<< HEAD
    [SerializeField] public float healthpoints = 8;
    [SerializeField] public float healthpointsMax = 10;
||||||| 3bb14a7
=======
    public Transform m_spriteTransform;
>>>>>>> Enemies
    bool lookRight = true;
    public bool OnAttack = false;

    //animations
    Animator animator;

    [SerializeField] AudioClip sndAttack, sndItemPickup;
    AudioSource audioSource;

    //Inventory
    InventoryManager inventoryManager;

    //UI
    UIScript uiScript;

    //RigidBody
    Rigidbody2D m_rigidBody2D;

    void Awake()
    {
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        uiScript = GameObject.Find("MainCanvas").GetComponent<UIScript>();
    }

    void FixedUpdate()
    {
        //movements
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float actualSpeed = speed;
        if (moveX != 0 && moveY != 0)
            actualSpeed = speed / Mathf.Sqrt(2);
        //m_rigidBody2D.MovePosition(new Vector2(m_rigidBody2D.position.x + moveX * actualSpeed * Time.deltaTime, m_rigidBody2D.position.y + moveY * actualSpeed * Time.deltaTime));
        transform.Translate(Vector2.right * moveX * actualSpeed * Time.deltaTime);
        transform.Translate(Vector2.up * moveY * actualSpeed * Time.deltaTime);
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
            animator.SetTrigger("Attack");
            audioSource.PlayOneShot(sndAttack);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uiScript.PanelInventory.SetActive(!uiScript.PanelInventory.activeInHierarchy);
            uiScript.UpdateInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiScript.PauseGame();
        }

        DeathPlayer();
    }

    void Flip()
    {
        lookRight = !lookRight;
        Vector2 theScale =  m_spriteTransform.localScale;
        theScale.x *= -1;
        m_spriteTransform.localScale = theScale;
    }

    public void AttackBool()
    {
        OnAttack = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
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
        }
    }

    //Animation of the player death
    void DeathPlayer()
    {
        if(healthpoints == 0)
            animator.SetTrigger("Fall");
    }
}

