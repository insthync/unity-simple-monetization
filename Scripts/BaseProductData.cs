using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProductData : ScriptableObject
{
    public Texture iconTexture;
    public Texture previewTexture;
    public abstract string GetId();
    public abstract string GetTitle();
    public abstract string GetDescription();
    public abstract string GetPriceText();
    public abstract bool CanBuy();
    public abstract void Buy(System.Action<bool, string> callback);
}
