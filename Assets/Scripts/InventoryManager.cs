using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int IronIngotNumber, RobotPlanNumber, MunitionsNumber;

    void Awake()
    {
        IronIngotNumber = PlayerPrefs.GetInt("IronIngot");
        RobotPlanNumber = PlayerPrefs.GetInt("RobotPlan");
        MunitionsNumber = PlayerPrefs.GetInt("Munitions");
    }

    public void UpdateItemNumber(string itemName, int itemValue)
    {
        switch (itemName)
        {
            case "IronIngot":
                IronIngotNumber += itemValue;
                break;
            case "RobotPlan":
                RobotPlanNumber += itemValue;
                break;
            case "Munitions":
                MunitionsNumber += itemValue;
                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("IronIngot", IronIngotNumber);
        PlayerPrefs.SetInt("RobotPlan", RobotPlanNumber);
        PlayerPrefs.SetInt("Munitions", MunitionsNumber);
    }
}
