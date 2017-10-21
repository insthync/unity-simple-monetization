using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePackageData : InGameProductData
{
    [Header("Rewards")]
    public InGameProductData[] items;

    public override void AddPurchasedItem()
    {
        base.AddPurchasedItem();
        foreach (var item in items)
        {
            item.AddPurchasedItem();
        }
    }
}
