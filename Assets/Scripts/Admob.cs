using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class Admob : MonoBehaviour
{
    // These ad units are configured to always serve tes ads.

#if UNITY_ANDROID
    [SerializeField] string _adUnitId; // google admob id for android
#else
    private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd = null;
    private static Admob instance = null;

    // 모바일 광과 SDK 초기화
    public void Awake()
    {
        // 모바일 광고 SDK 초기화
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

        //singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadInterstitialAd();
    }

    public static Admob Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    // 전면 광고 로드
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        print("Loading the interstitial ad.");

        AdRequest adRequest = new AdRequest();

        InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                Debug.LogError("interstitial ad failed to load an ad " + "with error: " + error);

                return;
            }

            Debug.Log("Interstitial ad load with response: " + ad.GetResponseInfo());

            _interstitialAd = ad;

            RegisterEventHandlers(_interstitialAd);
        });
    }

    // 전면 광고 표시
    public void ShowInterstitialAd()
    {
        if(_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            LoadInterstitialAd(); // 광고 재로드. 에러 발생 시 주석 처리 필요.
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    // 전면 광고 이벤트 수신
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // 광고 지급 관련 이벤트
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
        };

        // 추후 조사
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };

        // 광고 클릭 이벤트
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };

        // 광고가 열렸을 때
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };

        // 광고가 닫혔을 때
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");

            ElevatorHandler.Instance.ChangeScene();
        };

        // 광고가 열리지 못했을 때
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " + "with error : " + error);
        };
    }
}
