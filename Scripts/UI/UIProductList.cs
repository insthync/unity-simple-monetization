using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIProductList : MonoBehaviour
{
    [System.Serializable]
    public class PurchaseSuccessEvent : UnityEvent { }
    [System.Serializable]
    public class PurchaseFailEvent : UnityEvent<string> { }
    public BaseProductData[] products;
    public UIProductData prefab;
    public Transform container;
    public PurchaseSuccessEvent onPurchaseSuccess;
    public PurchaseFailEvent onPurchaseFail;
    private readonly Dictionary<string, UIProductData> UIs = new Dictionary<string, UIProductData>();

    private void Awake()
    {
        ClearProducts();
        foreach (var product in products)
        {
            AddProduct(product);
        }
    }

    public void UpdateBuyButtonsInteractable()
    {
        foreach (var ui in UIs)
        {
            ui.Value.UpdateBuyButtonInteractable();
        }
    }

    public void AddProduct(BaseProductData productData)
    {
        if (productData == null || UIs.ContainsKey(productData.GetId()))
            return;
        var uiObject = Instantiate(prefab.gameObject);
        uiObject.SetActive(true);
        uiObject.transform.SetParent(container, false);
        var ui = uiObject.GetComponent<UIProductData>();
        ui.productData = productData;
        ui.list = this;
        ui.UpdateBuyButtonInteractable();
        UIs[productData.GetId()] = ui;
    }

    public bool RemoveProduct(string id)
    {
        if (string.IsNullOrEmpty(id) || !UIs.ContainsKey(id))
            return false;
        var ui = UIs[id];
        if (UIs.Remove(id))
        {
            Destroy(ui.gameObject);
            return true;
        }
        return false;
    }

    public void ClearProducts()
    {
        UIs.Clear();
        for (var i = 0; i < container.childCount; ++i)
        {
            var child = container.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}
