using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrestigeUIManager : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TMP_Text currentMultiplierText;
    [SerializeField] private TMP_Text potentialGainText;
    [SerializeField] private TMP_Text requirementText;

    [Header("Buttons")]
    [SerializeField] private Button prestigeButton;

    private void OnEnable()
    {
        UpdatePrestigeUI();
    }

    private void Update()
    {
        UpdatePrestigeUI();
    }

    private void UpdatePrestigeUI()
    {
        if (GameManager.Instance == null) return;

        float currentMulti = GameManager.Instance.prestigeMultiplier;
        float potentialGain = GameManager.Instance.GetPotentialPrestigeGain();
        float combinedWealth = GameManager.Instance.holdCurrency + GameManager.Instance.tapCurrency;

        currentMultiplierText.text = $"Current Bonus: {currentMulti:F1}x";

        if (combinedWealth >= GameManager.Instance.prestigeThreshold)
        {
            potentialGainText.text = $"Gain on Reset: +{potentialGain:F1}x";
            requirementText.text = "You are ready to Prestige!";
            prestigeButton.interactable = true;
        }
        else
        {
            potentialGainText.text = "Gain on Reset: +0.0x";
            requirementText.text = $"Requires {GameManager.Instance.prestigeThreshold} combined currency.";
            prestigeButton.interactable = false;
        }

        prestigeButton.onClick.RemoveAllListeners();
        prestigeButton.onClick.AddListener(OnPrestigeButtonClicked);
    }

    private void OnPrestigeButtonClicked()
    {
        ConfirmationPopup.Instance.Show("Prestige", GameManager.Instance.ExecutePrestige);
    }
}