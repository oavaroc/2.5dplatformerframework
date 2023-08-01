using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private GameObject _startPoint;

    [SerializeField]
    private CharacterController _playerCharacterController;

    public void DamagePlayer(int damage)
    {
        Debug.Log("Damaging player");
        if (UIManager.Instance.UpdateLives(damage) <= 0)
        {
            Debug.Log("Reloading scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RespawnPlayer()
    {
        Debug.Log("Respawning");
        _playerCharacterController.enabled = false;
        _playerCharacterController.transform.position = _startPoint.transform.position;
        _playerCharacterController.enabled = true;
    }
}
