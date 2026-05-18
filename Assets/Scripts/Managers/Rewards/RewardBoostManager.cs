using UnityEngine;
using System;
using System.Collections;

public class RewardBoostManager : MonoBehaviour
{
    public static RewardBoostManager Instance { get; private set; }

    [Header("Durations")]
    [SerializeField] private float activeDuration = 60f;
    [SerializeField] private float cooldownDuration = 120f; 

    public float spawnBoostMultiplier { get; private set; } = 1f;
    public float workerBoostMultiplier { get; private set; } = 1f;

    private float multiplierActiveTimer = 0f;
    private float multiplierCooldownTimer = 0f;

    private float spawnActiveTimer = 0f;
    private float spawnCooldownTimer = 0f;

    private float workerActiveTimer = 0f;
    private float workerCooldownTimer = 0f;

    private Coroutine multiplierRoutine;
    private Coroutine spawnRoutine;
    private Coroutine workerRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ActivateBoost(BoostType type)
    {
        switch (type)
        {
            case BoostType.Multiplier:
                if (multiplierRoutine != null) StopCoroutine(multiplierRoutine);
                multiplierRoutine = StartCoroutine(MultiplierBoostRoutine());
                break;
            case BoostType.Spawn:
                if (spawnRoutine != null) StopCoroutine(spawnRoutine);
                spawnRoutine = StartCoroutine(SpawnBoostRoutine());
                break;
            case BoostType.Worker:
                if (workerRoutine != null) StopCoroutine(workerRoutine);
                workerRoutine = StartCoroutine(WorkerBoostRoutine());
                break;
        }
    }

    private IEnumerator MultiplierBoostRoutine()
    {
        multiplierActiveTimer = activeDuration;
        multiplierCooldownTimer = 0f;
        GameManager.Instance.adBoostMultiplier = 2f;
        GameManager.Instance.CalculateTotalMultiplier();

        while (multiplierActiveTimer > 0)
        {
            multiplierActiveTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        
        multiplierActiveTimer = 0f;
        GameManager.Instance.adBoostMultiplier = 1f;
        GameManager.Instance.CalculateTotalMultiplier();

        multiplierCooldownTimer = cooldownDuration;
        
        while (multiplierCooldownTimer > 0)
        {
            multiplierCooldownTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        
        multiplierCooldownTimer = 0f;
    }

    private IEnumerator SpawnBoostRoutine()
    {
        spawnActiveTimer = activeDuration;
        spawnCooldownTimer = 0f;
        spawnBoostMultiplier = 0.5f;

        while (spawnActiveTimer > 0)
        {
            spawnActiveTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        
        spawnActiveTimer = 0f;
        spawnBoostMultiplier = 1f;

        spawnCooldownTimer = cooldownDuration;
        
        while (spawnCooldownTimer > 0)
        {
            spawnCooldownTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        
        spawnCooldownTimer = 0f;
    }

    private IEnumerator WorkerBoostRoutine()
    {
        workerActiveTimer = activeDuration;
        workerCooldownTimer = 0f;
        workerBoostMultiplier = 0.5f;

        while (workerActiveTimer > 0)
        {
            workerActiveTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        
        workerActiveTimer = 0f;
        workerBoostMultiplier = 1f;

        workerCooldownTimer = cooldownDuration;
        
        while (workerCooldownTimer > 0)
        {
            workerCooldownTimer -= Time.unscaledDeltaTime;
            yield return null;
        }
        
        workerCooldownTimer = 0f;
    }

    public float GetActiveTime(BoostType type) => type switch
    {
        BoostType.Multiplier => multiplierActiveTimer,
        BoostType.Spawn => spawnActiveTimer,
        BoostType.Worker => workerActiveTimer,
        _ => 0f
    };

    public float GetCooldownTime(BoostType type) => type switch
    {
        BoostType.Multiplier => multiplierCooldownTimer,
        BoostType.Spawn => spawnCooldownTimer,
        BoostType.Worker => workerCooldownTimer,
        _ => 0f
    };

    public bool IsBoostActive(BoostType type) => GetActiveTime(type) > 0;
    
    public bool IsBoostOnCooldown(BoostType type) => GetCooldownTime(type) > 0;
}