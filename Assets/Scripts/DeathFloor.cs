using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Damaged");
            GameManager.Instance.DamagePlayer(-1);
            GameManager.Instance.RespawnPlayer();
        }
    }
}
