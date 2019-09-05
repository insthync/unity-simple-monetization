using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIProductFilter : MonoBehaviour
{
    public UIProductList uiProductList;
    public Text textFilterCategory;
    public string defaultCategoryText = "Filter";
    public UnityEvent onFilter;

    public string CurrentCategory { get; private set; }

    private void Start()
    {
        if (textFilterCategory != null)
            textFilterCategory.text = defaultCategoryText;
    }

    public void Filter(string category)
    {
        CurrentCategory = category;
        onFilter.Invoke();

        if (textFilterCategory != null)
            textFilterCategory.text = string.IsNullOrEmpty(category) ? defaultCategoryText : category;

        foreach (var ui in uiProductList.GetUIs())
        {
            try
            {
                ui.gameObject.SetActive(string.IsNullOrEmpty(category) || category.Equals(ui.productData.category));
            }
            catch
            {
                ui.gameObject.SetActive(false);
            }
        }
    }
}
