using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorkerSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private Button rechargeButton;
    [SerializeField] private TMP_Text costText;

    private int workerIndex;

    public void Setup(int index)
    {
        workerIndex = index;
        nameText.text = $"Worker #{index + 1}";
        
        rechargeButton.onClick.RemoveAllListeners();
        rechargeButton.onClick.AddListener(OnRechargeClicked);
    }

    private void Update()
    {
        if (WorkerManager.Instance == null || workerIndex >= WorkerManager.Instance.activeWorkers.Count) return;

        WorkerInstance myWorker = WorkerManager.Instance.activeWorkers[workerIndex];
        
        chargeSlider.maxValue = WorkerManager.Instance.maxWorkerCharge;
        chargeSlider.value = myWorker.currentCharge;

        costText.text = WorkerManager.Instance.rechargeCost.ToString();

        bool needsCharge = myWorker.currentCharge < WorkerManager.Instance.maxWorkerCharge;
        bool canAfford = GameManager.Instance.holdCurrency >= WorkerManager.Instance.rechargeCost;
        
        rechargeButton.interactable = needsCharge && canAfford;
    }

    private void OnRechargeClicked()
    {
        WorkerManager.Instance.RechargeWorker(workerIndex);
    }
}