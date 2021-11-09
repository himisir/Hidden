using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameWinUI;
    public GameObject gameLoseUI;
    bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        Guard.OnGuardHasSpotedPlayer += ShowGameLoseUI;
        Player.OnLevelComplete += ShowGameWinUI;

    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }

    }

    void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
    }
    void ShowGameLoseUI()
    {
        OnGameOver(gameLoseUI);
    }
    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        isGameOver = true;
        Guard.OnGuardHasSpotedPlayer -= ShowGameLoseUI;
        Player.OnLevelComplete -= ShowGameWinUI;
    }

}
