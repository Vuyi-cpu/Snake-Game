using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class SnakeDeath : MonoBehaviour
{
    [Header("Player")]
    public int playerNumber = 1;
    public string snakeName = "Blue";
    public int startingLives = 4;

    [Header("References")]
    public SnakeMovement snakeMovement;
    public SnakeGameManager gameManager;
    public TMP_Text livesText;

    [Header("Respawn")]
    public float respawnDelay = 1f;

    [Header("Growth Reset")]
    public UnityEvent onSnakeRespawn;

    private int currentLives;
    private bool isRespawning;
    private bool isEliminated;

    void Start()
    {
        currentLives = startingLives;
        UpdateLivesText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isRespawning || isEliminated)
            return;

        if (other.CompareTag("Wall") ||
            other.CompareTag("SnakeBody") || other.CompareTag("SnakeHead"))
        {
            LoseLife();
        }
    }

    private void LoseLife()
    {
        currentLives--;
        UpdateLivesText();

        if (currentLives <= 0)
        {
            isEliminated = true;

            snakeMovement.enabled = false;

            gameManager.GameOver(playerNumber);
        }
        else
        {
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;

        snakeMovement.enabled = false;

        yield return new WaitForSeconds(respawnDelay);


        onSnakeRespawn.Invoke();

        snakeMovement.ResetState();

        snakeMovement.enabled = true;
        isRespawning = false;
    }

    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "" + currentLives;
        }
    }
}

