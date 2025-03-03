using UnityEngine;
using UnityEngine.UI;
using RhythmGame.Core;

public class ScoreUI : MonoBehaviour
{
    public Text scoreText;
    private ScoreManager scoreManager;

    void Start()
    {
        // Create a new instance of ScoreManager since it's not a MonoBehaviour
        scoreManager = new ScoreManager();
        UpdateScoreDisplay();
    }

    void Update()
    {
        UpdateScoreDisplay();
    }

    public void UpdateScoreDisplay()
    {
        if (scoreManager != null)
        {
            scoreText.text = "Score: " + scoreManager.GetScore();
        }
    }

    // Method to get reference to the score manager if needed from other scripts
    public ScoreManager GetScoreManager()
    {
        return scoreManager;
    }
}