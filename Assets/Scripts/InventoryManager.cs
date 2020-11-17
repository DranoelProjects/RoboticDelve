using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    int ironIngotNumber, robotPlanNumber, munitionsNumber;

    void Awake()
    {
        ironIngotNumber = PlayerPrefs.GetInt("IronIngot");
        robotPlanNumber = PlayerPrefs.GetInt("RobotPlan");
        munitionsNumber = PlayerPrefs.GetInt("Munitions");
    }

    public void UpdateItemNumber(string itemName, int itemValue)
    {
        switch (itemName)
        {
            case "IronIngot":
                ironIngotNumber += itemValue;
                Debug.Log("ironIngotNumber: " + ironIngotNumber);
                break;
            case "RobotPlan":
                robotPlanNumber += itemValue;
                Debug.Log("robotPlanNumber: " + robotPlanNumber);
                break;
            case "Munitions":
                munitionsNumber += itemValue;
                Debug.Log("munitionsNumber: " + munitionsNumber);
                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("IronIngot", ironIngotNumber);
        PlayerPrefs.SetInt("RobotPlan", robotPlanNumber);
        PlayerPrefs.SetInt("Munitions", munitionsNumber);
    }
}
