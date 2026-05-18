using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject tapButtonPrefab;
    [SerializeField] private RectTransform playAreaRect;

    [Header("Upgrade Data References")]
    [SerializeField] private UpgradeData unlockSpawnsData;
    [SerializeField] private UpgradeData unlockTimingData;
    [SerializeField] private UpgradeData spawnRateData;
    [SerializeField] private UpgradeData maxAmountData;
    [SerializeField] private UpgradeData buttonLifetimeData;
    [SerializeField] private UpgradeData baseRewardData;

    [Header("Collision Layers")]
    [SerializeField] private float safeSpawnRadius = 1.5f; 
    [SerializeField] private LayerMask buttonLayerMask;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] spawnSFX;

    private List<GameplayButton> activeButtons = new List<GameplayButton>();
    private SpriteRenderer prefabSpriteRef;
    private Camera mainCamera;

    private float minX, maxX, minY, maxY;

    private bool isSpawningUnlocked;
    private int cachedMaxButtons;
    private float cachedSpawnRate;
    private float cachedReward;
    private float cachedLifetime;
    private bool cachedTimingUnlocked;

    private Coroutine spawnCoroutine;

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
        prefabSpriteRef = tapButtonPrefab.GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        CalculateSpawnBoundaries();
        RefreshUpgradeCache();
    }

    public void RefreshUpgradeCache()
    {
        isSpawningUnlocked = UpgradeManager.Instance.GetUpgradeLevel(unlockSpawnsData.upgradeID) > 0;
        cachedMaxButtons = (int)UpgradeManager.Instance.GetUpgradeCurrentValue(maxAmountData);
        
        cachedSpawnRate = UpgradeManager.Instance.GetUpgradeCurrentValue(spawnRateData) * RewardBoostManager.Instance.spawnBoostMultiplier;
        
        cachedReward = UpgradeManager.Instance.GetUpgradeCurrentValue(baseRewardData);
        cachedLifetime = UpgradeManager.Instance.GetUpgradeCurrentValue(buttonLifetimeData);
        cachedTimingUnlocked = UpgradeManager.Instance.GetUpgradeLevel(unlockTimingData.upgradeID) > 0;

        ManageSpawnRoutine();
    }

    private void ManageSpawnRoutine()
    {
        if (isSpawningUnlocked && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
        else if (!isSpawningUnlocked && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (activeButtons.Count < cachedMaxButtons)
            {
                SpawnButton();
            }
            
            yield return new WaitForSeconds(cachedSpawnRate);
        }
    }

    private void CalculateSpawnBoundaries()
    {
        Vector3[] corners = new Vector3[4];
        playAreaRect.GetWorldCorners(corners);

        Vector3 minWorld = mainCamera.ScreenToWorldPoint(corners[0]);
        Vector3 maxWorld = mainCamera.ScreenToWorldPoint(corners[2]);

        float paddingX = prefabSpriteRef.bounds.extents.x;
        float paddingY = prefabSpriteRef.bounds.extents.y;

        minX = minWorld.x + paddingX;
        maxX = maxWorld.x - paddingX;
        minY = minWorld.y + paddingY;
        maxY = maxWorld.y - paddingY;
    }

    private void SpawnButton()
    {
        Vector3 validSpawnPos = Vector3.zero;
        bool positionFound = false;
        int maxAttempts = 15;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 testPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);

            Collider2D overlapHit = Physics2D.OverlapCircle(testPos, safeSpawnRadius, buttonLayerMask);
            
            if (overlapHit == null)
            {
                validSpawnPos = testPos;
                positionFound = true;
                break; 
            }
        }
        
        if (!positionFound) return;

        GameObject newBtn = ObjectPoolManager.Instance.Spawn("TapButton", validSpawnPos, Quaternion.identity);

        if (newBtn != null && newBtn.TryGetComponent(out GameplayButton btnScript))
        {
            SoundManager.Instance.PlayRandomUISound(spawnSFX, 0.2f);
            
            btnScript.Initialize(cachedReward, cachedLifetime, cachedTimingUnlocked);
            activeButtons.Add(btnScript);
        }
    }

    public void ReportButtonDestroyed(GameplayButton button)
    {
        activeButtons.Remove(button);
    }

    public GameplayButton GetTargetButton()
    {
        return activeButtons.Count > 0 ? activeButtons[0] : null;
    }
}