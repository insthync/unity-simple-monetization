using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IapProductData : BaseProductData
{
    [HideInInspector]
    public string productId;
    
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
    [Header("Rewards")]
    public InGameCurrency currency;
    public List<InGameProductData> items;

    public override string GetId()
    {
        return productId;
    }

    public override string GetTitle()
    {
        if (ProductCatalogItem == null)
            return "Unknow";
        var title = ProductCatalogItem.defaultDescription.Title;
        if (Metadata != null && !string.IsNullOrEmpty(Metadata.localizedTitle))
            title = Metadata.localizedTitle;
        return title;
    }

    public override string GetDescription()
    {
        if (ProductCatalogItem == null)
            return "";
        var description = ProductCatalogItem.defaultDescription.Description;
        if (Metadata != null && !string.IsNullOrEmpty(Metadata.localizedDescription))
            description = Metadata.localizedDescription;
        return description;
    }

    public override string GetPriceText()
    {
        if (ProductCatalogItem == null || Metadata == null)
            return "N/A";
        return Metadata.localizedPriceString;
    }

    public override void Buy(System.Action<bool, string> callback)
    {
        MonetizationManager.PurchaseCallback = callback;
        MonetizationManager.Singleton.Purchase(productId);
    }
}
