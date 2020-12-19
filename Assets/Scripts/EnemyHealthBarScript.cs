using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthBarScript : MonoBehaviour
{
    float EnemyhealthPoints;
    public Image fillBar;
    EnemyAI enemy;
    public Image FillBarParent;
    public Text Txt;
    public bool IsBoss = false;

    //Animations
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        // enemy = transform.parent.GetComponent<EnemyAI>();
        enemy = this.GetComponentInParent<EnemyAI>();
    }

    public void UpdateHealthPoints()
    {
        EnemyhealthPoints = enemy.healthpoints/enemy.healthpointsMax;
        if (IsBoss && EnemyhealthPoints>=0)
        {
            Txt.text = (EnemyhealthPoints * 100) + " %";
        }
        fillBar.fillAmount = EnemyhealthPoints;
    }

}
