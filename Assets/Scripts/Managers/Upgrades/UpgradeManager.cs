using System;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public event Action<UpgradeData> OnUpgradePurchased;

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

    public int GetUpgradeLevel(string upgradeID)
    {
        return PlayerPrefs.GetInt(upgradeID, 0);
    }

    public float GetUpgradeCurrentValue(UpgradeData data)
    {
        int level = GetUpgradeLevel(data.upgradeID);
        return data.baseValue + (data.valueIncrement * level);
    }

    public float GetUpgradeCost(UpgradeData data)
    {
        int level = GetUpgradeLevel(data.upgradeID);
        float cost = data.baseCost * Mathf.Pow(data.costMultiplier, level);
        return Mathf.Round(cost * 10f) / 10f;
    }

    public void PurchaseUpgrade(UpgradeData data)
    {
        int currentLevel = GetUpgradeLevel(data.upgradeID);

        if (currentLevel >= data.maxLevel)
        {
            return;
        }

        float cost = GetUpgradeCost(data);

        if (data.currencyType == CurrencyType.Hold)
        {
            if (GameManager.Instance.holdCurrency >= cost)
            {
                GameManager.Instance.SpendHoldCurrencyRaw(cost);
                CompletePurchase(data, currentLevel);
            }
        }
        else
        {
            if (GameManager.Instance.tapCurrency >= cost)
            {
                GameManager.Instance.SpendTapCurrencyRaw(cost);
                CompletePurchase(data, currentLevel);
            }
        }
    }

    private void CompletePurchase(UpgradeData data, int currentLevel)
    {
        currentLevel++;
        PlayerPrefs.SetInt(data.upgradeID, currentLevel);
        PlayerPrefs.Save();

        OnUpgradePurchased?.Invoke(data);
    }
}