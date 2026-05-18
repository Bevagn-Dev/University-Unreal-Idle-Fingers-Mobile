using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("3D Sound Settings")]
    [SerializeField] private AudioSource soundFXPrefab;
    [SerializeField] private int sfxPoolSize = 10;

    [Header("2D UI Sound Settings")]
    [SerializeField] private AudioSource uiAudioSource;

    private Queue<AudioSource> sfxPool;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePool();
    }

    private void InitializePool()
    {
        sfxPool = new Queue<AudioSource>();
        
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource audioSource = Instantiate(soundFXPrefab, transform);
            audioSource.gameObject.SetActive(false);
            sfxPool.Enqueue(audioSource);
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

        AudioSource audioSource;

        if (sfxPool.Count > 0)
        {
            audioSource = sfxPool.Dequeue();
        }
        else
        {
            audioSource = Instantiate(soundFXPrefab, transform);
        }
        
        audioSource.transform.position = spawnTransform.position;
        audioSource.gameObject.SetActive(true);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        StartCoroutine(ReturnToPool(audioSource, audioClip.length));
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        if (audioClips == null || audioClips.Length == 0) return;
        
        int rand = Random.Range(0, audioClips.Length);
        PlaySoundFXClip(audioClips[rand], spawnTransform, volume);
    }

    private IEnumerator ReturnToPool(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
        audioSource.gameObject.SetActive(false);
        sfxPool.Enqueue(audioSource);
    }
}