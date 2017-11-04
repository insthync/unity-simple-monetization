using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIInGameCurrency))]
public class UISavedInGameCurrency : MonoBehaviour {
    public string currencyId;
    private UIInGameCurrency uiInGameCurrency;
    private void Awake()
    {
        uiInGameCurrency = GetComponent<UIInGameCurrency>();
    }

    private void Update()
    {
        var amount = MonetizationManager.Save.GetCurrency(currencyId);
        uiInGameCurrency.currency.id = currencyId;
        uiInGameCurrency.currency.amount = amount;
    }
}
