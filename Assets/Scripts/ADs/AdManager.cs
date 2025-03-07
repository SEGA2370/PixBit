using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    private InterstitialAd _interstitialAd;

    // Ad Unit ID - replace with your actual Ad Unit ID from AdMob.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-1107660696799331/4376549694"; // Test Ad Unit ID for Android
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/4411468910"; // Test Ad Unit ID for iOS
#else
    private string _adUnitId = "unused";
#endif

    private float _lastAdTime; // Track the last ad display time
    private const float AdCooldown = 30f; // Ad cooldown time (2 minutes)

    public event Action OnAdClosed; // Event to notify when the ad is closed

    private void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { Debug.Log("AdMob initialized."); });

        // Initialize the cooldown to allow the first ad immediately.
        _lastAdTime = Time.time - AdCooldown;

        // Load the first interstitial ad.
        LoadInterstitialAd();
    }

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");
        var adRequest = new AdRequest();

        InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                return;
            }

            Debug.Log("Interstitial ad loaded successfully.");
            _interstitialAd = ad;
            RegisterEventHandlers(_interstitialAd);
            RegisterReloadHandler(_interstitialAd);
        });
    }

    public bool CanShowAd()
    {
        return (Time.time - _lastAdTime) >= AdCooldown;
    }

    public void ShowAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd() && CanShowAd())
        {
            _interstitialAd.Show();
            _lastAdTime = Time.time; // Reset cooldown timer
        }
        else
        {
            Debug.Log("Ad not ready or still in cooldown.");
        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Ad closed full screen content.");
            OnAdClosed?.Invoke(); // Trigger the OnAdClosed event
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Ad failed to open full screen content: " + error);
            OnAdClosed?.Invoke(); // Ensure the game continues even if the ad fails
        };
    }

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Ad closed. Reloading...");
            LoadInterstitialAd();
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Ad failed to open. Reloading...");
            LoadInterstitialAd();
        };
    }
}
