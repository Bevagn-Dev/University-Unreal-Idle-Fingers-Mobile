using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class GameplayButton : MonoBehaviour, IPointerDownHandler
{
    [Header("Visuals")]
    [SerializeField] private float fadeDuration = 0.2f;

    [Space(10)]

    [Header("Floating Text Colors")]
    [SerializeField] private Color perfectColor = Color.yellow;
    [SerializeField] private Color goodColor = Color.green;
    [SerializeField] private Color earlyColor = Color.white;
    [SerializeField] private Color autoColor = new Color(0.7f, 0.2f, 0.8f);

    [Space(10)]

    [Header("References")]
    [SerializeField] private Transform shrinkingRing;
    [SerializeField] private SpriteRenderer ringSpriteRenderer; 

    [Header("Sounds")]
    [SerializeField] private AudioClip tapSFX;

    private SpriteRenderer spriteRenderer;
    private TMP_Text rewardText;
    private Vector3 originalScale;

    private float baseRewardAmount;
    private float lifeTimer;
    private float maxLifeTime;
    private bool isInteractable = true;
    private bool timingUnlocked = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Initialize(float reward, float lifetime, bool hasTimingUnlocked)
    {
        baseRewardAmount = reward;
        maxLifeTime = lifetime;
        lifeTimer = maxLifeTime;
        timingUnlocked = hasTimingUnlocked;
        
        isInteractable = true;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (rewardText == null) rewardText = GetComponentInChildren<TMP_Text>();
        
        transform.localScale = originalScale;

        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 1f;
        spriteRenderer.color = spriteColor;
        
        if (timingUnlocked)
        {
            shrinkingRing.gameObject.SetActive(true);

            if (ringSpriteRenderer != null) 
            {
                Color ringColor = ringSpriteRenderer.color;
                ringColor.a = 1f;
                ringSpriteRenderer.color = ringColor;
            }

            shrinkingRing.DOKill();
            shrinkingRing.localScale = Vector3.one * 2f; 
            shrinkingRing.DOScale(Vector3.one, maxLifeTime).SetEase(Ease.Linear);
        }
        else
        {
            shrinkingRing.gameObject.SetActive(false);
        }

        Color txtColor = rewardText.color;
        txtColor.a = 1f;
        rewardText.color = txtColor;

        rewardText.text = baseRewardAmount.ToString();
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
        
        string qualityText = "";
        Color textColor = earlyColor;

        if (timingUnlocked)
        {
            if (timingBonus >= 2.0f) 
            {
                qualityText = "Perfect!";
                textColor = perfectColor;
            }
            else if (timingBonus >= 1.2f) 
            {
                qualityText = "Good!";
                textColor = goodColor;
            }
        }

        float finalReward = GameManager.Instance.CalculateFinalValue(baseRewardAmount, timingBonus);
        GameManager.Instance.AddTapCurrencyRaw(finalReward);

        GameObject rewardPopup = ObjectPoolManager.Instance.Spawn("FloatingText", transform.position, Quaternion.identity);
        if (rewardPopup != null)
        {
            FloatingText ftScript = rewardPopup.GetComponent<FloatingText>();
            if (ftScript != null)
            {
                ftScript.SetupFloat($"+{finalReward}", textColor);
            }
        }

        if (timingUnlocked && timingBonus >= 1.2f)
        {
            GameObject qualityPopup = ObjectPoolManager.Instance.Spawn("FloatingText", transform.position, Quaternion.identity);
            if (qualityPopup != null)
            {
                FloatingText ftScript = qualityPopup.GetComponent<FloatingText>();
                if (ftScript != null)
                {
                    ftScript.SetupPulse(qualityText, textColor);
                }
            }
        }

        transform.DOScale(originalScale * 0.85f, 0.1f).SetEase(Ease.OutQuad).OnComplete(FadeOutAndDestroy);
    }

    public void ExecuteAutoTap(float workerBonus)
    {
        if (!isInteractable) 
        {
            return;
        }
        
        isInteractable = false;

        SoundManager.Instance.PlayUISound(tapSFX, 0.7f);
        
        shrinkingRing.DOKill();
        transform.DOKill();

        float finalReward = GameManager.Instance.CalculateFinalValue(baseRewardAmount, workerBonus);
        GameManager.Instance.AddTapCurrencyRaw(finalReward);

        ObjectPoolManager.Instance.Spawn("RedDot", transform.position, Quaternion.identity);

        GameObject rewardPopup = ObjectPoolManager.Instance.Spawn("FloatingText", transform.position, Quaternion.identity);
        if (rewardPopup != null)
        {
            FloatingText ftScript = rewardPopup.GetComponent<FloatingText>();
            if (ftScript != null)
            {
                ftScript.SetupFloat($"Auto! +{finalReward}", autoColor);
            }
        }

        transform.DOScale(originalScale * 0.85f, 0.1f).SetEase(Ease.OutQuad).OnComplete(FadeOutAndDestroy);
    }

    private void MissButton()
    {
        isInteractable = false;
        
        shrinkingRing.DOKill();
        transform.DOKill();
        
        transform.DOShakeScale(0.2f, 0.2f, 5, 45).OnComplete(FadeOutAndDestroy);
    }

    private void FadeOutAndDestroy()
    {
        spriteRenderer.DOFade(0f, fadeDuration);
        
        if (timingUnlocked && ringSpriteRenderer != null) 
        {
            ringSpriteRenderer.DOFade(0f, fadeDuration);
        }
        
        rewardText.DOFade(0f, fadeDuration).OnComplete(() => 
        {
            SpawnManager.Instance.ReportButtonDestroyed(this);
            ObjectPoolManager.Instance.Despawn("TapButton", gameObject);
        });
    }

    private float CalculateTimingBonus()
    {
        if (!timingUnlocked)
        {
            return 1.0f;
        }

        float progress = 1f - (lifeTimer / maxLifeTime);

        if (progress >= 0.85f) 
        {
            return UpgradeManager.Instance.GetUpgradeLevel("tap_perfect_mult") > 0 ? 2.0f : 1.5f;
        }
        if (progress >= 0.60f) 
        {
            return 1.2f;
        }
        else
        {
            return 1.0f;
        }
    }
}