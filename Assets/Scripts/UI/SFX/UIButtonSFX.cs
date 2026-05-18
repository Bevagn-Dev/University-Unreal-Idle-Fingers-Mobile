using UnityEngine;
using UnityEngine.UI; 

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip clickSound;
    
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }

    private void Start()
    {
        myButton.onClick.AddListener(PlayMySound);
    }

    private void PlayMySound()
    {
        if (clickSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUISound(clickSound, 0.1f);
        }
    }
}