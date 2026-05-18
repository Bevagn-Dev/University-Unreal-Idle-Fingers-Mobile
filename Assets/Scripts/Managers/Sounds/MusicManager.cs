using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Component References")]
    [SerializeField] private AudioSource musicSource;

    [Header("Playlist")]
    [SerializeField] private AudioClip[] playlist;

    private int lastPlayedIndex = -1;
    private Coroutine playlistRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (musicSource != null && playlist != null && playlist.Length > 0)
        {
            musicSource.loop = false; 
            playlistRoutine = StartCoroutine(PlaylistRoutine());
        }
    }

    private IEnumerator PlaylistRoutine()
    {
        while (true)
        {
            PlayRandomSong();
            yield return new WaitForSecondsRealtime(musicSource.clip.length);
        }
    }

    private void PlayRandomSong()
    {
        int randomIndex = Random.Range(0, playlist.Length);

        if (playlist.Length > 1)
        {
            while (randomIndex == lastPlayedIndex)
            {
                randomIndex = Random.Range(0, playlist.Length);
            }
        }

        lastPlayedIndex = randomIndex;
        musicSource.clip = playlist[randomIndex];
        musicSource.Play();
    }
}