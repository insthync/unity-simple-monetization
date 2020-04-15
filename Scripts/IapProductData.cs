using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !NO_IAP && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
using UnityEngine.Purchasing;
#endif

[CreateAssetMenu(fileName = "IapProductData", menuName = "Monetization/IAP Product", order = 101)]
public class IapProductData : BaseProductData
{
    [HideInInspector]
    public string productId;

#if !NO_IAP && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
    public ProductCatalogItem ProductCatalogItem
    {
        get
        {
            var catalog = ProductCatalog.LoadDefaultCatalog();
            foreach (var item in catalog.allProducts)
            {
                if (item.id.Equals(productId))
                    return item;
            }
            return null;
        }
    }
    public Product ProductData
    {
        get
        {
            if (MonetizationManager.StoreController == null || MonetizationManager.StoreController.products == null)
                return null;
            return MonetizationManager.StoreController.products.WithID(productId);
        }
    }
    public ProductMetadata Metadata
    {
        get
        {
            if (ProductData == null)
                return null;
            return ProductData.metadata;
        }
    }
#endif

    [Header("Rewards")]
    public InGameCurrency[] currencies;
    public InGameProductData[] items;

    public override string GetId()
    {
        return productId;
    }

    public override string GetTitle()
    {
#if !NO_IAP && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
        if (ProductCatalogItem == null)
            return "Unknow";
        var title = ProductCatalogItem.defaultDescription.Title;
        if (Metadata != null && !string.IsNullOrEmpty(Metadata.localizedTitle))
        {
            title = Metadata.localizedTitle;
            // Try replace product name (for Android)
            title = title.Replace("(" + Application.productName + ")", "");
        }
        return title;
#else
        Debug.LogWarning("Cannot get IAP product title, Unity Purchasing is not enabled.");
        return "Unknow";
#endif
    }

    public override string GetDescription()
    {
#if !NO_IAP && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
        if (ProductCatalogItem == null)
            return "";
        var description = ProductCatalogItem.defaultDescription.Description;
        if (Metadata != null && !string.IsNullOrEmpty(Metadata.localizedDescription))
            description = Metadata.localizedDescription;
        return description;
#else
        Debug.LogWarning("Cannot get IAP product description, Unity Purchasing is not enabled.");
        return "";
#endif
    }

    public override string GetPriceText()
    {
#if !NO_IAP && UNITY_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
        if (ProductCatalogItem == null || Metadata == null)
            return "N/A";
        return Metadata.localizedPriceString;
#else
        Debug.LogWarning("Cannot get IAP product price, Unity Purchasing is not enabled.");
        return "N/A";
#endif
    }

    public override bool CanBuy()
    {
        return true;
    }

    public override void Buy(System.Action<bool, string> callback)
    {
        MonetizationManager.PurchaseCallback = callback;
        MonetizationManager.Singleton.Purchase(productId);
    }
}
