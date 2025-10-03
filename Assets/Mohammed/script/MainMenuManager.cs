using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public Button playButton;
    public Button soundButton;
    public Button closeSettingsButton;  // NEW: button to close settings
    public GameObject settingsCanvas;   // Reference to your settings canvas

    private void Start()
    {
        // Hide settings menu at start
        if (settingsCanvas != null)
            settingsCanvas.SetActive(false);

        // Add button listeners
        playButton.onClick.AddListener(PlayGame);
        soundButton.onClick.AddListener(OpenSettings);

        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);
    }

    private void PlayGame()
    {
        // Change to your gameplay scene
        SceneManager.LoadScene("SampleScene");  
    }

    private void OpenSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.SetActive(true);
    }

    private void CloseSettings()
    {
        if (settingsCanvas != null)
            settingsCanvas.SetActive(false);
    }
}
