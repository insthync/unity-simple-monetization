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
    public bool canBuyOnlyOnce;
    public virtual bool IsUnlock()
    {
        var list = MonetizationSave.GetPurchasedItems();
        return !isLock || list.Contains(name);
    }

    public virtual bool CanBuy()
    {
        var currency = MonetizationSave.GetCurrency();
        if (canBuyOnlyOnce)
            return !IsUnlock() && currency >= price;
        return currency >= price;
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

    public override void Buy(System.Action<bool, string> callback)
    {
        if (!CanBuy())
        {
            if (callback != null)
                callback(false, "Cannot buy item.");
            return;
        }
        MonetizationSave.AddCurrency(-price);
        Unlock();
        if (callback != null)
            callback(true, string.Empty);
    }
}
