﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameProductData : BaseProductData
{
    public string title;
    [TextArea]
    public string description;
    public int price;
    public bool isLock;
    public virtual bool IsUnlock()
    {
        var list = MonetizationSave.GetPurchasedItems();
        return !isLock || list.Contains(name);
    }

    public virtual bool CanBuy()
    {
        var hardCurrency = MonetizationSave.GetHardCurrency();
        return hardCurrency >= price;
    }

    public virtual void Unlock()
    {
        MonetizationSave.AddPurchasedItem(name);
    }

    public override string GetId()
    {
        return name;
    }

    public override string GetTitle()
    {
        return title;
    }

    public override string GetDescription()
    {
        return description;
    }

    public override string GetPriceText()
    {
        return price.ToString("N0");
    }

    public override void Buy(System.Action<bool> result)
    {
        if (!CanBuy())
        {
            if (result != null)
                result(false);
            return;
        }
        MonetizationSave.AddHardCurrency(-price);
        Unlock();
        if (result != null)
            result(true);
    }
}
