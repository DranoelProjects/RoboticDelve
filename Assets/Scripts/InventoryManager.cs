using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int IronIngotNumber, MunitionsNumber, GoldNuggetAmount, KeyAmount, CopperIngotAmount, ChlorophyteIngotAmount, LeadIngotAmount, CobaltIngotAmount, TitaniumIngotAmount;

    void Awake()
    {
        IronIngotNumber = PlayerPrefs.GetInt("IronIngot");
        MunitionsNumber = PlayerPrefs.GetInt("Munitions");
        GoldNuggetAmount = PlayerPrefs.GetInt("GoldNugget");
        KeyAmount = PlayerPrefs.GetInt("Key");
        CopperIngotAmount = PlayerPrefs.GetInt("CopperIngot");
        ChlorophyteIngotAmount = PlayerPrefs.GetInt("ChlorophyteIngot");
        LeadIngotAmount = PlayerPrefs.GetInt("LeadIngot");
        CobaltIngotAmount = PlayerPrefs.GetInt("CobaltIngot");
        TitaniumIngotAmount = PlayerPrefs.GetInt("TitaniumIngot");
    }

    public void UpdateItemNumber(string itemName, int itemValue)
    {
        switch (itemName)
        {
            case "IronIngot":
                IronIngotNumber += itemValue;
                break;
            case "Munitions":
                MunitionsNumber += itemValue;
                break;
            case "GoldNugget":
                GoldNuggetAmount += itemValue;
                break;
            case "Key":
                KeyAmount += itemValue;
                break;
            case "CopperIngot":
                CopperIngotAmount += itemValue;
                break;
            case "ChlorophyteIngot":
                ChlorophyteIngotAmount += itemValue;
                break;
            case "LeadIngot":
                LeadIngotAmount += itemValue;
                break;
            case "CobaltIngot":
                CobaltIngotAmount += itemValue;
                break;
            case "TitaniumIngot":
                TitaniumIngotAmount += itemValue;
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("IronIngot", IronIngotNumber);
        PlayerPrefs.SetInt("Munitions", MunitionsNumber);
        PlayerPrefs.SetInt("GoldNugget", GoldNuggetAmount);
        PlayerPrefs.SetInt("Key", KeyAmount);
        PlayerPrefs.SetInt("CopperIngot", CopperIngotAmount);
        PlayerPrefs.SetInt("ChlorophyteIngot", ChlorophyteIngotAmount);
        PlayerPrefs.SetInt("LeadIngot", LeadIngotAmount);
        PlayerPrefs.SetInt("CobaltIngot", CobaltIngotAmount);
        PlayerPrefs.SetInt("TitaniumIngot", TitaniumIngotAmount);
    }
}
