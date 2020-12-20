using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialUI : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip sndBtnClicked;
    [SerializeField] GameObject player;
    PlayerScript playerScript;

    void Start()
    {
        Time.timeScale = 0;
        playerScript = player.GetComponent<PlayerScript>();
        playerScript.IsAbleToAttack = false;
    }

    public void OnClickOK()
    {
        StartCoroutine(AttackBool());
        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;
        audioSource.PlayOneShot(sndBtnClicked);
    }

    IEnumerator AttackBool()
    {
        yield return new WaitForSeconds(0.3f);
        playerScript.IsAbleToAttack = true;
    }

    public void EndTutoriel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
