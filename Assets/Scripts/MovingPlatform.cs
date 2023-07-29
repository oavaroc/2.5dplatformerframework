using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Transform[] _destinations;
    [SerializeField]
    private float _speed = 5f;

    private int _currentDestination = 0;

    [SerializeField]
    private float _delayAtEachDestination = 5f;

    private bool _keepRunning = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestinationRoutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destinations[_currentDestination].position, Time.deltaTime * _speed);
    }

    IEnumerator DestinationRoutine()
    {
        while (_keepRunning)
        {
            yield return new WaitForSeconds(_delayAtEachDestination);
            NextDestination();
            while(Vector3.Distance(transform.position, _destinations[_currentDestination].position) > 0.1f)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void NextDestination()
    {
        _currentDestination = (_currentDestination + 1) % _destinations.Length;
    }
}
