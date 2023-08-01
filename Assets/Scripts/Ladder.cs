using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    private Transform _mountPosition;
    [SerializeField]
    private Transform _dismountPosition;
    [SerializeField]
    private Transform _ladderTop;
    [SerializeField]
    private Transform _ladderBottom;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (Keyboard.current.eKey.isPressed)
            {
                Debug.Log("point 1");
                if (other.TryGetComponent<Player>(out Player player))
                {
                    Debug.Log("point 3:"+ player.GetClimbingLadder());
                    if (player.GetClimbingLadder())
                    {
                        Debug.Log("point 2");
                        player.StopClimbingLadder(_dismountPosition.position);

                    }
                    else
                    {
                        player.StartClimbingLadder(_mountPosition.position, _ladderTop,_ladderBottom);
                        player.transform.rotation = transform.parent.rotation;
                    }

                }

            }
        }
    }
}
