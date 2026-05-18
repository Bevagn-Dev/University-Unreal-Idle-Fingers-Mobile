using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public static Screenshot Instance { get; private set; }
    [SerializeField] private string basePath = "";
    [SerializeField] [Range(1, 5)] private int size = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeScreenshot();
        }
    }

    public void TakeScreenshot()
    {
        string finalPath = basePath + "Screenshot_" + System.Guid.NewGuid().ToString() + ".png";
        
        ScreenCapture.CaptureScreenshot(finalPath, size);
        print("Screenshot taken: " + finalPath);
    }
}