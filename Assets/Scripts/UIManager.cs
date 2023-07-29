using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private TextMeshProUGUI _ScoreCount;


    private int _Score = 0;

    public void AddScore(int score)
    {
        Debug.Log("Adding score: " + score);
        _Score += score;
        _ScoreCount.text = _Score.ToString();
    }
}
