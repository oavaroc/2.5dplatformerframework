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

    private PlayerInputActions _input;

    private void Start()
    {
        _input = new PlayerInputActions();
    }

    public void DamagePlayer(int damage)
    {
        Debug.Log("Damaging player");
        if (UIManager.Instance.UpdateLives(damage) <= 0)
        {
            Debug.Log("Reloading scene");
            ReloadScene();
        }
    }

    public void RespawnPlayer()
    {
        Debug.Log("Respawning");
        _playerCharacterController.enabled = false;
        _playerCharacterController.transform.position = _startPoint.transform.position;
        _playerCharacterController.enabled = true;
    }

    public void PlayerWon()
    {
        UIManager.Instance.WinScreenActivate();
        _playerCharacterController.gameObject.SetActive(false);
        _input.Enable();
        _input.Reload.Reload.performed += Reload_performed;
    }

    private void Reload_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ReloadScene();
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    private void OnDestroy()
    {

        _input.Reload.Reload.performed -= Reload_performed;
    }
}
