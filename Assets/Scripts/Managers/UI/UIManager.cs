using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public  GameObject upgradesPanel;
    public  GameObject settingsPanel;
    public  GameObject prestigePanel;
    public  GameObject rewardsPanel;
    public  GameObject bottomUI;
    public GameObject topUI;

    [Header("Fading")]
    [SerializeField] private CanvasGroup fadePanelGroup;
    [SerializeField] private float fadeDuration = 0.25f;

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
        fadePanelGroup.alpha = 0f;
        fadePanelGroup.gameObject.SetActive(false);
        
        CloseAllPanels();
    }

    public void ShowUpgrades()
    {
        OpenPanel(upgradesPanel);
    }

    public void ShowSettings()
    {
        OpenPanel(settingsPanel);
    }

    public void ShowPrestige()
    {
        OpenPanel(prestigePanel);
    }

    public void ShowRewards()
    {
        OpenPanel(rewardsPanel);
    }

    private void OpenPanel(GameObject panel)
    {
        upgradesPanel.SetActive(false);
        settingsPanel.SetActive(false);
        prestigePanel.SetActive(false);
        rewardsPanel.SetActive(false);

        panel.SetActive(true);
        HideHud();
        Time.timeScale = 0f;
    }

    public void CloseAllPanels()
    {
        upgradesPanel.SetActive(false);
        settingsPanel.SetActive(false);
        prestigePanel.SetActive(false);
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