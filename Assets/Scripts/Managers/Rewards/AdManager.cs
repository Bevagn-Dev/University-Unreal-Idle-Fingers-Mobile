using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdManager Instance { get; private set; }

    [Header("Platform Credentials")]
    [SerializeField] private string androidGameId;
    [SerializeField] private string iOSGameId;
    [SerializeField] private bool testMode = true;

    private string adUnitId;
    
    private BoostType activeRewardType;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeAds();
    }

    private void InitializeAds()
    {
        string gameId = Application.platform == RuntimePlatform.IPhonePlayer ? iOSGameId : androidGameId;
        adUnitId = Application.platform == RuntimePlatform.IPhonePlayer ? "Rewarded_iOS" : "Rewarded_Android";
        
        Advertisement.Initialize(gameId, testMode, this);
    }

    public void ShowAd(BoostType rewardType)
    {
        activeRewardType = rewardType;
        Advertisement.Load(adUnitId, this);
    }

    public void OnInitializationComplete() { }
    
    public void OnInitializationFailed(UnityAdsInitializationError error, string message) 
    { 
        Debug.LogError($"Ads Initialization Failed: {error} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(placementId, this);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) 
    { 
        Debug.LogError($"Ad Load Failed: {error} - {message}");
    }
    
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) 
    { 
        Debug.LogError($"Ad Show Failed: {error} - {message}");
    }
    
    public void OnUnityAdsShowStart(string placementId) { }
    
    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {

        if (placementId == adUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            RewardBoostManager.Instance.ActivateBoost(activeRewardType);
        }
    }
}