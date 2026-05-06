using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    [Header("Toggle")]
    public bool workersEnabled = true;

    [Header("Worker Upgrades")]
    public int workerCount = 1;
    public float baseCooldown = 3.0f; 
    
    public float perfectChance = 0.05f; 
    public float goodChance = 0.25f;    
    
    public float batteryDrainPerTap = 2.0f;

    private float actionTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        if (!workersEnabled || workerCount <= 0 || GameManager.Instance.currentBattery < batteryDrainPerTap) 
        {
            return;
        }

        actionTimer -= Time.deltaTime;

        if (actionTimer <= 0)
        {
            PerformAutoTap();
            actionTimer = baseCooldown / workerCount; 
        }
    }

    private void PerformAutoTap()
    {
        GameplayButton target = SpawnManager.Instance.GetTargetButton();
        
        if (target != null)
        {
            GameManager.Instance.DrainBattery(batteryDrainPerTap);
            
            float roll = Random.value;
            float generatedBonus = 1.0f;

            if (roll <= perfectChance) 
            {
                generatedBonus = 2.0f;
            }
            else if (roll <= perfectChance + goodChance) 
            {
                generatedBonus = 1.2f;
            }

            target.ExecuteAutoTap(generatedBonus);
        }
    }

    public void ToggleWorkers(bool state)
    {
        workersEnabled = state;
    }
}