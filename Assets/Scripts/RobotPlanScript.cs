using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotPlanScript : MonoBehaviour
{
    [SerializeField] Text textOutputDescription;
    [SerializeField] RobotPlanData[] robotsArray;
    public RobotPlanData CurrentRobot;
    int index;

    void Start()
    {
        index = Random.Range(0, robotsArray.Length);
        CurrentRobot = robotsArray[index];
        textOutputDescription.text = CurrentRobot.Description;
    }
}
