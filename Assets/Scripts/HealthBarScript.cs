using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBarScript : MonoBehaviour
{
    float healthPoints;
    public Image fillBar;
    PlayerScript player;
    EnemyAI enemy;
    
    void Awake()
    {
        player = this.GetComponentInParent<PlayerScript>();
        
    }

    public void UpdateHealthPoints()
    {
        healthPoints = player.healthpoints / player.healthpointsMax;
        fillBar.fillAmount = healthPoints;
    }
}