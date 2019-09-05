using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProductOwningActivatator : MonoBehaviour
{
    public UIInGameProductData uiProductData;
    public GameObject[] activatingWhileOwningObjects;
    public GameObject[] activatingWhileNotOwningObjects;

    private void Update()
    {
        if (uiProductData == null || uiProductData.productData == null)
            return;

        var inGameProductData = uiProductData.productData as InGameProductData;
        foreach (var obj in activatingWhileOwningObjects)
        {
            obj.SetActive(inGameProductData.IsBought());
        }
        foreach (var obj in activatingWhileNotOwningObjects)
        {
            obj.SetActive(!inGameProductData.IsBought());
        }
    }
}
