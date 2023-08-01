using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWinScript : MonoBehaviour
{
    [SerializeField]
    private int _coinsToWin = 200;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && UIManager.Instance.GetScore() >= _coinsToWin)
        {
            GameManager.Instance.PlayerWon();
        }
    }
}
