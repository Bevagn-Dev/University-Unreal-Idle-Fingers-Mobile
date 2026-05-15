using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Component References")]
    [SerializeField] private AudioSource musicSource;

    [Header("Playlist")]
    [SerializeField] private AudioClip[] playlist;

    private int lastPlayedIndex = -1;

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
        if (musicSource != null)
        {
            musicSource.loop = false; 
            PlayRandomSong();
        }
    }

    private void Update()
    {
        if (musicSource != null && !musicSource.isPlaying && playlist.Length > 0)
        {
            PlayRandomSong();
        }
    }

    private void PlayRandomSong()
    {
        if (playlist == null || playlist.Length == 0) return;

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