using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGameCurrency : MonoBehaviour
{
    public Text textCurrencyAmount;
    public InGameCurrency currency;
    [Tooltip("Format {0} = amount, {1} = name, {2} = symbol")]
    public string displayFormat = "{1} {0} {2}";

    private void Update()
    {
        if (MonetizationManager.Currencies.ContainsKey(currency.id))
        {
            var setting = MonetizationManager.Currencies[currency.id];
            textCurrencyAmount.text = string.Format(displayFormat, currency.amount.ToString("N0"), setting.name, setting.symbol);
        }
        else
            textCurrencyAmount.text = "N/A";
    }
}
