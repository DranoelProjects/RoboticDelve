using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthBarScript : MonoBehaviour
{
    float EnemyhealthPoints;
    public Image fillBar;
    PlayerScript player;
    EnemyAI enemy;
    GameObject parent;
    
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
        // healthPoints = 0.6f;
        fillBar.fillAmount = EnemyhealthPoints;
    }

}
