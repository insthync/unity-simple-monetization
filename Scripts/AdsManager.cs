using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour
{
    public const string PlacementRewardedVideo = "RewardedVideoPlacement";
    public static AdsManager Singleton { get; private set; }
    public int rewardCurrency;
    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

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
        if (showResultHandler != null)
            showResultHandler(RemakeShowResult.NotReady);
#endif
    }

    public static void ShowRewardedAd(System.Action<RemakeShowResult> showResultHandler)
    {
        ShowAd(PlacementRewardedVideo, (result) =>
        {
            if (result == RemakeShowResult.Finished)
                MonetizationSave.AddCurrency(Singleton.rewardCurrency);
            if (showResultHandler != null)
                showResultHandler(result);
        });
    }
    
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
}
