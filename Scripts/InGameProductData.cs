using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameProductData : BaseProductData
{
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

    public virtual bool Buy()
    {
        if (!CanBuy())
            return false;
        MonetizationSave.AddHardCurrency(-price);
        Unlock();
        return true;
    }
}
