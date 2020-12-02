using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RobotPlanData", menuName = "Items/RobotPlanData")]
public class RobotPlanData : ScriptableObject
{
    [Header("Common data")]
    public string Description;
    public string RobotKind;
    public string RobotName;
    public string Scarcity;
    public GameObject RobotToMake;

    [Header("Items needed to manufacture")]
    public string ItemName1;
    public int ItemNumber1;
    public string ItemName2;
    public int ItemNumber2;
    public string ItemName3;
    public int ItemNumber3;
}
