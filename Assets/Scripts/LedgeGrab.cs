using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrab : MonoBehaviour
{
    [SerializeField]
    private Transform _playerHangPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LedgeGrab"))
        {
            Player player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                player.ActivateLedgeGrab(_playerHangPosition.position);
            }
        }
    }
}
