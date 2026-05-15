using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("References")]
    public GameObject tapButtonPrefab;
    public RectTransform playAreaRect;

    [Header("Upgrade Data References")]
    public UpgradeData unlockSpawnsData;
    public UpgradeData unlockTimingData;
    public UpgradeData spawnRateData;
    public UpgradeData maxAmountData;
    public UpgradeData buttonLifetimeData;
    public UpgradeData baseRewardData;

    [Header("Collision Layers")]
    [SerializeField] private float safeSpawnRadius = 1.5f; 
    [SerializeField] private LayerMask buttonLayerMask;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] spawnSFX;

    private float spawnTimer = 0f;
    private List<GameplayButton> activeButtons = new List<GameplayButton>();
    private SpriteRenderer prefabSpriteRef;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
        }
    }

    private void Start()
    {
        prefabSpriteRef = tapButtonPrefab.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (UpgradeManager.Instance.GetUpgradeLevel(unlockSpawnsData.upgradeID) == 0)
        {
            return;
        }

        int currentMaxButtons = (int)UpgradeManager.Instance.GetUpgradeCurrentValue(maxAmountData);

        if (activeButtons.Count >= currentMaxButtons) 
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        
        float currentSpawnRate = UpgradeManager.Instance.GetUpgradeCurrentValue(spawnRateData) * RewardBoostManager.Instance.spawnBoostMultiplier;  
        
        if (spawnTimer >= currentSpawnRate)
        {
            SpawnButton();
            spawnTimer = 0f;
        }
    }

    private void SpawnButton()
    {
        Vector3[] corners = new Vector3[4];
        playAreaRect.GetWorldCorners(corners);

        Vector3 minWorld = Camera.main.ScreenToWorldPoint(corners[0]);
        Vector3 maxWorld = Camera.main.ScreenToWorldPoint(corners[2]);

        float paddingX = prefabSpriteRef.bounds.extents.x;
        float paddingY = prefabSpriteRef.bounds.extents.y;

        float minX = minWorld.x + paddingX;
        float maxX = maxWorld.x - paddingX;
        float minY = minWorld.y + paddingY;
        float maxY = maxWorld.y - paddingY;

        Vector3 validSpawnPos = Vector3.zero;
        bool positionFound = false;
        int maxAttempts = 15;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector3 testPos = new Vector3(randomX, randomY, 0f);

            Collider2D overlapHit = Physics2D.OverlapCircle(testPos, safeSpawnRadius, buttonLayerMask);
            
            if (overlapHit == null)
            {
                validSpawnPos = testPos;
                positionFound = true;
                break; 
            }
        }
        
        if (!positionFound) 
        {
            return;
        }

        GameObject newBtn = ObjectPoolManager.Instance.Spawn("TapButton", validSpawnPos, Quaternion.identity);

        if(newBtn != null)
        {
            GameplayButton btnScript = newBtn.GetComponent<GameplayButton>();

            SoundManager.Instance.PlayRandomUISound(spawnSFX, 0.2f);
            
            float reward = UpgradeManager.Instance.GetUpgradeCurrentValue(baseRewardData);
            float lifetime = UpgradeManager.Instance.GetUpgradeCurrentValue(buttonLifetimeData);
            bool timingUnlocked = UpgradeManager.Instance.GetUpgradeLevel(unlockTimingData.upgradeID) > 0;
            
            btnScript.Initialize(reward, lifetime, timingUnlocked);
            activeButtons.Add(btnScript);
        }
    }

    public void ReportButtonDestroyed(GameplayButton button)
    {
        if (activeButtons.Contains(button))
        {
            activeButtons.Remove(button);
        }
    }

    public GameplayButton GetTargetButton()
    {
        if (activeButtons.Count > 0)
        {
            return activeButtons[0];
        }
        return null;
    }
}