using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyTrigger : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Text task;
    [SerializeField] string newTask;
    bool alreadyDisplayed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !alreadyDisplayed)
        {
            panel.SetActive(true);
            alreadyDisplayed = true;
            Time.timeScale = 0;
            task.text = newTask;
        }
    }
}
