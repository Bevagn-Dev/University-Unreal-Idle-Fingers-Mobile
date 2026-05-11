using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Elements")]
    public TMP_Text comboText;
    public TMP_Text tapText;        
    public TMP_Text holdText;

    [Space(10)]

    [Header("Money")]
    public float holdCurrency = 0f;
    public float tapCurrency = 0f;

    [Space(10)]

    [Header("Battery System")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;

    [Space(10)]

    [Header("Combos/Multipliers")]
    public float baseUpgradeMultiplier = 1f;
    public float prestigeMultiplier = 1f;
    public float adBoostMultiplier = 1f;

    public event Action OnCurrenciesUpdated;
    public event Action OnBatteryUpdated;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        currentBattery = maxBattery;
        CalculateTotalMultiplier();
    }

    public float CalculateTotalMultiplier()
    {
        float comboAmount = baseUpgradeMultiplier * prestigeMultiplier * adBoostMultiplier;
        
        if (comboText != null)
        {
            comboText.text = ($"{comboAmount}x");
        }

        return comboAmount;
    }

    public float CalculateFinalValue(float baseAmount, float localBonus = 1f)
    {
        float calculatedAmount = baseAmount * localBonus * CalculateTotalMultiplier();
        return Mathf.Round(calculatedAmount * 10f) / 10f; 
    }

    public void AddHoldCurrencyRaw(float finalAmount)
    {
        holdCurrency += finalAmount;
        OnCurrenciesUpdated?.Invoke();
        
        if (holdText != null)
        {
            holdText.text = holdCurrency.ToString();
        }
    }

    public void AddTapCurrencyRaw(float finalAmount)
    {
        tapCurrency += finalAmount;
        OnCurrenciesUpdated?.Invoke();
        
        if (tapText != null)
        {
            tapText.text = tapCurrency.ToString();
        }
    }

    public void RechargeBattery(float amount)
    {
        currentBattery += amount;

        if (currentBattery > maxBattery)
        {
            currentBattery = maxBattery;
        }

        OnBatteryUpdated?.Invoke();
    }

    public void DrainBattery(float amount)
    {
        currentBattery -= amount;

        if (currentBattery < 0)
        {
            currentBattery = 0;
        }

        OnBatteryUpdated?.Invoke();
    }

    public bool CanAfford(float amount, bool useHoldCurrency)
    {
        if (useHoldCurrency)
        {
            return holdCurrency >= amount;
        }
        else
        {
            return tapCurrency >= amount;
        }
    }

    public void SpendHoldCurrencyRaw(float amount)
    {
        holdCurrency -= amount;
        if (holdCurrency < 0) holdCurrency = 0;
        
        OnCurrenciesUpdated?.Invoke();
        
        if (holdText != null)
        {
            holdText.text = holdCurrency.ToString();
        }
    }

    public void SpendTapCurrencyRaw(float amount)
    {
        tapCurrency -= amount;
        if (tapCurrency < 0) tapCurrency = 0;
        
        OnCurrenciesUpdated?.Invoke();
        
        if (tapText != null)
        {
            tapText.text = tapCurrency.ToString();
        }
    }

}