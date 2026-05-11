using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject upgradesPanel;
    public GameObject settingsPanel;
    public GameObject presigePanel;
    public GameObject rewardsPanel;
    public GameObject fadePanel;
    public GameObject bottomUI;
    public GameObject topUI;

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
        fadePanel.SetActive(false);

        bottomUI.SetActive(true);
        topUI.SetActive(true);

        Time.timeScale = 1f;
    }

    private void HideHud()
    {
        fadePanel.SetActive(true);
        bottomUI.SetActive(false);
        topUI.SetActive(false);
    }
}