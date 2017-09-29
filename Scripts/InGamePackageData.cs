using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePackageData : InGameProductData
{
    public List<InGameProductData> items;

    public override void Unlock()
    {
        base.Unlock();
        foreach (var item in items)
        {
            item.Unlock();
        }
    }
}
