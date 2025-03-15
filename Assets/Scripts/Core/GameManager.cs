using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isGameActive;
    private int currentWave;

    void Start()
    {
        isGameActive = false;
        currentWave = 0;
    }

    public void StartGame()
    {
        isGameActive = true;
        currentWave = 1;
        FMODManager.Instance.StartMusic();
        // Additional logic to initialize the game
    }

    public void EndGame()
    {
        isGameActive = false;
        // Additional logic to handle game over
    }

    public void UpdateWave()
    {
        if (isGameActive)
        {
            currentWave++;
            // Logic to handle wave progression
        }
    }
}