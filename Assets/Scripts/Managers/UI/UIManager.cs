using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject upgradesPanel;
    public GameObject settingsPanel;
    public GameObject presigePanel;
    public GameObject rewardsPanel;
    public GameObject bottomUI;
    public GameObject topUI;

    [Header("Fading")]
    public CanvasGroup fadePanelGroup;
    [SerializeField] private float fadeDuration = 0.25f;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
        }
    }

    private void Start()
    {
        fadePanelGroup.alpha = 0f;
        fadePanelGroup.gameObject.SetActive(false);
        
        CloseAllPanels();
    }

    public void ShowUpgrades()
    {
        CloseAllPanels();
        HideHud();
        upgradesPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowSettings()
    {
        CloseAllPanels();
        HideHud();
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowPrestige()
    {
        CloseAllPanels();
        HideHud();
        presigePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowRewards()
    {
        CloseAllPanels();
        HideHud();
        rewardsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseAllPanels()
    {
        upgradesPanel.SetActive(false);
        settingsPanel.SetActive(false);
        presigePanel.SetActive(false);
        rewardsPanel.SetActive(false);

        fadePanelGroup.DOKill();
        fadePanelGroup.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() => 
        {
            fadePanelGroup.gameObject.SetActive(false);
        });

        bottomUI.SetActive(true);
        topUI.SetActive(true);

        Time.timeScale = 1f;
    }

    private void HideHud()
    {
        fadePanelGroup.gameObject.SetActive(true);
        
        fadePanelGroup.DOKill();
        fadePanelGroup.DOFade(1f, fadeDuration).SetUpdate(true);

        bottomUI.SetActive(false);
        topUI.SetActive(false);
    }
}