using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeGameManager : MonoBehaviour
{
    [Header("Snake Movement Scripts")]
    public SnakeMovement player1Movement;
    public SnakeMovement player2Movement;

    [Header("Game Over Panels")]
    public GameObject brownGameOverPanel;
    public GameObject blueGameOverPanel;
    public GameObject drawGameOverPanel;

    private bool player1Lost;
    private bool player2Lost;
    private bool gameHasEnded;
    private bool resultCheckStarted;

    void Start()
    {
        brownGameOverPanel.SetActive(false);
        blueGameOverPanel.SetActive(false);
        drawGameOverPanel.SetActive(false);
    }

    public void GameOver(int losingPlayer)
    {
        if (gameHasEnded)
            return;

        if (losingPlayer == 1)
        {
            player1Lost = true;
        }
        else if (losingPlayer == 2)
        {
            player2Lost = true;
        }

     
        player1Movement.enabled = false;
        player2Movement.enabled = false;

        if (!resultCheckStarted)
        {
            resultCheckStarted = true;
            StartCoroutine(CheckGameResult());
        }
    }

    private IEnumerator CheckGameResult()
    {
        yield return new WaitForEndOfFrame();

        gameHasEnded = true;

        if (player1Lost && player2Lost)
        {
            drawGameOverPanel.SetActive(true);
        }
        else if (player1Lost)
        {
           
            brownGameOverPanel.SetActive(true);
        }
        else if (player2Lost)
        {
  
            blueGameOverPanel.SetActive(true);
        }
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked.");
        Application.Quit();
    }
}