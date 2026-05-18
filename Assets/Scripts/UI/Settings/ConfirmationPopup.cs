using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening; 

public class ConfirmationPopup : MonoBehaviour
{
    public static ConfirmationPopup Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup popupCanvasGroup; 
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.2f;

    private UnityAction currentConfirmAction;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        popupCanvasGroup.alpha = 0f;
        popupCanvasGroup.gameObject.SetActive(false);

        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(Hide);
    }

    public void Show(string title, UnityAction confirmAction)
    {
        titleText.text = title;
        currentConfirmAction = confirmAction;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.bottomUI.SetActive(false);
            UIManager.Instance.topUI.SetActive(false);
        }

        popupCanvasGroup.gameObject.SetActive(true);
        popupCanvasGroup.DOKill();
        popupCanvasGroup.DOFade(1f, fadeDuration).SetUpdate(true); 
    }

    private void OnConfirmClicked()
    {
        currentConfirmAction?.Invoke();
        Hide();
    }

    public void Hide()
    {
        popupCanvasGroup.DOKill();
        popupCanvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() => 
        {
            popupCanvasGroup.gameObject.SetActive(false);
            
            if (UIManager.Instance != null && 
                !UIManager.Instance.settingsPanel.activeSelf && 
                !UIManager.Instance.upgradesPanel.activeSelf && 
                !UIManager.Instance.prestigePanel.activeSelf && 
                !UIManager.Instance.rewardsPanel.activeSelf)
            {
                UIManager.Instance.bottomUI.SetActive(true);
                UIManager.Instance.topUI.SetActive(true);
            }
        });
    }
}