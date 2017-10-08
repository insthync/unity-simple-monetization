using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameProductData : BaseProductData
{
    public string title;
    [TextArea]
    public string description;
    public InGameCurrency price;
    public bool isLock;
    public bool canBuyOnlyOnce;
    public virtual bool IsUnlock()
    {
        var list = MonetizationSave.GetPurchasedItems();
        return !isLock || list.Contains(name);
    }

    public virtual bool CanBuy()
    {
        var currency = MonetizationSave.GetCurrency(price.id);
        if (canBuyOnlyOnce)
            return !IsUnlock() && currency >= price.amount;
        return currency >= price.amount;
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
        if (MonetizationManager.Currencies.ContainsKey(price.id))
        {
            var currency = MonetizationManager.Currencies[price.id];
            return price.amount.ToString("N0") + currency.symbol;
        }
        return "N/A";
    }

    public override void Buy(System.Action<bool, string> callback)
    {
        if (!CanBuy())
        {
            if (callback != null)
                callback(false, "Cannot buy item.");
            return;
        }
        MonetizationSave.AddCurrency(price.id, -price.amount);
        Unlock();
        if (callback != null)
            callback(true, string.Empty);
    }
}
