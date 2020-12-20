using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyTrigger : MonoBehaviour
{
    [SerializeField] bool needToShowPanel = true, isTrigger = true;
    [SerializeField] GameObject panel;
    [SerializeField] Text task;
    [SerializeField] string newTask;
    InventoryManager inventoryManager;
    BoxCollider2D boxCollider;
    bool alreadyDisplayed = false;

    private void Awake()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = isTrigger;
    }

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !alreadyDisplayed)
        {
            if (needToShowPanel)
            {
                collision.GetComponent<PlayerScript>().IsAbleToAttack = false;
                panel.SetActive(true);
                Time.timeScale = 0;
            }
            task.text = newTask;
            alreadyDisplayed = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && inventoryManager.KeyAmount > 0)
        {
            boxCollider.isTrigger = true;
            task.text = newTask;
        } else if (inventoryManager.KeyAmount <= 0)
        {
            task.text = "Vous devez vaincre le Slime Boss";
        }
    }
}
