using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

[System.Serializable]
public class WorkerInstance
{
    public float currentCharge;
}

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    [Header("Upgrade Data References")]
    [SerializeField] private UpgradeData workerCountData;
    [SerializeField] private UpgradeData workerSpeedData;
    [SerializeField] private UpgradeData workerEfficiencyData;
    [SerializeField] private UpgradeData workerPowerData;

    [Header("Worker Settings")]
    public float maxWorkerCharge = 50f;
    public float rechargeCost = 10f; 

    public List<WorkerInstance> activeWorkers = new List<WorkerInstance>();
    
    public IReadOnlyList<WorkerInstance> ActiveWorkers => activeWorkers;
    public float MaxWorkerCharge => maxWorkerCharge;
    public float RechargeCost => rechargeCost;

    public event Action OnWorkersChanged;

    private int cachedTargetCount;
    private float cachedWorkerSpeed;
    private float cachedDrainAmount;
    private float cachedAutoPower;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpgradeManager.Instance.OnUpgradePurchased += HandleUpgrades;
        RefreshUpgradeCache();
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
        RefreshUpgradeCache();
    }

    private void RefreshUpgradeCache()
    {
        cachedTargetCount = UpgradeManager.Instance.GetUpgradeLevel(workerCountData.upgradeID);
        cachedWorkerSpeed = UpgradeManager.Instance.GetUpgradeCurrentValue(workerSpeedData) * RewardBoostManager.Instance.workerBoostMultiplier;
        cachedDrainAmount = UpgradeManager.Instance.GetUpgradeCurrentValue(workerEfficiencyData);
        cachedAutoPower = UpgradeManager.Instance.GetUpgradeCurrentValue(workerPowerData);

        UpdateWorkerCount();
    }

    private void UpdateWorkerCount()
    {
        while (activeWorkers.Count < cachedTargetCount)
        {
            WorkerInstance newWorker = new WorkerInstance();
            newWorker.currentCharge = maxWorkerCharge;
            activeWorkers.Add(newWorker);
            
            StartCoroutine(WorkerBehavior(newWorker));
            OnWorkersChanged?.Invoke();
        }
    }

    private IEnumerator WorkerBehavior(WorkerInstance worker)
    {
        while (true)
        {
            yield return new WaitUntil(() => worker.currentCharge >= cachedDrainAmount);

            GameplayButton target = SpawnManager.Instance.GetTargetButton();
            
            if (target != null)
            {
                target.ExecuteAutoTap(cachedAutoPower);
                worker.currentCharge -= cachedDrainAmount;
                
                yield return new WaitForSeconds(cachedWorkerSpeed);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
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
}