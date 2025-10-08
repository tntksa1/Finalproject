using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button playButton;



    private void Start()
    {
        playButton.onClick.AddListener(PlayGame);
    }

    private void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }


   
}
