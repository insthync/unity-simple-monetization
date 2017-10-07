using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIProductData : MonoBehaviour
{
    [Header("UI")]
    public Text textTitle;
    public Text textDescription;
    public Text textPrice;
    public RawImage iconImage;
    public RawImage previewImage;
    [Header("Events")]
    public UnityEvent OnBuySuccess;
    public UnityEvent<string> OnBuyFailed;
    [Header("Product Data")]
    public BaseProductData productData;
    private BaseProductData dirtyProductData;

    private void Update()
    {
        if (productData != dirtyProductData)
        {
            UpdateData();
            dirtyProductData = productData;
        }
    }

    public void UpdateData()
    {
        var title = "";
        var description = "";
        var price = "";
        Texture iconTexture = null;
        Texture previewTexture = null;
        if (productData != null)
        {
            title = productData.GetTitle();
            description = productData.GetDescription();
            price = productData.GetPriceText();
            iconTexture = productData.iconTexture;
            previewTexture = productData.previewTexture;
        }
        if (textTitle != null)
            textTitle.text = title;
        if (textDescription != null)
            textDescription.text = description;
        if (textPrice != null)
            textPrice.text = price;
        if (iconImage != null)
            iconImage.texture = iconTexture;
        if (previewImage != null)
            previewImage.texture = previewTexture;
    }

    public void OnClickBuy()
    {
        if (productData != null)
            productData.Buy(BuyResult);
    }

    private void BuyResult(bool success, string errorMessage)
    {
        if (success)
            OnBuySuccess.Invoke();
        else
            OnBuyFailed.Invoke(errorMessage);
    }
}
