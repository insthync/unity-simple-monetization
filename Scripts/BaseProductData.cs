using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProductData : ScriptableObject
{
    public string title;
    [TextArea]
    public string description;
    public string category;
    public Texture iconImage;
    public Texture previewImage;
}
