using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("References")]
    public GameObject tapButtonPrefab;
    public ButtonValues[] rarityPool; 

    [Header("Stats")]
    public float spawnInterval = 1.5f;
    public int maxButtonsOnScreen = 5;
    
    public float safeSpawnRadius = 1.5f; 
    
    [Header("Collision Layers")]
    [Tooltip("Set this to the physics layer your Tap Buttons are on so it ignores the background/anchor.")]
    public LayerMask buttonLayerMask;

    private float spawnTimer = 0f;
    private int currentActiveButtons = 0;

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
        if (currentActiveButtons >= maxButtonsOnScreen) 
        {
            Debug.Log("Screen is full! Current count: " + currentActiveButtons);
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
            Debug.LogWarning($"Failed to find an empty spawn spot after {maxAttempts} attempts! Is your safeSpawnRadius too big?");
            return;
        }

        ButtonValues chosenData = rarityPool[Random.Range(0, rarityPool.Length)];
        
        GameObject newBtn = Instantiate(tapButtonPrefab, validSpawnPos, Quaternion.identity);
        GameplayButton btnScript = newBtn.GetComponent<GameplayButton>();
        
        btnScript.Initialize(chosenData);
        currentActiveButtons++;
    }

    public void ReportButtonDestroyed()
    {
        currentActiveButtons--;
        if (currentActiveButtons < 0) currentActiveButtons = 0; 
    }
}