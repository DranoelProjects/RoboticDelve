using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBarScript : MonoBehaviour
{
    float healthPoints;
    public Image fillBar;
    PlayerScript player;
    [SerializeField] Text txt;
    EnemyAI enemy;
    
    void Awake()
    {
        player = this.GetComponentInParent<PlayerScript>();
        
    }

    public void UpdateHealthPoints()
    {
        healthPoints = player.healthpoints / player.healthpointsMax;
        txt.text = (healthPoints * 100) + " %";
        fillBar.fillAmount = healthPoints;
        if(healthPoints < 0.4f)
        {
            fillBar.color = Color.red;
        }
    }
}