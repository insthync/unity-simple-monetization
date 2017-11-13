using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

#if UNITY_PURCHASING
public class MonetizationManager : MonoBehaviour, IStoreListener
#else
public class MonetizationManager : MonoBehaviour
#endif
{
    /// <summary>
    /// This is remake of `ShowResult` enum.
    /// Will uses when Unity's Ads not available for some platforms (such as standalone)
    /// to avoid compile errors.
    /// </summary>
    public enum RemakeShowResult
    {
        Finished,
        Skipped,
        Failed,
        NotReady,
    }
    // NOTE: something about product type
    // -- Consumable product is product such as gold, gem that can be consumed
    // -- Non-Consumable product is product such as special characters/items
    // that player will buy it to unlock ability to use and will not buy it later
    // -- Subscription product is product such as weekly/monthly promotion
    public const string TAG_INIT = "IAP_INIT";
    public const string TAG_PURCHASE = "IAP_PURCHASE";
    public const string TAG_RESTORE = "IAP_RESTORE";
    public static MonetizationManager Singleton { get; private set; }
#if UNITY_PURCHASING
    public static IStoreController StoreController { get; private set; }
    public static IExtensionProvider StoreExtensionProvider { get; private set; }
#endif
    public static System.Action<bool, string> PurchaseCallback;
    public static System.Action<bool, string> RestoreCallback;
    [Header("Unity monetize settings")]
    public string androidGameId;
    public string iosGameId;
    public bool testMode;
    [Header("In-game products")]
    public List<IapProductData> products;
    public List<InGameCurrencySetting> currencies;
    [Header("Advertisement")]
    public string rewardVideoPlacement = "rewardedVideo";
    public AdsReward[] adsRewards;
    [Header("Save")]
    public BaseMonetizationSave save;
    public static readonly Dictionary<string, IapProductData> Products = new Dictionary<string, IapProductData>();
    public static readonly Dictionary<string, InGameCurrencySetting> Currencies = new Dictionary<string, InGameCurrencySetting>();
    public static readonly Dictionary<AdsReward, int> AdsRewards = new Dictionary<AdsReward, int>();

    public static BaseMonetizationSave Save
    {
        get
        {
            if (Singleton == null)
                return null;

            if (Singleton.save == null)
                Singleton.save = Singleton.GetComponent<BaseMonetizationSave>();
            if (Singleton.save == null)
                Singleton.save = Singleton.gameObject.AddComponent<PlayerPrefsMonetizationSave>();
            return Singleton.save;
        }
    }
    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        DontDestroyOnLoad(gameObject);
        InitializeCurrencies();
        InitializeAds();
        InitializePurchasing();
        InitializeAdsRewards();
    }

#region Initailize functions
    private void InitializeAds()
    {
#if UNITY_ANDROID && UNITY_ADS
        Advertisement.Initialize(androidGameId, testMode);
#elif UNITY_IOS && UNITY_ADS
        Advertisement.Initialize(iosGameId, testMode);
#else
        Debug.LogWarning("Cannot initialize advertisement, Unity Ads is not enabled or not supported platforms.");
#endif
    }

    private void InitializePurchasing()
    {
#if (UNITY_ANDROID || UNITY_IOS) && UNITY_PURCHASING
        // If we have already connected to Purchasing ...
        if (IsPurchasingInitialized())
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

            var logMessage = "[" + TAG_INIT + "]: Adding product " + productCatalogItem.id + " type " + productCatalogItem.type.ToString();
            Debug.Log(logMessage);
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
        try
        {
            UnityPurchasing.Initialize(this, builder);
        }
        catch (System.InvalidOperationException ex)
        {
            var errorMessage = "[" + TAG_INIT + "]: Cannot initialize purchasing, the platform may not supports.";
            Debug.LogError(errorMessage);
            Debug.LogException(ex);
        }
#else
        Debug.LogWarning("Cannot initialize purchasing, Unity Purchasing is not enabled or not supported platforms.");
#endif
    }

    private void InitializeCurrencies()
    {
        foreach (var currency in currencies)
        {
            Currencies[currency.id] = currency;
        }
    }

    private void InitializeAdsRewards()
    {
        foreach (var reward in adsRewards)
        {
            AdsRewards[reward] = reward.randomWeight;
        }
    }

    public static bool IsPurchasingInitialized()
    {
#if UNITY_PURCHASING
        // Only say we are initialized if both the Purchasing references are set.
        return StoreController != null && StoreExtensionProvider != null;
#else
        return false;
#endif
    }
    #endregion

    #region ADS Actions
