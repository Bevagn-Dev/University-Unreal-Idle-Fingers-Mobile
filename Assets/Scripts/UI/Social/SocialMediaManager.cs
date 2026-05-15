using UnityEngine;

public class SocialMediaManager : MonoBehaviour
{
    [Header("Your Social Links")]
    [SerializeField] private string twitterURL = "https://twitter.com";
    [SerializeField] private string instagramURL = "https://instagram.com";
    [SerializeField] private string facebookURL = "https://facebook.com";
    [SerializeField] private string discordURL = "https://discord.gg"; 

    public void OpenTwitter()
    {
        Application.OpenURL(twitterURL);
    }

    public void OpenInstagram()
    {
        Application.OpenURL(instagramURL);
    }

    public void OpenFacebook()
    {
        Application.OpenURL(facebookURL);
    }

    public void OpenDiscord()
    {
        Application.OpenURL(discordURL);
    }
}