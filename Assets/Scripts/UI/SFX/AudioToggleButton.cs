using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AudioToggleButton : MonoBehaviour
{
    public enum AudioTarget { Master, Music, SFX }

    [Header("Settings")]
    [SerializeField] private AudioTarget audioTarget;

    [Header("Visuals")]
    [SerializeField] private GameObject soundOnIcon;  
    [SerializeField] private GameObject soundOffIcon; 

    private Button myButton;
    private bool isMuted = false; 

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }

    private void Start()
    {
        myButton.onClick.AddListener(OnToggleClicked);
        UpdateVisuals();
    }

    private void OnToggleClicked()
    {
        isMuted = !isMuted;

        UpdateVisuals();

        switch (audioTarget)
        {
            case AudioTarget.Master:
                SoundMixerManager.Instance.ToggleMasterVolume(isMuted);
                break;
            case AudioTarget.Music:
                SoundMixerManager.Instance.ToggleMusicVolume(isMuted);
                break;
            case AudioTarget.SFX:
                SoundMixerManager.Instance.ToggleSFXVolume(isMuted);
                break;
        }
    }

    private void UpdateVisuals()
    {
        if (soundOnIcon != null) 
        {
            soundOnIcon.SetActive(!isMuted);
        }
        
        if (soundOffIcon != null) 
        {
            soundOffIcon.SetActive(isMuted);
        }
    }

    public void InitializeState(bool savedMuteState)
    {
        isMuted = savedMuteState;
        UpdateVisuals();
    }
}