using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GameplayButton : MonoBehaviour, IPointerClickHandler
{
    [Header("World Space References")]
    public Transform shrinkingRing;
    public SpriteRenderer mainSprite;
    public TextMeshPro valueText;

    private ButtonValues data;
    private float lifeTimer;
    private float maxLifeTime;

    public void Initialize(ButtonValues setupData)
    {
        data = setupData;
        maxLifeTime = data.lifeTimeSeconds;
        lifeTimer = maxLifeTime;

        mainSprite.color = data.color;
        valueText.text = data.baseTapRewardAmount.ToString();
        
        shrinkingRing.localScale = Vector3.one * 2f; 
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        
        float progress = 1f - (lifeTimer / maxLifeTime); 
        shrinkingRing.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, progress);

        if (lifeTimer <= 0)
        {
            GameManager.Instance.BreakCombo();
            RemoveButton();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ExecuteTap();
    }

    public void ExecuteTap() 
    {
        float timingBonus = CalculateTimingBonus();
        float totalReward = data.baseTapRewardAmount * timingBonus;

        GameManager.Instance.AddTapCurrency(totalReward);
        GameManager.Instance.RegisterComboHit();
        
        RemoveButton();
    }

    private float CalculateTimingBonus()
    {
        float progress = 1f - (lifeTimer / maxLifeTime);
        if (progress >= 0.85f) return 2.0f; // Perfect
        if (progress >= 0.60f) return 1.2f; // Good
        return 1.0f; // Early
    }

    private void RemoveButton()
    {
        SpawnManager.Instance.ReportButtonDestroyed();
        Destroy(gameObject);
    }

}