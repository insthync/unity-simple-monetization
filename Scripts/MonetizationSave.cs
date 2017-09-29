using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonetizationSave
{
    public const string KeyHardCurrencyName = "SaveHardCurrency";
    public const string KeyPurchasedItemsName = "SavePurchasedItems";

    public static int GetHardCurrency()
    {
        return PlayerPrefs.GetInt(KeyHardCurrencyName, 0);
    }

    public static void SetHardCurrency(int amount)
    {
        PlayerPrefs.SetInt(KeyHardCurrencyName, amount);
        PlayerPrefs.Save();
    }

    public static bool AddHardCurrency(int amount)
    {
        var newAmount = GetHardCurrency() + amount;
        if (newAmount < 0)
            return false;
        SetHardCurrency(newAmount);
        return true;
    }

    public static List<string> GetPurchasedItems()
    {
        var result = JsonUtility.FromJson<List<string>>(PlayerPrefs.GetString(KeyPurchasedItemsName, "{}"));
        if (result == null)
            result = new List<string>();
        return result;
    }

    public static void SetPurchasedItems(List<string> purchasedItems)
    {
        PlayerPrefs.SetString(KeyPurchasedItemsName, JsonUtility.ToJson(purchasedItems));
        PlayerPrefs.Save();
    }

    public static void AddPurchasedItem(string itemName)
    {
        var list = GetPurchasedItems();
        if (!list.Contains(itemName))
            list.Add(itemName);
        SetPurchasedItems(list);
    }
}
