using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject holdPanel;
    public GameObject pressPanel;
    public GameObject comboPanel;
    public GameObject autoPanel;

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

    public void ShowHoldPanel()
    {
        CloseAllPanels();
        holdPanel.SetActive(true);
    }

    public void ShowPressPanel()
    {
        CloseAllPanels();
        pressPanel.SetActive(true);
    }

    public void ShowComboPanel()
    {
        CloseAllPanels();
        comboPanel.SetActive(true);
    }

    public void ShowAutoPanel()
    {
        CloseAllPanels();
        autoPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        holdPanel.SetActive(false);
        pressPanel.SetActive(false);
        comboPanel.SetActive(false);
        autoPanel.SetActive(false);
    }
}
