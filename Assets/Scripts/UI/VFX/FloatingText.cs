using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    public TMP_Text textComponent; 

    [Header("Animation Tweaks")]
    public float floatDistance = 0.6f; 
    public float floatDuration = 0.8f; 

    public void SetupFloat(string message, Color textColor)
    {
        textComponent.DOKill();
        transform.DOKill();
        
        Color resetColor = textColor;
        resetColor.a = 1f; 
        
        textComponent.text = message;
        textComponent.color = resetColor;
        
        transform.localScale = Vector3.one * 0.4f; 

        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

        transform.DOMoveY(transform.position.y + floatDistance, floatDuration).SetEase(Ease.OutQuint);

        float halfTime = floatDuration / 2f;
        
        textComponent.DOFade(0f, halfTime).SetDelay(halfTime).OnComplete(() => 
        {
            ObjectPoolManager.Instance.Despawn("FloatingText", gameObject);
        });
    }

    public void SetupPulse(string message, Color textColor)
    {
        textComponent.DOKill();
        transform.DOKill();
        
        Color resetColor = textColor;
        resetColor.a = 1f; 
        
        textComponent.text = message;
        textComponent.color = resetColor;
        
        transform.localScale = Vector3.one * 0.5f; 

        transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack);
        transform.DOScale(Vector3.one * 1.4f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);

        textComponent.DOFade(0f, 0.6f).SetDelay(0.6f).OnComplete(() => 
        {
            transform.DOKill();
            ObjectPoolManager.Instance.Despawn("FloatingText", gameObject);
        });
    }
}