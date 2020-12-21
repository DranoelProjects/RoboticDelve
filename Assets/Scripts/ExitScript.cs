using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    InventoryManager inventoryManager;
    [SerializeField] AudioClip sndWinLevel;
    [SerializeField] bool isTutorialScene = false;
    AudioSource audioSource;
    UIScript uiScript;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        uiScript = GameObject.Find("MainCanvas").GetComponent<UIScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check if player found the key
        //only the scientist can win the lvl
        if (collision.CompareTag("Player") && inventoryManager.KeyAmount > 0)
        {
            //remove the key from inventory
            inventoryManager.KeyAmount = 0;
            //Play scientist win lvl animation
            collision.GetComponentInChildren<Animator>().SetTrigger("WinLevel");
            //Disable moving script
            collision.GetComponent<PlayerScript>().CanMoove = false;
            //Show win level panel
            uiScript.WinLevel();
            //Play winning level sound
            audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(sndWinLevel);
            //Loading next level after 2 seconds
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        //Check if the current scene is tutorial or already the real Donjon
        if (isTutorialScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
