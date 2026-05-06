using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance { get; private set; }
    [SerializeField] private AudioMixer audioMixer;
    private float savedMasterVol = 1f;
    private float savedMusicVol = 1f;
    private float savedSFXVol = 1f;


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

    public void SetMasterVolume(float level)
    {
        savedMasterVol = level;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        savedMusicVol = level;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSFXVolume(float level)
    {
        savedSFXVol = level;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
    }


    public void ToggleMasterVolume(bool isMuted)
    {
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