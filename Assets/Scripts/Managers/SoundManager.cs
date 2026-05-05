using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("3D Sound Settings")]
    [SerializeField] private AudioSource soundFXPrefab;

    [Header("2D UI Sound Settings")]
    [Tooltip("An AudioSource component attached directly to this GameObject, with Spatial Blend set to 0 (2D)")]
    [SerializeField] private AudioSource uiAudioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    public void PlayUISound(AudioClip audioClip, float volume = 1f)
    {
        if (audioClip == null) return;
        uiAudioSource.PlayOneShot(audioClip, volume);
    }

    public void PlayRandomUISound(AudioClip[] audioClips, float volume = 1f)
    {
        if (audioClips == null || audioClips.Length == 0) return;
        int rand = Random.Range(0, audioClips.Length);
        PlayUISound(audioClips[rand], volume);
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if (audioClip == null) return;

        AudioSource audioSource = Instantiate(soundFXPrefab, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        if (audioClips == null || audioClips.Length == 0) return;
        
        int rand = Random.Range(0, audioClips.Length);
        
        PlaySoundFXClip(audioClips[rand], spawnTransform, volume);
    }
}