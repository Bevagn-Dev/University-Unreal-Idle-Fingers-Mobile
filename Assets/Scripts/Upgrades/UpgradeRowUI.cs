using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeRowUI : MonoBehaviour
{
    [SerializeField] private UpgradeData data;

    [Space(10)]

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button buyButton;
    
    [Space(10)]
    
    [SerializeField] private Image currencyIconImage;
    [SerializeField] private Sprite holdCurrencySprite;
    [SerializeField] private Sprite tapCurrencySprite;

    private void Start()
    {
        UpdateUI();

        GameManager.Instance.OnCurrenciesUpdated += CheckAffordability;
        UpgradeManager.Instance.OnUpgradePurchased += HandleUpgradePurchased;

        buyButton.onClick.AddListener(OnBuyClicked);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrenciesUpdated -= CheckAffordability;
        }

        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnUpgradePurchased -= HandleUpgradePurchased;
        }
    }

    private void HandleUpgradePurchased(UpgradeData purchasedData)
    {
        if (purchasedData.upgradeID == data.upgradeID)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        nameText.text = data.displayName;

        int currentLevel = UpgradeManager.Instance.GetUpgradeLevel(data.upgradeID);
        float currentValue = UpgradeManager.Instance.GetUpgradeCurrentValue(data);

        if (data.maxLevel == 1)
        {
            valueText.text = currentLevel > 0 ? "Unlocked" : "Locked";
        }
        else
        {
            valueText.text = ($"x{currentValue}");
        }

        if (currencyIconImage != null)
        {
            currencyIconImage.sprite = data.currencyType == CurrencyType.Hold ? holdCurrencySprite : tapCurrencySprite;
        }

        if (currentLevel >= data.maxLevel)
        {
            costText.text = "MAX";
            buyButton.interactable = false;
        }
        else
        {
            float cost = UpgradeManager.Instance.GetUpgradeCost(data);
            costText.text = cost.ToString();
            CheckAffordability();
        }
    }

    private void CheckAffordability()
    {
        int currentLevel = UpgradeManager.Instance.GetUpgradeLevel(data.upgradeID);

        if (currentLevel >= data.maxLevel)
        {
            return;
        }

        float cost = UpgradeManager.Instance.GetUpgradeCost(data);
        bool canAfford = false;

        if (data.currencyType == CurrencyType.Hold)
        {
            canAfford = GameManager.Instance.holdCurrency >= cost;
        }
        else
        {
            canAfford = GameManager.Instance.tapCurrency >= cost;
        }

        buyButton.interactable = canAfford;
    }

    private void OnBuyClicked()
    {
        UpgradeManager.Instance.PurchaseUpgrade(data);
    }
}