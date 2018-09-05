using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGameProductData : UIProductData
{
    [Header("Price UIs")]
    public PriceUI[] priceUIs;

    public override void UpdateData()
    {
        base.UpdateData();

        var inGameProductData = productData as InGameProductData;
        if (inGameProductData == null)
        {
            Debug.LogWarning("[UIInGameProductData] productData is not InGameProductData");
        }
        else
        {
            foreach (var priceUI in priceUIs)
            {
                if (priceUI.textPrice == null)
                    continue;
                priceUI.textPrice.text = inGameProductData.GetPriceText(priceUI.currencyId);
            }
        }
    }

    public override void UpdateBuyButtonInteractable()
    {
        base.UpdateBuyButtonInteractable();

        if (productData == null)
            return;

        var inGameProductData = productData as InGameProductData;
        if (inGameProductData == null)
            return;

        foreach (var priceUI in priceUIs)
        {
            if (priceUI.buyButton == null)
                continue;
            priceUI.buyButton.interactable = inGameProductData.CanBuy(priceUI.currencyId);
        }
    }

    public override void OnClickBuy()
    {
        if (productData != null)
            productData.Buy(BuyResult);
    }

    public void OnClickBuy(string currencyId)
    {
        var inGameProductData = productData as InGameProductData;
        if (inGameProductData == null)
            return;

        if (productData != null)
            inGameProductData.Buy(currencyId, BuyResult);
    }
}

[System.Serializable]
public struct PriceUI
{
    public string currencyId;
    public Text textPrice;
    public Button buyButton;
}
