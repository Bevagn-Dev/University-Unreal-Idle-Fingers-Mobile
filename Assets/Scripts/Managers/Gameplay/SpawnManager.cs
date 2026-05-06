using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("References")]
    public GameObject tapButtonPrefab;
    public ButtonValues[] rarityPool; 

    [Header("Stats")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int maxButtonsOnScreen = 5;
    
    [SerializeField] private float safeSpawnRadius = 1.5f; 
    
    [Header("Collision Layers")]
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
        if (activeButtons.Count >= maxButtonsOnScreen) 
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnInterval)
        {
            SpawnButton();
            spawnTimer = 0f;
        }
    }

    private void SpawnButton()
    {
        Rect safeArea = Screen.safeArea;
        Vector3 minWorld = Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMin, safeArea.yMin, 0));
        Vector3 maxWorld = Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMax, safeArea.yMax, 0));

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
            ButtonValues chosenData = rarityPool[Random.Range(0, rarityPool.Length)];
            GameplayButton btnScript = newBtn.GetComponent<GameplayButton>();

            SoundManager.Instance.PlayRandomUISound(spawnSFX, 0.2f);
            
            btnScript.Initialize(chosenData);
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