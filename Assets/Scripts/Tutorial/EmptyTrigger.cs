using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyTrigger : MonoBehaviour
{
    [SerializeField] bool needToShowPanel = true;
    [SerializeField] GameObject panel;
    [SerializeField] Text task;
    [SerializeField] string newTask;
    bool alreadyDisplayed = false;

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
}
