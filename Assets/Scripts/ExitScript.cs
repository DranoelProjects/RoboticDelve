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
        if (collision.CompareTag("Player") && inventoryManager.KeyAmount > 0)
        {
            inventoryManager.KeyAmount = 0;
            collision.GetComponentInChildren<Animator>().SetTrigger("WinLevel");
            collision.GetComponent<PlayerScript>().CanMoove = false;
            uiScript.WinLevel();
            audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(sndWinLevel);
            int currentLevel = PlayerPrefs.GetInt("Level");
            PlayerPrefs.SetInt("Level", currentLevel + 1);
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        if (isTutorialScene)
        {
            PlayerPrefs.SetInt("Level", 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
