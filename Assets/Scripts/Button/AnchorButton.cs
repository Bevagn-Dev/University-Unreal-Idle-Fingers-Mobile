using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AnchorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Stats")]
    public float timeToComplete = 2f;
    public float baseRewardAmount = 10f;

    [Space(10)]

    [Header("References")]
    public SpriteRenderer progressBarSpriteRenderer;


    private SpriteRenderer spriteRenderer;
    private Material progressMaterial;
    private TMP_Text rewardText;
    private Vector3 originalScale;

    private bool isHeld = false;
    private float currentHoldTime = 0f;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        progressMaterial = progressBarSpriteRenderer.material;
        rewardText = GetComponentInChildren<TMP_Text>();

        originalScale = transform.localScale;
        rewardText.text = ($"+{baseRewardAmount}");

        SetRadialProgress(0f);
    }


    void Update()
    {
        if (isHeld)
        {
            currentHoldTime += Time.deltaTime;

            SetRadialProgress(currentHoldTime / timeToComplete);

            if (currentHoldTime >= timeToComplete)
            {
                CompleteHold();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;

        transform.DOKill();

        transform.DOScale(originalScale * 0.85f, 0.15f).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
        currentHoldTime = 0f;

        SetRadialProgress(0f);
        TriggerReleaseAnimation();
    }

    private void CompleteHold()
    {
        GameManager.Instance.AddHoldCurrency(baseRewardAmount);
        
        isHeld = false;
        currentHoldTime = 0f;

        SetRadialProgress(0f);
        TriggerReleaseAnimation();
        TeleportToNewLocation();
    }

    private void TeleportToNewLocation()
    {
        Rect safeArea = Screen.safeArea;

        Vector3 minWorld = Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMin, safeArea.yMin, 0));
        Vector3 maxWorld = Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMax, safeArea.yMax, 0));

        float paddingX = spriteRenderer.bounds.extents.x;
        float paddingY = spriteRenderer.bounds.extents.y;

        float minX = minWorld.x + paddingX;
        float maxX = maxWorld.x - paddingX;
        float minY = minWorld.y + paddingY;
        float maxY = maxWorld.y - paddingY;

        transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);
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
