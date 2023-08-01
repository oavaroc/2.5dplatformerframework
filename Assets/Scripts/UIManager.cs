using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private TextMeshProUGUI _ScoreCount;
    [SerializeField]
    private TextMeshProUGUI _LivesCount;


    private int _Lives = 0;
    private int _Score = 0;

    public void AddScore(int score)
    {
        Debug.Log("Adding score: " + score);
        _Score += score;
        _ScoreCount.text = _Score.ToString();
    }
    public int UpdateLives(int lives)
    {
        Debug.Log("Updating lives: " + lives);
        _Lives += lives;
        _LivesCount.text = _Lives.ToString();
        return _Lives;
    }
}
