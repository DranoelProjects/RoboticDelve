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
    
    //Animations
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GameObject.Find("Enemy").GetComponent<EnemyAI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        EnemyhealthPoints = enemy.healthpoints/enemy.healthpointsMax;
        // healthPoints = 0.6f;
        fillBar.fillAmount = EnemyhealthPoints;
    }

}
