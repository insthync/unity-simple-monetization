using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InGamePackageData", menuName = "Monetization/In Game Package", order = 103)]
public class InGamePackageData : InGameProductData
{
    [Header("Rewards")]
    public InGameCurrency[] currencies;
    public InGameProductData[] items;

    public override void AddPurchasedItem()
    {
        base.AddPurchasedItem();
        foreach (var currency in currencies)
        {
            MonetizationManager.Save.AddCurrency(currency.id, currency.amount);
        }
        foreach (var item in items)
        {
            item.AddPurchasedItem();
        }
    }
}
