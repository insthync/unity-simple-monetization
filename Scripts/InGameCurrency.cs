using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InGameCurrency
{
    public string id;
    public int amount;
}

[System.Serializable]
public class InGameCurrencySetting
{
    public string id;
    public int startAmount;
    public string name;
    public string symbol;
}