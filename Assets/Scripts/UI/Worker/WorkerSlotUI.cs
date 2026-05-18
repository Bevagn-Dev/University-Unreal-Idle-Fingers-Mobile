using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WorkerSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private Button rechargeButton;
    [SerializeField] private TMP_Text costText;

    private int workerIndex;
    private WorkerInstance myWorker;
    private Coroutine uiUpdateRoutine;

    private void Awake()
    {
        rechargeButton.onClick.AddListener(OnRechargeClicked);
    }

    public void Setup(int index)
    {
        workerIndex = index;
        nameText.text = $"Worker #{index + 1}";
        
        if (WorkerManager.Instance != null)
        {
            myWorker = WorkerManager.Instance.ActiveWorkers[workerIndex];
            chargeSlider.maxValue = WorkerManager.Instance.MaxWorkerCharge;
            costText.text = WorkerManager.Instance.RechargeCost.ToString();
        }

        if (uiUpdateRoutine != null) StopCoroutine(uiUpdateRoutine);
        uiUpdateRoutine = StartCoroutine(UpdateUIRoutine());
    }

    private IEnumerator UpdateUIRoutine()
    {
        WaitForSeconds waitDelay = new WaitForSeconds(0.1f);
        float lastCharge = -1f;
        float lastCurrency = -1f;

        while (true)
        {
            if (myWorker != null && GameManager.Instance != null && WorkerManager.Instance != null)
            {
                float currentCharge = myWorker.currentCharge;
                float currentCurrency = GameManager.Instance.holdCurrency;

                if (currentCharge != lastCharge || currentCurrency != lastCurrency)
                {
                    chargeSlider.value = currentCharge;

                    bool needsCharge = currentCharge < WorkerManager.Instance.MaxWorkerCharge;
                    bool canAfford = currentCurrency >= WorkerManager.Instance.RechargeCost;
                    
                    rechargeButton.interactable = needsCharge && canAfford;

                    lastCharge = currentCharge;
                    lastCurrency = currentCurrency;
                }
            }
            
            yield return waitDelay;
        }
    }

    private void OnRechargeClicked()
    {
        if (WorkerManager.Instance != null)
        {
            WorkerManager.Instance.RechargeWorker(workerIndex);
        }
    }

    private void OnDisable()
    {
        if (uiUpdateRoutine != null)
        {
            StopCoroutine(uiUpdateRoutine);
            uiUpdateRoutine = null;
        }
    }
}