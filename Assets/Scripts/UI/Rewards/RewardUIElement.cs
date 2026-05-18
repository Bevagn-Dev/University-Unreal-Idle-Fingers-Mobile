using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RewardUIElement : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private BoostType boostType;
    [SerializeField] private string defaultText = "Temp Boost";

    [Header("References")]
    [SerializeField] private Button adButton;
    [SerializeField] private TMP_Text buttonText;

    private Coroutine uiUpdateRoutine;
    private int lastRecordedSecond = -1;
    private int currentState = -1; 

    private void Awake()
    {
        adButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnEnable()
    {
        if (uiUpdateRoutine != null) StopCoroutine(uiUpdateRoutine);
        uiUpdateRoutine = StartCoroutine(UpdateUIRoutine());
    }

    private void OnDisable()
    {
        if (uiUpdateRoutine != null)
        {
            StopCoroutine(uiUpdateRoutine);
            uiUpdateRoutine = null;
        }
    }

    private IEnumerator UpdateUIRoutine()
    {
        WaitForSeconds waitDelay = new WaitForSeconds(0.2f);

        while (true)
        {
            if (RewardBoostManager.Instance != null)
            {
                bool isActive = RewardBoostManager.Instance.IsBoostActive(boostType);
                bool isCooldown = RewardBoostManager.Instance.IsBoostOnCooldown(boostType);

                if (isActive)
                {
                    float time = RewardBoostManager.Instance.GetActiveTime(boostType);
                    int currentSecond = Mathf.FloorToInt(time);
                    
                    if (currentSecond != lastRecordedSecond || currentState != 1)
                    {
                        buttonText.text = $"Active: {FormatTime(time)}";
                        adButton.interactable = false;
                        lastRecordedSecond = currentSecond;
                        currentState = 1;
                    }
                }
                else if (isCooldown)
                {
                    float time = RewardBoostManager.Instance.GetCooldownTime(boostType);
                    int currentSecond = Mathf.FloorToInt(time);

                    if (currentSecond != lastRecordedSecond || currentState != 2)
                    {
                        buttonText.text = $"Wait: {FormatTime(time)}";
                        adButton.interactable = false;
                        lastRecordedSecond = currentSecond;
                        currentState = 2;
                    }
                }
                else
                {
                    if (currentState != 0)
                    {
                        buttonText.text = defaultText;
                        adButton.interactable = true;
                        currentState = 0;
                        lastRecordedSecond = -1;
                    }
                }
            }
            
            yield return waitDelay;
        }
    }

    private void OnButtonClicked()
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowAd(boostType);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}