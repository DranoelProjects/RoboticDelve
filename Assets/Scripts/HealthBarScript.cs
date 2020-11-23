using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBarScript : MonoBehaviour
{
    [SerializeField] float healthPoints;
    public Image fillBar;
    PlayerScript player;
    GameObject Scientist;
    string character;
    int i = 0;
    
    //Animations
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if(GameObject.Find("Scientist"))
        {
            player = GameObject.Find("Scientist/ScientistSprite").GetComponent<PlayerScript>();
            character = "Scientist";
        }
        else if(GameObject.Find("Robot"))
        {
            player = GameObject.Find("Robot/RobotSprite").GetComponent<PlayerScript>();
            character = "Robot";
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthPoints = player.healthpoints/player.healthpointsMax;
        // healthPoints = 0.6f;
        fillBar.fillAmount = healthPoints;
        ChangeCharacter();
    }

    void ChangeCharacter()
    {
        if (character == "Scientist" && GameObject.Find("Scientist") == false)
        {
            Awake();
        }
        else if(character == "Robot" && GameObject.Find("Robot") == false)
        {
            Awake();
        }

    }
}
