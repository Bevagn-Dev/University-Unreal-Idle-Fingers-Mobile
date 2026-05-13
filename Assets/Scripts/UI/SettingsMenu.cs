using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Mute Toggles")]    
    [SerializeField] private AudioToggleButton masterMuteToggle;
    [SerializeField] private AudioToggleButton musicMuteToggle;
    [SerializeField] private AudioToggleButton sfxMuteToggle;

    public void HardResetGame()
    {
        ConfirmationPopup.Instance.Show("Erase Save Data?", ExecuteHardReset);
    }

    private void ExecuteHardReset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable()
    {
        if (masterSlider != null)
        {
            masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 1f);
        }
        
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 1f);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);
        }

        if (masterMuteToggle != null)
        {
            masterMuteToggle.InitializeState(PlayerPrefs.GetInt("MasterMute", 0) == 1);
        }

        if (musicMuteToggle != null)
        {
            musicMuteToggle.InitializeState(PlayerPrefs.GetInt("MusicMute", 0) == 1);
        }

        if (sfxMuteToggle != null)
        {
            sfxMuteToggle.InitializeState(PlayerPrefs.GetInt("SFXMute", 0) == 1);
        }
    }
}