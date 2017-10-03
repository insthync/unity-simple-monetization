using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProductList : MonoBehaviour
{
    public BaseProductData[] products;
    public UIProductData prefab;
    public Transform container;
    private readonly Dictionary<string, UIProductData> UIs = new Dictionary<string, UIProductData>();

    private void Awake()
    {
        foreach (var product in products)
        {
            AddProduct(product);
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
        foreach (var child in container)
        {
            Destroy(container.gameObject);
        }
    }
}
