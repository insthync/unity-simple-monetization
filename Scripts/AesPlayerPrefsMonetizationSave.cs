using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AesPlayerPrefsMonetizationSave : BaseMonetizationSave
{
    public const string Tag = "AesPlayerPrefsMonetizationSave";
    public const string KeyCurrencyName = "AesSaveCurrency";
    public const string KeyPurchasedItemsName = "AesSavePurchasedItems";
    public const string KeyCurrencyIVName = "AesSaveCurrencyIV";
    public const string KeyPurchasedItemsIVName = "AesSavePurchasedItemsIV";

    public string salt;

    public static string GetCurrencyKey(string name)
    {
        return KeyCurrencyName + "_" + name;
    }

    public static string GetCurrencyIVKey(string name)
    {
        return KeyCurrencyIVName + "_" + name;
    }

    public override int GetCurrency(string name)
    {
        if (!MonetizationManager.Currencies.ContainsKey(name))
            return 0;

        var result = MonetizationManager.Currencies[name].startAmount;
        var encryptedText = PlayerPrefs.GetString(GetCurrencyKey(name), "");
        var IV = PlayerPrefs.GetString(GetCurrencyIVKey(name), "");
        if (encryptedText.Length > 0 && IV.Length > 0)
        {
            var currencyText = EncryptionUtility.AESDecrypt(encryptedText, salt, IV);
            int.TryParse(currencyText, out result);
        }

        return result;
    }

    public override void SetCurrency(string name, int amount)
    {
        if (!MonetizationManager.Currencies.ContainsKey(name))
            return;

        var IV = EncryptionUtility.GenerateIV();
        var encryptedText = EncryptionUtility.AESEncrypt(amount.ToString(), salt, IV);
        PlayerPrefs.SetString(GetCurrencyKey(name), encryptedText);
        PlayerPrefs.SetString(GetCurrencyIVKey(name), IV);
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
        var json = "{}";
        var encryptedText = PlayerPrefs.GetString(KeyPurchasedItemsName, "");
        var IV = PlayerPrefs.GetString(KeyPurchasedItemsIVName, "");
        if (encryptedText.Length > 0 && IV.Length > 0)
            json = EncryptionUtility.AESDecrypt(encryptedText, salt, IV);
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
        var IV = EncryptionUtility.GenerateIV();
        var encryptedText = EncryptionUtility.AESEncrypt(json, salt, IV);
        PlayerPrefs.SetString(KeyPurchasedItemsName, encryptedText);
        PlayerPrefs.SetString(KeyPurchasedItemsIVName, IV);
        PlayerPrefs.Save();
    }

    public override void AddPurchasedItem(string itemName)
    {
        var list = GetPurchasedItems();
        if (!list.Contains(itemName))
            list.Add(itemName);
        SetPurchasedItems(list);
    }
}
