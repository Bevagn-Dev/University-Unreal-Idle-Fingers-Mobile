using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AudioToggleButton : MonoBehaviour
{
    [Header("Settings")]
    public AudioTarget audioTarget;
    public enum AudioTarget { Master, Music, SFX }

    [Header("Visuals")]
    public GameObject soundOnIcon;  
    public GameObject soundOffIcon; 

    private Button myButton;
    private bool isMuted = false; 

    private void Start()
    {
        myButton = GetComponent<Button>();
        
        myButton.onClick.AddListener(OnToggleClicked);

        UpdateVisuals();
    }

    private void OnToggleClicked()
    {
        isMuted = !isMuted;

        UpdateVisuals();

        if (audioTarget == AudioTarget.Master)
        {
            SoundMixerManager.Instance.ToggleMasterVolume(isMuted);
        }
        else if (audioTarget == AudioTarget.Music)
        {
            SoundMixerManager.Instance.ToggleMusicVolume(isMuted);
        }
        else if (audioTarget == AudioTarget.SFX)
        {
            SoundMixerManager.Instance.ToggleSFXVolume(isMuted);
        }
        
        // SoundManager.Instance.PlayUISound(clickSound); 
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
}