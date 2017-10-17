using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class UIWatchAdsButton : MonoBehaviour
{
    [System.Serializable]
    public class ShowAdResultEvent : UnityEvent<MonetizationManager.RemakeShowResult> { }
    public enum WatchAdsButtonType
    {
        RewardCurrency,
        Custom
    }
    public string adsPlacement;
    public WatchAdsButtonType buttonType;
    public ShowAdResultEvent onShowAdResult;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public string AdsPlacement
    {
        get
        {
            switch (buttonType)
            {
                case WatchAdsButtonType.RewardCurrency:
                    return MonetizationManager.Singleton.rewardVideoPlacement;
                default:
                    return adsPlacement;
            }
        }
    }

    private void Update()
    {
        button.interactable = MonetizationManager.IsAdsReady(AdsPlacement);
    }

    private void OnClick()
    {
        switch (buttonType)
        {
            case WatchAdsButtonType.RewardCurrency:
                MonetizationManager.ShowRewardedAd(OnShowAdResult);
                break;
            default:
                MonetizationManager.ShowAd(adsPlacement, OnShowAdResult);
                break;
        }
    }

    private void OnShowAdResult(MonetizationManager.RemakeShowResult result)
    {
        onShowAdResult.Invoke(result);
    }
}
