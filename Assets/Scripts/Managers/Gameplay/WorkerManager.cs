using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class WorkerInstance
{
    public float currentCharge = 100f;
    public float actionTimer = 0f;
}

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    public List<WorkerInstance> activeWorkers = new List<WorkerInstance>();
    public float maxWorkerCharge = 100f;
    public float rechargeCost = 10f; 

    public event Action OnWorkersChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        UpgradeManager.Instance.OnUpgradePurchased += HandleUpgrades;
        UpdateWorkerCount();
    }

    private void OnDestroy()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnUpgradePurchased -= HandleUpgrades;
        }
    }

    private void HandleUpgrades(UpgradeData data)
    {
        if (data.upgradeID == "worker_count")
        {
            UpdateWorkerCount();
        }
    }

    private void UpdateWorkerCount()
    {
        int targetCount = UpgradeManager.Instance.GetUpgradeLevel("worker_count");

        while (activeWorkers.Count < targetCount)
        {
            activeWorkers.Add(new WorkerInstance());
            OnWorkersChanged?.Invoke();
        }
    }

    private void Update()
    {
        if (activeWorkers.Count == 0) return;

        float workerSpeed = UpgradeManager.Instance.GetUpgradeCurrentValue(GetUpgradeData("worker_speed", 3.0f)) * RewardBoostManager.Instance.workerBoostMultiplier;
        float drainAmount = UpgradeManager.Instance.GetUpgradeCurrentValue(GetUpgradeData("worker_efficiency", 5.0f));
        float autoPower = UpgradeManager.Instance.GetUpgradeCurrentValue(GetUpgradeData("worker_power_mult", 1.0f));

        foreach (var worker in activeWorkers)
        {
            if (worker.currentCharge < drainAmount) continue;

            worker.actionTimer -= Time.deltaTime;

            if (worker.actionTimer <= 0)
            {
                GameplayButton target = SpawnManager.Instance.GetTargetButton();
                
                if (target != null)
                {
                    target.ExecuteAutoTap(autoPower);
                    worker.currentCharge -= drainAmount;
                    worker.actionTimer = workerSpeed;
                }
            }
        }
    }

    public void RechargeWorker(int index)
    {
        if (index >= 0 && index < activeWorkers.Count)
        {
            if (GameManager.Instance.holdCurrency >= rechargeCost)
            {
                GameManager.Instance.SpendHoldCurrencyRaw(rechargeCost);
                activeWorkers[index].currentCharge = maxWorkerCharge;
            }
        }
    }

    private UpgradeData GetUpgradeData(string id, float fallback)
    {
        UpgradeData dummy = ScriptableObject.CreateInstance<UpgradeData>();
        dummy.upgradeID = id;
        dummy.baseValue = fallback;
        dummy.valueIncrement = 0;
        return dummy;
    }
}