#if UNITY_ADS
    private static RemakeShowResult ConvertToRemakeShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                return RemakeShowResult.Finished;
            case ShowResult.Skipped:
                return RemakeShowResult.Skipped;
            case ShowResult.Failed:
                return RemakeShowResult.Failed;
        }
        return RemakeShowResult.Failed;
    }

    private static ShowResult ConvertToUnityShowResult(RemakeShowResult result)
    {
        switch (result)
        {
            case RemakeShowResult.Finished:
                return ShowResult.Finished;
            case RemakeShowResult.Skipped:
                return ShowResult.Skipped;
            case RemakeShowResult.Failed:
                return ShowResult.Failed;
        }
        return ShowResult.Failed;
    }
#endif

    public static void ShowAd(string placement, System.Action<RemakeShowResult> showResultHandler)
    {
#if UNITY_ADS
        if (Advertisement.IsReady(placement))
        {
            var options = new ShowOptions
            {
                resultCallback = (result) =>
                {
                    if (showResultHandler != null)
                        showResultHandler(ConvertToRemakeShowResult(result));
                }
            };
            Advertisement.Show(placement, options);
        }
        else
        {
            if (showResultHandler != null)
                showResultHandler(RemakeShowResult.NotReady);
        }
#else
        Debug.LogWarning("Cannot show advertisement, Unity Ads is not enabled");
        if (showResultHandler != null)
            showResultHandler(RemakeShowResult.NotReady);
#endif
    }

    public static bool IsAdsReady(string placement)
    {
#if UNITY_ADS
        return Advertisement.IsReady(placement);
#else
        return false;
#endif
    }

    public static void ShowRewardedAd(System.Action<RemakeShowResult> showResultHandler)
    {
        ShowAd(Singleton.rewardVideoPlacement, (result) =>
        {
            if (result == RemakeShowResult.Finished)
            {
                var randomizer = WeightedRandomizer.From(AdsRewards);
                var reward = randomizer.TakeOne();
                var currencies = reward.currencies;
                foreach (var currency in currencies)
                {
                    Save.AddCurrency(currency.id, currency.amount);
                }
                var items = reward.items;
                foreach (var item in items)
                {
                    Save.AddPurchasedItem(item.name);
                }
            }
            if (showResultHandler != null)
                showResultHandler(result);
        });
    }
#endregion

#region IAP Actions
    public void Purchase(string productId)
    {
#if (UNITY_ANDROID || UNITY_IOS) && UNITY_PURCHASING
        // If Purchasing has not yet been set up ...
        if (!IsPurchasingInitialized())
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
#else
        Debug.LogWarning("Cannot purchase product, Unity Purchasing is not enabled.");
#endif
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
#if (UNITY_ANDROID || UNITY_IOS) && UNITY_PURCHASING
        // If Purchasing has not yet been set up ...
        if (!IsPurchasingInitialized())
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
#else
        Debug.LogWarning("Cannot restore product, Unity Purchasing is not enabled.");
#endif
    }

    private void OnTransactionsRestored(bool success)
    {
        var errorMessage = success ? "" : "";
        RestoreResult(success, errorMessage);
    }
    #endregion

    #region IStoreListener
#if UNITY_PURCHASING
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        // Overall Purchasing system, configured with products for this application.
        StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        StoreExtensionProvider = extensions;
        var productCount = StoreController.products.all.Length;
        var logMessage = "[" + TAG_INIT + "]: OnInitialized with " + productCount + " products";
        Debug.Log(logMessage);
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
            var currencies = product.currencies;
            foreach (var currency in currencies)
            {
                Save.AddCurrency(currency.id, currency.amount);
            }
            var items = product.items;
            foreach (var item in items)
            {
                Save.AddPurchasedItem(item.name);
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
#endif
#endregion

#region Callback Events
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
#endregion
}
