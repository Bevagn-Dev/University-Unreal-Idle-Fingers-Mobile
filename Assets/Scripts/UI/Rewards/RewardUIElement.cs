using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardUIElement : MonoBehaviour
{
    [Header("Configuration")]
    public BoostType boostType;
    public string defaultText = "Temp Boost";

    [Header("References")]
    [SerializeField] private Button adButton;
    [SerializeField] private TMP_Text buttonText;

    private void Start()
    {
        adButton.onClick.AddListener(OnButtonClicked);
    }

    private void Update()
    {
        if (RewardBoostManager.Instance == null) return;

        bool isActive = RewardBoostManager.Instance.IsBoostActive(boostType);
        bool isCooldown = RewardBoostManager.Instance.IsBoostOnCooldown(boostType);

        if (isActive)
        {
            float time = RewardBoostManager.Instance.GetActiveTime(boostType);
            buttonText.text = $"Active: {FormatTime(time)}";
            adButton.interactable = false;
        }
        else if (isCooldown)
        {
            float time = RewardBoostManager.Instance.GetCooldownTime(boostType);
            buttonText.text = $"Wait: {FormatTime(time)}";
            adButton.interactable = false;
        }
        else
        {
            buttonText.text = defaultText;
            adButton.interactable = true;
        }
    }

    private void OnButtonClicked()
    {
        AdManager.Instance.ShowAd(boostType.ToString());
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}