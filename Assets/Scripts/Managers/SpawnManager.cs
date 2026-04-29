using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("References")]
    public GameObject tapButtonPrefab;
    public ButtonValues[] rarityPool; 

    [Header("Upgradable Stats")]
    public float spawnInterval = 1.5f;
    public int maxButtonsOnScreen = 5;

    private float spawnTimer = 0f;
    private int currentActiveButtons = 0;

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

    void Update()
    {
        if (currentActiveButtons >= maxButtonsOnScreen) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnButton();
            spawnTimer = 0f;
        }
    }

    private void SpawnButton()
    {
        ButtonValues chosenData = rarityPool[Random.Range(0, rarityPool.Length)];
        
        GameObject newBtn = Instantiate(tapButtonPrefab);
        GameplayButton btnScript = newBtn.GetComponent<GameplayButton>();
        btnScript.Initialize(chosenData);

        Rect safeArea = Screen.safeArea;
        Vector3 minWorld = Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMin, safeArea.yMin, 0));
        Vector3 maxWorld = Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMax, safeArea.yMax, 0));

        float pad = 1f; 
        
        float x = Random.Range(minWorld.x + pad, maxWorld.x - pad);
        float y = Random.Range(minWorld.y + pad, maxWorld.y - pad);
        
        newBtn.transform.position = new Vector3(x, y, 0f);
        currentActiveButtons++;
    }

    public void ReportButtonDestroyed()
    {
        currentActiveButtons--;
    }
}
