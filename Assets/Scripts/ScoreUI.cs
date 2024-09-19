using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    private void OnEnable()                         //활성화될 때 이벤트 등록
    {
        EventSystem.OnScoreChanged += UpdateScore;
        EventSystem.OnGameOver += ShowGameOver;
    }

    private void OnDisable()                        //비활성화될 때 이벤트 해제
    {
        EventSystem.OnScoreChanged += UpdateScore;
        EventSystem.OnGameOver += ShowGameOver;
    }

    void UpdateScore(int newScore)
    {
        Debug.Log($"Score Update: {newScore}");
    }

    void ShowGameOver()
    {
        Debug.Log("Game Over!");
    }
}
