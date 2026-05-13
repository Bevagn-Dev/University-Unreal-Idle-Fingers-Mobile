using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance { get; private set; }
    
    [SerializeField] private AudioMixer audioMixer;
    
    private float savedMasterVol = 1f;
    private float savedMusicVol = 1f;
    private float savedSFXVol = 1f;

    private bool isMasterMuted = false;
    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
        }
    }

    private void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        savedMasterVol = PlayerPrefs.GetFloat("MasterVol", 1f);
        savedMusicVol = PlayerPrefs.GetFloat("MusicVol", 1f);
        savedSFXVol = PlayerPrefs.GetFloat("SFXVol", 1f);

        isMasterMuted = PlayerPrefs.GetInt("MasterMute", 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt("MusicMute", 0) == 1;
        isSFXMuted = PlayerPrefs.GetInt("SFXMute", 0) == 1;

        ToggleMasterVolume(isMasterMuted);
        ToggleMusicVolume(isMusicMuted);
        ToggleSFXVolume(isSFXMuted);
    }

    public void SetMasterVolume(float level)
    {
        savedMasterVol = level;
        PlayerPrefs.SetFloat("MasterVol", level);
        
        if (!isMasterMuted)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
        }
    }

    public void SetMusicVolume(float level)
    {
        savedMusicVol = level;
        PlayerPrefs.SetFloat("MusicVol", level);
        
        if (!isMusicMuted)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
        }
    }

    public void SetSFXVolume(float level)
    {
        savedSFXVol = level;
        PlayerPrefs.SetFloat("SFXVol", level);
        
        if (!isSFXMuted)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
        }
    }

    public void ToggleMasterVolume(bool isMuted)
    {
        isMasterMuted = isMuted;
        PlayerPrefs.SetInt("MasterMute", isMuted ? 1 : 0);
        
        if (isMuted) 
        {
            audioMixer.SetFloat("MasterVolume", -80f);
        }
        else
        {
            SetMasterVolume(savedMasterVol);
        }
    }

    public void ToggleMusicVolume(bool isMuted)
    {
        isMusicMuted = isMuted;
        PlayerPrefs.SetInt("MusicMute", isMuted ? 1 : 0);
        
        if (isMuted) 
        {
            audioMixer.SetFloat("MusicVolume", -80f);
        }
        else 
        {
            SetMusicVolume(savedMusicVol);
        }
    }

    public void ToggleSFXVolume(bool isMuted)
    {
        isSFXMuted = isMuted;
        PlayerPrefs.SetInt("SFXMute", isMuted ? 1 : 0);
        
        if (isMuted) 
        {
            audioMixer.SetFloat("SFXVolume", -80f);
        }
        else 
        {
            SetSFXVolume(savedSFXVol);
        }
    }
}