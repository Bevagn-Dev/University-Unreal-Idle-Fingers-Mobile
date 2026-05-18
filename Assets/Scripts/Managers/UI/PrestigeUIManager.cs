using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrestigeUIManager : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TMP_Text currentMultiplierText;
    [SerializeField] private TMP_Text potentialGainText;
    [SerializeField] private TMP_Text currentText;
    [SerializeField] private TMP_Text requirementText;

    [Header("Buttons")]
    [SerializeField] private Button prestigeButton;

    private void Awake()
    {
        prestigeButton.onClick.AddListener(OnPrestigeButtonClicked);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrenciesUpdated += UpdatePrestigeUI;
            UpdatePrestigeUI();
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCurrenciesUpdated -= UpdatePrestigeUI;
        }
    }

    private void UpdatePrestigeUI()
    {
        float currentMulti = GameManager.Instance.prestigeMultiplier;
        float potentialGain = GameManager.Instance.GetPotentialPrestigeGain();
        float combinedWealth = GameManager.Instance.holdCurrency + GameManager.Instance.tapCurrency;
        float threshold = GameManager.Instance.prestigeThreshold;

        currentMultiplierText.text = $"Current Bonus: {currentMulti:F1}x";
        currentText.text = $"Current Combined Currency: {combinedWealth}";

        if (combinedWealth >= threshold)
        {
            potentialGainText.text = $"Gain on Reset: +{potentialGain:F1}x";
            requirementText.text = "You are ready to Prestige!";
            prestigeButton.interactable = true;
        }
        else
        {
            potentialGainText.text = "Gain on Reset: +0.0x";
            requirementText.text = $"Requires {threshold} combined currency.";
            prestigeButton.interactable = false;
        }
    }

    private void OnPrestigeButtonClicked()
    {
        ConfirmationPopup.Instance.Show("Prestige", GameManager.Instance.ExecutePrestige);
    }
}