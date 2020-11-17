using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] float speed = 7f;
    [SerializeField] bool isAbleToAttack = false;
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

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        uiScript = GameObject.Find("MainCanvas").GetComponent<UIScript>();
    }

    void Update()
    {
        //movements
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        transform.Translate(Vector2.right * moveX * speed * Time.deltaTime);
        transform.Translate(Vector2.up * moveY * speed * Time.deltaTime);
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
    }

    void Flip()
    {
        lookRight = !lookRight;
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
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
        }
    }
}

