using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneButton : MonoBehaviour
{
    public void ReloadScene()
    {
         Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    
}
