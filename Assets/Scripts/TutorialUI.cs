﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void OnClickOK()
    {
        playerScript.IsAbleToAttack = false;
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
}
