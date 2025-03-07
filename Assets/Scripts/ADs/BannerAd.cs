using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;

public class BannerAd : MonoBehaviour
{
    private BannerView _bannerView;

    private string _adUnitId;

    public void Start()
    {
        // Set the ad unit ID based on the platform.
#if UNITY_ANDROID
        _adUnitId = "ca-app-pub-7666692145810371~7951767930"; // Test ad unit ID for Android
#elif UNITY_IPHONE
        _adUnitId = "ca-app-pub-3940256099942544/2934735716"; // Test ad unit ID for iOS
#else
        _adUnitId = "unused";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
            CreateAndLoadBannerAd();
        });
    }

    /// <summary>
    /// Creates and loads a banner ad.
    /// </summary>
    private void CreateAndLoadBannerAd()
    {
        // Dispose of any existing banner view to avoid duplicates.
     /*   if (_bannerView != null)
        {
            _bannerView.Destroy();
        }*/

        // Create a 320x50 banner at the top of the screen.
        _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an ad request and load the ad.
        AdRequest adRequest = new AdRequest();
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Clean up the banner ad when the script is destroyed.
    /// </summary>
  /*  private void OnDestroy()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }
    }*/
}
