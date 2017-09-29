using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IapProductData : ScriptableObject
{
    public Texture iconImage;
    public Texture previewImage;
    [HideInInspector]
    public string productId;

    private ProductCatalogItem productCatalogItem = null;
    public ProductCatalogItem ProductCatalogItem
    {
        get
        {
            if (productCatalogItem == null)
            {
                var catalog = ProductCatalog.LoadDefaultCatalog();
                foreach (var item in catalog.allProducts)
                {
                    if (item.id.Equals(productId))
                    {
                        productCatalogItem = item;
                        break;
                    }
                }
            }
            return productCatalogItem;
        }
    }
    public int hardCurrency;
    public List<InGameProductData> items;
}
