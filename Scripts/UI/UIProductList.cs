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
    [Header("Product list creating with UI Prefab")]
    public BaseProductData[] products;
    public UIProductData prefab;
    public Transform container;
    [Header("Ready to use UI Product List")]
    public UIProductData[] uiProducts;
    [Header("Events")]
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
        foreach (var uiProduct in uiProducts)
        {
            if (uiProduct == null) continue;
            SetupUIProductData(uiProduct);
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
        SetupUIProductData(ui);
    }

    public void SetupUIProductData(UIProductData ui)
    {
        if (ui.productData == null)
        {
            Debug.LogWarning("[UI Product List] Some ui's product data is empty");
            return;
        }
        ui.list = this;
        UIs[ui.productData.GetId()] = ui;
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
            if (!IsUIProductGameObject(child.gameObject))
                Destroy(child.gameObject);
        }
    }

    public bool IsUIProductGameObject(GameObject go)
    {
        foreach (var uiProduct in uiProducts)
        {
            if (uiProduct == null) continue;
            if (uiProduct.gameObject == go)
                return true;
        }
        return false;
    }

    public List<UIProductData> GetUIs()
    {
        return new List<UIProductData>(UIs.Values);
    }
}
