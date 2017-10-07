using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IapManager : MonoBehaviour, IStoreListener
{
    // NOTE: something about product type
    // -- Consumable product is product such as gold, gem that can be consumed
    // -- Non-Consumable product is product such as special characters/items
    // that player will buy it to unlock ability to use and will not buy it later
    // -- Subscription product is product such as weekly/monthly promotion
    public const string TAG_INIT = "IAP_INIT";
    public const string TAG_PURCHASE = "IAP_PURCHASE";
    public const string TAG_RESTORE = "IAP_RESTORE";
    public static IapManager Singleton { get; private set; }
    public static IStoreController StoreController { get; private set; }
    public static IExtensionProvider StoreExtensionProvider { get; private set; }
    public static System.Action<bool, string> PurchaseCallback;
    public static System.Action<bool, string> RestoreCallback;
    public List<IapProductData> products;
    public readonly Dictionary<string, IapProductData> Products = new Dictionary<string, IapProductData>();
    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        DontDestroyOnLoad(gameObject);
        InitializePurchasing();
    }

    private void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
            return;

        // Create a builder, first passing in a suite of Unity provided stores.
        var module = StandardPurchasingModule.Instance();
        var builder = ConfigurationBuilder.Instance(module);

        // Retrieve products from products list
        foreach (var product in products)
        {
            var productCatalogItem = product.ProductCatalogItem;
            if (productCatalogItem == null)
                continue;
            if (productCatalogItem.allStoreIDs.Count > 0)
            {
                var ids = new IDs();
                foreach (var storeID in productCatalogItem.allStoreIDs)
                {
                    ids.Add(storeID.id, storeID.store);
                }
                builder.AddProduct(productCatalogItem.id, productCatalogItem.type, ids);
            }
            else
            {
                builder.AddProduct(productCatalogItem.id, productCatalogItem.type);
            }
            Products[productCatalogItem.id] = product;
        }

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    public static bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return StoreController != null && StoreExtensionProvider != null;
    }

    public void Purchase(string productId)
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            var errorMessage = "[" + TAG_PURCHASE + "]: FAIL. Not initialized.";
            PurchaseResult(false, errorMessage);
            return;
        }

        var product = StoreController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log(string.Format("[" + TAG_PURCHASE + "] Purchasing product asychronously: '{0}'", product.definition.id));
            StoreController.InitiatePurchase(product);
        }
        else
        {
            var errorMessage = "[" + TAG_PURCHASE + "]: FAIL. Not purchasing product, either is not found or is not available for purchase.";
            RestoreResult(false, errorMessage);
        }
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            var errorMessage = "[" + TAG_RESTORE + "]: FAIL. Not initialized.";
            RestoreResult(false, errorMessage);
            return;
        }

        if (Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.WSAPlayerARM)
        {
            StoreExtensionProvider.GetExtension<IMicrosoftExtensions>().RestoreTransactions();
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS)
        {
            StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnTransactionsRestored);
        }
        else if (Application.platform == RuntimePlatform.Android && StandardPurchasingModule.Instance().appStore == AppStore.SamsungApps)
        {
            StoreExtensionProvider.GetExtension<ISamsungAppsExtensions>().RestoreTransactions(OnTransactionsRestored);
        }
        else if (Application.platform == RuntimePlatform.Android && StandardPurchasingModule.Instance().appStore == AppStore.CloudMoolah)
        {
            StoreExtensionProvider.GetExtension<IMoolahExtension>().RestoreTransactionID((restoreTransactionIDState) =>
            {
                OnTransactionsRestored(restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed && restoreTransactionIDState != RestoreTransactionIDState.NotKnown);
            });
        }
        else
        {
            var errorMessage = "[" + TAG_RESTORE + "]: FAIL. Platform: " + Application.platform.ToString() + " is not a supported platform for the Codeless IAP restore button";
            RestoreResult(false, errorMessage);
        }
    }

    private void OnTransactionsRestored(bool success)
    {
        var errorMessage = success ? "" : "";
        RestoreResult(success, errorMessage);
    }

    #region IStoreListener
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        // Overall Purchasing system, configured with products for this application.
        StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        var errorMessage = "[" + TAG_INIT + "]: Fail. InitializationFailureReason:" + error;
        Debug.LogError(errorMessage);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var productId = args.purchasedProduct.definition.id;

        IapProductData product = null;
        if (Products.TryGetValue(productId, out product))
        {
            MonetizationSave.AddHardCurrency(product.hardCurrency);
            foreach (var item in product.items)
            {
                MonetizationSave.AddPurchasedItem(item.name);
            }
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed.
        PurchaseResult(true);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        var errorMessage = "[" + TAG_PURCHASE + "]: FAIL. Product: " + product.definition.storeSpecificId + ", PurchaseFailureReason: " + failureReason;
        PurchaseResult(false, errorMessage);
    }
    #endregion

    private static void PurchaseResult(bool success, string errorMessage = "")
    {
        if (!success)
            Debug.LogError(errorMessage);
        if (PurchaseCallback != null)
        {
            PurchaseCallback(success, errorMessage);
            PurchaseCallback = null;
        }
    }

    private static void RestoreResult(bool success, string errorMessage = "")
    {
        if (!success)
            Debug.LogError(errorMessage);
        if (RestoreCallback != null)
        {
            RestoreCallback(success, errorMessage);
            RestoreCallback = null;
        }
    }
}
