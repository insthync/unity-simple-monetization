using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InGameProductData", menuName = "Monetization/In Game Product", order = 102)]
public class InGameProductData : BaseProductData
{
    public static System.Action<InGameProductData, System.Action<bool, string>> OverrideBuyFunction = null;
    public static System.Action<InGameProductData, string, System.Action<bool, string>> OverrideBuyWithCurrencyIdFunction = null;

    public enum PricesOption
    {
        Alternative,
        Requisite,
    }
    public string title;
    [TextArea]
    public string description;
    public PricesOption pricesOption;
    public InGameCurrency price;
    public InGameCurrency[] prices;
    public bool canBuyOnlyOnce;

    private Dictionary<string, int> cachePrices;
    public Dictionary<string, int> Prices
    {
        get
        {
            if (cachePrices == null)
            {
                cachePrices = new Dictionary<string, int>();
                if (!string.IsNullOrEmpty(price.id))
                {
                    cachePrices[price.id] = price.amount;
                }
                if (prices != null)
                {
                    foreach (var price in prices)
                    {
                        if (!string.IsNullOrEmpty(price.id))
                        {
                            cachePrices[price.id] = price.amount;
                        }
                    }
                }
            }
            return cachePrices;
        }
    }

    public virtual bool IsBought()
    {
        var list = MonetizationManager.Save.GetPurchasedItems();
        return list.Contains(GetId());
    }

    public virtual void AddPurchasedItem()
    {
        MonetizationManager.Save.AddPurchasedItem(GetId());
    }

    public virtual void RemovePurchasedItem()
    {
        MonetizationManager.Save.RemovePurchasedItem(GetId());
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

    public string GetPriceText(string currencyId)
    {
        int priceAmount;
        if (Prices.TryGetValue(currencyId, out priceAmount) &&
            MonetizationManager.Currencies.ContainsKey(currencyId))
        {
            var currency = MonetizationManager.Currencies[currencyId];
            return priceAmount.ToString("N0") + currency.symbol;
        }
        return "N/A";
    }

    public override bool CanBuy()
    {
        var canBuy = false;
        switch (pricesOption)
        {
            case PricesOption.Alternative:
                canBuy = false;
                foreach (var price in Prices)
                {
                    var currentAmount = MonetizationManager.Save.GetCurrency(price.Key);
                    if (currentAmount >= price.Value)
                    {
                        canBuy = true;
                        break;
                    }
                }
                break;
            case PricesOption.Requisite:
                canBuy = true;
                foreach (var price in Prices)
                {
                    var currentAmount = MonetizationManager.Save.GetCurrency(price.Key);
                    if (currentAmount < price.Value)
                    {
                        canBuy = false;
                        break;
                    }
                }
                break;
        }
        if (canBuyOnlyOnce)
            return !IsBought() && canBuy;
        return canBuy;
    }

    public bool CanBuy(string currencyId)
    {
        if (pricesOption == PricesOption.Requisite)
            return CanBuy();
        var currency = 0;
        if (Prices.ContainsKey(currencyId))
            currency = MonetizationManager.Save.GetCurrency(currencyId);
        if (canBuyOnlyOnce)
            return !IsBought() && currency >= price.amount;
        return currency >= price.amount;
    }

    public override void Buy(System.Action<bool, string> callback)
    {
        if (OverrideBuyFunction != null)
        {
            OverrideBuyFunction.Invoke(this, callback);
            return;
        }

        if (!CanBuy())
        {
            if (callback != null)
                callback.Invoke(false, "Cannot buy item.");
            return;
        }

        switch (pricesOption)
        {
            case PricesOption.Alternative:
                foreach (var price in Prices)
                {
                    var currentAmount = MonetizationManager.Save.GetCurrency(price.Key);
                    if (currentAmount >= price.Value)
                    {
                        MonetizationManager.Save.AddCurrency(price.Key, -price.Value);
                        break;
                    }
                }
                break;
            case PricesOption.Requisite:
                foreach (var price in Prices)
                {
                    MonetizationManager.Save.AddCurrency(price.Key, -price.Value);
                }
                break;
        }

        AddPurchasedItem();
        if (callback != null)
            callback.Invoke(true, string.Empty);
    }

    public void Buy(string currencyId, System.Action<bool, string> callback)
    {
        if (OverrideBuyWithCurrencyIdFunction != null)
        {
            OverrideBuyWithCurrencyIdFunction.Invoke(this, currencyId, callback);
            return;
        }

        if (pricesOption == PricesOption.Requisite)
        {
            Buy(callback);
            return;
        }

        if (!CanBuy(currencyId))
        {
            if (callback != null)
                callback.Invoke(false, "Cannot buy item.");
            return;
        }

        MonetizationManager.Save.AddCurrency(currencyId, -Prices[currencyId]);
        AddPurchasedItem();
        if (callback != null)
            callback.Invoke(true, string.Empty);
    }
}
