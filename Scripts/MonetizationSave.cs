using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonetizationSave
{
    [System.Serializable]
    public class PurchasedItems
    {
        public List<string> itemNames = new List<string>();
        public void Add(string itemName)
        {
            if (itemNames != null)
                itemNames.Add(itemName);
        }

        public bool Remove(string itemName)
        {
            if (itemNames == null)
                return false;
            return itemNames.Remove(itemName);
        }

        public bool Contains(string itemName)
        {
            if (itemNames == null)
                return false;
            return itemNames.Contains(itemName);
        }
    }
    public const string Tag = "MonetizationSave";
    public const string KeyCurrencyName = "SaveCurrency";
    public const string KeyPurchasedItemsName = "SavePurchasedItems";

    public static string GetCurrencyKey(string name)
    {
        return KeyCurrencyName + "_" + name;
    }

    public static int GetCurrency(string name)
    {
        if (!MonetizationManager.Currencies.ContainsKey(name))
            return 0;
        return PlayerPrefs.GetInt(GetCurrencyKey(name), MonetizationManager.Currencies[name].startAmount);
    }

    public static void SetCurrency(string name, int amount)
    {
        if (!MonetizationManager.Currencies.ContainsKey(name))
            return;
        PlayerPrefs.SetInt(GetCurrencyKey(name), amount);
        PlayerPrefs.Save();
    }

    public static bool AddCurrency(string name, int amount)
    {
        if (!MonetizationManager.Currencies.ContainsKey(name))
            return false;
        var newAmount = GetCurrency(name) + amount;
        if (newAmount < 0)
            return false;
        SetCurrency(name, newAmount);
        return true;
    }

    public static PurchasedItems GetPurchasedItems()
    {
        var json = PlayerPrefs.GetString(KeyPurchasedItemsName, "{}");
        Debug.Log("["+ Tag + "] Loading Items From Json: " + json);
        var result = JsonUtility.FromJson<PurchasedItems>(json);
        if (result == null)
            result = new PurchasedItems();
        return result;
    }

    public static void SetPurchasedItems(PurchasedItems purchasedItems)
    {
        var json = JsonUtility.ToJson(purchasedItems);
        Debug.Log("["+ Tag + "] Saving Items To Json: " + json);
        PlayerPrefs.SetString(KeyPurchasedItemsName, json);
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
