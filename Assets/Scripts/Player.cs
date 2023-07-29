using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _characterController;

    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _gravity;
    private float _horizontalDirection=0;

    [SerializeField]
    private int _maxJumps = 2;
    private int _jumpsAvailable = 0;

    private PlayerInputActions _input;

    private float _yVelocity = 0;

    private Vector3 _movementDirection = Vector3.zero;

    private Animator _anim;

    private bool _goingRight = true;
    private bool _jumping = false;

    [SerializeField]
    private GameObject _climbUpPoint;

    private bool _grabbingLedge = false;
    private bool _climbingUp = false;

    // Start is called before the first frame update
    void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();

        _characterController = GetComponent<CharacterController>();
        if (_characterController == null)
        {
            Debug.Log("Character controller is null");
        }

        _anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_characterController.enabled)
        {
            HandleMovement();

        }else if (_input.Player.ClimbUp.WasPressedThisFrame() && _grabbingLedge)
        {
            _grabbingLedge = false;
            _climbingUp = true;
            _anim.SetBool("ClimbUp", _climbingUp);
            _anim.SetBool("LedgeGrab", _grabbingLedge);

        }
    }

    private void HandleMovement()
    {
        _movementDirection = new Vector3(0, 0, _input.Player.HorizontalMovement.ReadValue<float>()) * _speed;
        if(_input.Player.HorizontalMovement.ReadValue<float>() > 0)
        {
            transform.rotation = new Quaternion(0,0,0,0);
        }
        else if(_input.Player.HorizontalMovement.ReadValue<float>() < 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        if (_characterController.isGrounded)
        {
            if (_jumping)
            {
                _jumping = false;
                _anim.SetBool("Jumping", _jumping);
            }
            _anim.SetFloat("Speed", Mathf.Abs(_movementDirection.z));
            _jumpsAvailable=0;
            _yVelocity = 0;
        }
        else
        {
            _yVelocity -= _gravity;
        }
        HandleJump();

        _movementDirection.y = _yVelocity;
        _characterController.Move(_movementDirection  * Time.deltaTime);

    }
    private void HandleJump()
    {
        if (_input.Player.Jump.WasPressedThisFrame() && _jumpsAvailable < _maxJumps)
        {
            _jumping = true;
            _anim.SetBool("Jumping", _jumping);
            _jumpsAvailable++;
            _yVelocity = _jumpHeight;
        }
    }

    public void ActivateLedgeGrab(Vector3 pos)
    {
        _grabbingLedge = true;
        _anim.SetBool("LedgeGrab", _grabbingLedge);
        _jumping = false;
        _anim.SetBool("Jumping", _jumping);
        _characterController.enabled = false;
        transform.position = pos;
    }

    public void ClimbUp()
    {
        _climbingUp = false;
        _anim.SetBool("ClimbUp", _climbingUp);
        transform.position = _climbUpPoint.transform.position;
        _characterController.enabled = true;
    }
}
