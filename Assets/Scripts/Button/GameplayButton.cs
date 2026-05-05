using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class GameplayButton : MonoBehaviour, IPointerDownHandler
{

    [Header("Visuals")]
    [SerializeField] private float fadeDuration = 0.2f;

    [Space(10)]

    [Header("References")]
    [SerializeField] private Transform shrinkingRing;
    [SerializeField] private SpriteRenderer ringSpriteRenderer; 

    [Header("Sounds")]
    [SerializeField] private AudioClip tapSFX;

    private SpriteRenderer spriteRenderer;
    private TMP_Text rewardText;
    private Vector3 originalScale;

    private ButtonValues data;
    private float lifeTimer;
    private float maxLifeTime;
    private bool isInteractable = true;

    public void Initialize(ButtonValues setupData)
    {
        data = setupData;
        maxLifeTime = data.lifeTimeSeconds;
        lifeTimer = maxLifeTime;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rewardText = GetComponentInChildren<TMP_Text>();
        
        originalScale = transform.localScale;

        spriteRenderer.color = data.color;
        rewardText.text = data.baseTapRewardAmount.ToString();
        
        shrinkingRing.localScale = Vector3.one * 2f; 
        shrinkingRing.DOScale(Vector3.one, maxLifeTime).SetEase(Ease.Linear);
    }

    void Update()
    {
        if (!isInteractable) 
        {
            return;
        }

        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0)
        {
            MissButton();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) 
        {
            return;
        }

        ExecuteTap();
    }

    public void ExecuteTap() 
    {
        isInteractable = false;

        SoundManager.Instance.PlayUISound(tapSFX, 1f);
        
        shrinkingRing.DOKill();
        transform.DOKill();

        float timingBonus = CalculateTimingBonus();
        float totalReward = data.baseTapRewardAmount * timingBonus;

        GameManager.Instance.AddTapCurrency(totalReward);
        GameManager.Instance.RegisterComboHit();
        

        transform.DOScale(originalScale * 0.85f, 0.1f).SetEase(Ease.OutQuad).OnComplete(FadeOutAndDestroy);
    }

    private void MissButton()
    {
        isInteractable = false;
        
        shrinkingRing.DOKill();
        transform.DOKill();

        GameManager.Instance.BreakCombo();
        
        transform.DOShakeScale(0.2f, 0.2f, 5, 45).OnComplete(FadeOutAndDestroy);
    }

    private void FadeOutAndDestroy()
    {
        spriteRenderer.DOFade(0f, fadeDuration);
        if (ringSpriteRenderer != null) ringSpriteRenderer.DOFade(0f, fadeDuration);
        
        rewardText.DOFade(0f, fadeDuration).OnComplete(() => 
        {
            SpawnManager.Instance.ReportButtonDestroyed();
            Destroy(gameObject);
        });
    }

    private float CalculateTimingBonus()
    {
        float progress = 1f - (lifeTimer / maxLifeTime);

        if (progress >= 0.85f) 
        {
            return 2.0f; // Perfect
        }
        if (progress >= 0.60f) 
        {
            return 1.2f; // Good
        }
        else
        {
            return 1.0f; // Early
        }
    }
}