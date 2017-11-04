using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMonetizationSave : MonoBehaviour
{
    [System.Serializable]
    public class PurchasedItems
    {
        public List<string> itemNames = new List<string>();
        public void Add(string itemName)
        {
            if (itemNames != null)
                itemNames.Add(itemName);
        }

        public bool Remove(string itemName)
        {
            if (itemNames == null)
                return false;
            return itemNames.Remove(itemName);
        }

        public bool Contains(string itemName)
        {
            if (itemNames == null)
                return false;
            return itemNames.Contains(itemName);
        }
    }

    public abstract int GetCurrency(string name);
    public abstract void SetCurrency(string name, int amount);
    public abstract bool AddCurrency(string name, int amount);
    public abstract PurchasedItems GetPurchasedItems();
    public abstract void SetPurchasedItems(PurchasedItems purchasedItems);
    public abstract void AddPurchasedItem(string itemName);
}
