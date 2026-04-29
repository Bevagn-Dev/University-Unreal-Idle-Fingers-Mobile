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

    [Header("Combos/Multipliers")]
    public float baseUpgradeMultiplier = 1f;
    public float comboMultiplier = 1f;
    public float adBoostMultiplier = 1f;

    public event Action OnCurrenciesUpdated;

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

        CalculateTotalMultiplier();
    }

    public float CalculateTotalMultiplier()
    {
        float comboAmount = baseUpgradeMultiplier * comboMultiplier * adBoostMultiplier;
        comboText.text = ($"{comboAmount}x");

        return comboAmount;
    }

    public void AddHoldCurrency(float baseAmount)
    {
        holdCurrency += (baseAmount * CalculateTotalMultiplier());
        OnCurrenciesUpdated?.Invoke();
        holdText.text = holdCurrency.ToString();
    }

    public void AddTapCurrency(float baseAmount)
    {
        tapCurrency += (baseAmount * CalculateTotalMultiplier());
        OnCurrenciesUpdated?.Invoke();
        tapText.text = tapCurrency.ToString();
    }

    public void RegisterComboHit()
    {
        comboMultiplier = Mathf.Min(comboMultiplier + 0.1f, 3f); 
    }

    public void BreakCombo()
    {
        comboMultiplier = 1f;
    }
}

