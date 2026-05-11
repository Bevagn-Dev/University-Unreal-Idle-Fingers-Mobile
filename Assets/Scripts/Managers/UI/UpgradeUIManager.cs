using UnityEngine;
using TMPro;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject anchorPanel;
    [SerializeField] private GameObject tapPanel;
    [SerializeField] private GameObject workersPanel;
    [SerializeField] private TMP_Text panelTitles;

    [Header("Currency Text")]
    [SerializeField] private TMP_Text holdCurrencyText;
    [SerializeField] private TMP_Text tapCurrencyText;

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

    void Start()
    {
        UpdateCurrencyDisplay();
        ShowAnchorPanel();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrenciesUpdated += UpdateCurrencyDisplay;
        }    
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrenciesUpdated -= UpdateCurrencyDisplay;
        }
    }

    public void ShowAnchorPanel()
    {
        CloseAllPanels();
        panelTitles.text = "Anchor Upgrades";
        anchorPanel.SetActive(true);
    }

    public void ShowTapPanel()
    {
        CloseAllPanels();
        panelTitles.text = "Tap Upgrades";
        tapPanel.SetActive(true);
    }

    public void ShowWorkersPanel()
    {
        CloseAllPanels();
        panelTitles.text = "Worker Upgrades";
        workersPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        anchorPanel.SetActive(false);
        tapPanel.SetActive(false);
        workersPanel.SetActive(false);
    }

    private void UpdateCurrencyDisplay()
    {
        if (holdCurrencyText != null)
        {
            holdCurrencyText.text = GameManager.Instance.holdCurrency.ToString();
        }

        if (tapCurrencyText != null)
        {
            tapCurrencyText.text = GameManager.Instance.tapCurrency.ToString();
        }
    }
}