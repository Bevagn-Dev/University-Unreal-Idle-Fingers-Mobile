using UnityEngine;
using UnityEngine.UI; 

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip clickSound;
    
    private Button myButton;

    private void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(PlayMySound);
    }

    private void PlayMySound()
    {

        SoundManager.Instance.PlayUISound(clickSound, 0.1f);
        
    }
}
