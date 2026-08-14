using UnityEngine;
using TMPro;

public class SnakeGameManager : MonoBehaviour
{
    [Header("Snake Movement Scripts")]
    public SnakeMovement player1Movement;
    public SnakeMovement player2Movement;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text winnerText;

    private bool gameHasEnded;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void GameOver(int losingPlayer)
    {
        if (gameHasEnded)
            return;

        gameHasEnded = true;

        Debug.Log("Game over received. Losing player: " + losingPlayer);

        player1Movement.enabled = false;
        player2Movement.enabled = false;

        gameOverPanel.SetActive(true);

        if (losingPlayer == 1)
        {
            winnerText.text = "Green Snake Wins!";
        }
        else if (losingPlayer == 2) 
        {
            winnerText.text = "Blue Snake Wins!";
        }
    }
}
