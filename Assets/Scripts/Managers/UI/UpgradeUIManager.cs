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
            return;
        }
        Instance = this;
    }

    private void Start()
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
        OpenPanel(anchorPanel, "Anchor Upgrades");
    }

    public void ShowTapPanel()
    {
        OpenPanel(tapPanel, "Tap Upgrades");
    }

    public void ShowWorkersPanel()
    {
        OpenPanel(workersPanel, "Worker Upgrades");
    }

    private void OpenPanel(GameObject panel, string title)
    {
        CloseAllPanels();
        panelTitles.text = title;
        panel.SetActive(true);
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