using UnityEngine;
using UnityEngine.UI;

public class SimpleScoreWin : MonoBehaviour
{
    public Text scoreText;   // Assign in Inspector
    public GameObject win;     // Assign in Inspector
    public int winScore = 10;

    private int score = 0;
    private bool hasWon = false;

    void Start()
    {
        win.gameObject.SetActive(false); // hide win text at start
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        if (hasWon) return; // stop counting if already won

        score += amount;
        UpdateScoreText();

        if (score >= winScore)
        {
            WinGame();
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void WinGame()
    {
        hasWon = true;
        win.gameObject.SetActive(true);
         Time.timeScale = 0f;
        Debug.Log("You Win!");
    }
}
