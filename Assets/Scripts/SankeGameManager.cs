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
    
    public GameObject startUI;

    private bool player1Lost;
    private bool player2Lost;
    private bool gameHasEnded;
    private bool resultCheckStarted;

    
    private static bool isReplaying = false; 

    void Start()
    {
        brownGameOverPanel.SetActive(false);
        blueGameOverPanel.SetActive(false);
        drawGameOverPanel.SetActive(false);

        
        if (isReplaying)
        {
            startUI.SetActive(false);
            player1Movement.enabled = true;
            player2Movement.enabled = true;
        }
        else
        {
           
            startUI.SetActive(true);
            player1Movement.enabled = false;
            player2Movement.enabled = false;
        }
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
        
        isReplaying = true; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        startUI.SetActive(false);
        player1Movement.enabled = true;
        player2Movement.enabled = true;
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked.");
        Application.Quit();
    }
}