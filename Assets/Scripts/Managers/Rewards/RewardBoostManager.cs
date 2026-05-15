using UnityEngine;
using System;

public class RewardBoostManager : MonoBehaviour
{
    public static RewardBoostManager Instance { get; private set; }

    [Header("Durations")]
    public float activeDuration = 60f;
    public float cooldownDuration = 120f; 

    [Header("Active Multipliers")]
    public float spawnBoostMultiplier = 1f;
    public float workerBoostMultiplier = 1f;

    // Tracking variables
    private float multiplierActiveTimer = 0f;
    private float multiplierCooldownTimer = 0f;

    private float spawnActiveTimer = 0f;
    private float spawnCooldownTimer = 0f;

    private float workerActiveTimer = 0f;
    private float workerCooldownTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        ProcessTimer(ref multiplierActiveTimer, ref multiplierCooldownTimer, OnMultiplierEnded);
        ProcessTimer(ref spawnActiveTimer, ref spawnCooldownTimer, OnSpawnEnded);
        ProcessTimer(ref workerActiveTimer, ref workerCooldownTimer, OnWorkerEnded);
    }

    private void ProcessTimer(ref float activeTimer, ref float cooldownTimer, Action onEndAction)
    {
        if (activeTimer > 0)
        {
            activeTimer -= Time.unscaledDeltaTime;
            if (activeTimer <= 0)
            {
                activeTimer = 0;
                cooldownTimer = cooldownDuration;
                onEndAction?.Invoke();
            }
        }
        else if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
            }
        }
    }

    public void ActivateBoost(BoostType type)
    {
        switch (type)
        {
            case BoostType.Multiplier:
                multiplierActiveTimer = activeDuration;
                GameManager.Instance.adBoostMultiplier = 2f;
                GameManager.Instance.CalculateTotalMultiplier();
                break;
            case BoostType.Spawn:
                spawnActiveTimer = activeDuration;
                spawnBoostMultiplier = 0.5f;
                break;
            case BoostType.Worker:
                workerActiveTimer = activeDuration;
                workerBoostMultiplier = 0.5f;
                break;
        }
    }

    private void OnMultiplierEnded()
    {
        GameManager.Instance.adBoostMultiplier = 1f;
        GameManager.Instance.CalculateTotalMultiplier();
    }

    private void OnSpawnEnded()
    {
        spawnBoostMultiplier = 1f;
    }

    private void OnWorkerEnded()
    {
        workerBoostMultiplier = 1f;
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