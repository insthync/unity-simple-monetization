using UnityEngine;

public class PlayerPrefsMonetizationSave : BaseMonetizationSave
{
    public const string Tag = "PlayerPrefsMonetizationSave";
    public const string KeyCurrencyName = "SaveCurrency";
    public const string KeyPurchasedItemsName = "SavePurchasedItems";
    public bool clearCurrency;
    public bool clearPurchasedItems;

    private void Start()
    {
        IsPurchasedItemsLoaded = true;
    }

    private void Update()
    {
        if (clearCurrency)
        {
            PlayerPrefs.DeleteKey(KeyCurrencyName);
            PlayerPrefs.Save();
            clearCurrency = false;
        }
        if (clearPurchasedItems)
        {
            PlayerPrefs.DeleteKey(KeyPurchasedItemsName);
            PlayerPrefs.Save();
            clearPurchasedItems = false;
        }
    }

    public static string GetCurrencyKey(string name)
    {
        return KeyCurrencyName + "_" + name;
    }

    public override int GetCurrency(string name)
    {
        if (!MonetizationManager.Currencies.ContainsKey(name))
            return 0;
        return PlayerPrefs.GetInt(GetCurrencyKey(name), MonetizationManager.Currencies[name].startAmount);
    }

    public override void SetCurrency(string name, int amount)
    {
        if (!MonetizationManager.Currencies.ContainsKey(name))
            return;
        PlayerPrefs.SetInt(GetCurrencyKey(name), amount);
        PlayerPrefs.Save();
    }

    public override bool AddCurrency(string name, int amount)
    {
        if (!MonetizationManager.Currencies.ContainsKey(name))
            return false;
        var newAmount = GetCurrency(name) + amount;
        if (newAmount < 0)
            return false;
        SetCurrency(name, newAmount);
        return true;
    }

    public override PurchasedItems GetPurchasedItems()
    {
        var json = PlayerPrefs.GetString(KeyPurchasedItemsName, "{}");
        Debug.Log("[" + Tag + "] Loading Items From Json: " + json);
        var result = JsonUtility.FromJson<PurchasedItems>(json);
        if (result == null)
            result = new PurchasedItems();
        return result;
    }

    public override void SetPurchasedItems(PurchasedItems purchasedItems)
    {
        var json = JsonUtility.ToJson(purchasedItems);
        Debug.Log("[" + Tag + "] Saving Items To Json: " + json);
        PlayerPrefs.SetString(KeyPurchasedItemsName, json);
        PlayerPrefs.Save();
    }

    public override void AddPurchasedItem(string itemName)
    {
        var list = GetPurchasedItems();
        if (!list.Contains(itemName))
            list.Add(itemName);
        SetPurchasedItems(list);
    }

    public override void RemovePurchasedItem(string itemName)
    {
        var list = GetPurchasedItems();
        if (list.Contains(itemName))
            list.Remove(itemName);
        SetPurchasedItems(list);
    }
}
