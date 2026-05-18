using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AnchorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Upgrades")]
    [SerializeField] private UpgradeData baseRewardData;
    [SerializeField] private UpgradeData fillSpeedData;

    [Space(10)]

    [Header("Spawn Boundaries")]
    [SerializeField] private RectTransform playAreaRect;
    [SerializeField] private float safeSpawnRadius = 1.5f; 
    [SerializeField] private LayerMask buttonLayerMask;

    [Header("References")]
    [SerializeField] private SpriteRenderer progressBarSpriteRenderer;

    [Header("Sounds")]
    [SerializeField] private AudioClip holdSFX;
    [SerializeField] private AudioClip releaseSFX;

    [Header("Floating Text Colors")]
    [SerializeField] private Color scoreColor;

    private SpriteRenderer spriteRenderer;
    private Material progressMaterial;
    private TMP_Text rewardText;
    private Camera mainCamera; 
    private Vector3 originalScale;

    private bool isHeld = false;
    private float currentHoldTime = 0f;

    private float cachedRewardValue;
    private float cachedTimeToComplete;
    private float minX, maxX, minY, maxY;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rewardText = GetComponentInChildren<TMP_Text>();
        mainCamera = Camera.main; 
    }

    private void Start()
    {
        progressMaterial = progressBarSpriteRenderer.material;
        originalScale = transform.localScale;

        SetRadialProgress(0f);
        UpdateRewardDisplay();
        CalculateBoundaries();
    }

    private void Update()
    {
        if (!isHeld) return;

        currentHoldTime += Time.deltaTime;
        SetRadialProgress(currentHoldTime / cachedTimeToComplete);

        if (currentHoldTime >= cachedTimeToComplete)
        {
            CompleteHold();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;

        cachedRewardValue = UpgradeManager.Instance.GetUpgradeCurrentValue(baseRewardData);
        cachedTimeToComplete = UpgradeManager.Instance.GetUpgradeCurrentValue(fillSpeedData);

        UpdateRewardDisplay();

        SoundManager.Instance.PlayUISound(holdSFX, 1f);

        transform.DOKill();
        transform.DOScale(originalScale * 0.85f, 0.15f).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetHoldState();

        SoundManager.Instance.PlayUISound(releaseSFX, 1f);
        TriggerReleaseAnimation();
    }

    private void CompleteHold()
    {
        float finalReward = GameManager.Instance.CalculateFinalValue(cachedRewardValue, 1f);
        GameManager.Instance.AddHoldCurrencyRaw(finalReward);
        
        GameObject popup = ObjectPoolManager.Instance.Spawn("FloatingText", transform.position, Quaternion.identity);
        if (popup != null)
        {
            FloatingText ftScript = popup.GetComponent<FloatingText>();
            if (ftScript != null)
            {
                ftScript.SetupFloat($"+{finalReward}", scoreColor);
            }
        }

        ResetHoldState();
        TriggerReleaseAnimation();
        TeleportToNewLocation();
    }

    private void ResetHoldState()
    {
        isHeld = false;
        currentHoldTime = 0f;
        SetRadialProgress(0f);
    }

    private void UpdateRewardDisplay()
    {
        float currentReward = UpgradeManager.Instance.GetUpgradeCurrentValue(baseRewardData);
        rewardText.text = $"+{currentReward}";
    }

    private void CalculateBoundaries()
    {
        Vector3[] corners = new Vector3[4];
        playAreaRect.GetWorldCorners(corners);

        Vector3 minWorld = mainCamera.ScreenToWorldPoint(corners[0]);
        Vector3 maxWorld = mainCamera.ScreenToWorldPoint(corners[2]);

        float paddingX = spriteRenderer.bounds.extents.x;
        float paddingY = spriteRenderer.bounds.extents.y;

        minX = minWorld.x + paddingX;
        maxX = maxWorld.x - paddingX;
        minY = minWorld.y + paddingY;
        maxY = maxWorld.y - paddingY;
    }

    private void TeleportToNewLocation()
    {
        Vector3 validSpawnPos = transform.position;
        bool positionFound = false;
        int maxAttempts = 15;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 testPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);

            Collider2D overlapHit = Physics2D.OverlapCircle(testPos, safeSpawnRadius, buttonLayerMask);
            
            if (overlapHit == null || overlapHit.gameObject == gameObject)
            {
                validSpawnPos = testPos;
                positionFound = true;
                break; 
            }
        }

        if (positionFound)
        {
            transform.position = validSpawnPos;
        }
    }

    private void SetRadialProgress(float progress)
    {
        float progressInDegrees = progress * 360f;

        if (progressMaterial != null)
        {
            progressMaterial.SetFloat("_Arc2", progressInDegrees);
        }
    }

    private void TriggerReleaseAnimation()
    {
        transform.DOKill(); 
        transform.localScale = originalScale; 

        transform.DOShakeScale(0.5f , 0.15f, 4, 45);
    }
}