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
    [SerializeField]
    private GameObject _dashPoint;

    private bool _grabbingLedge = false;
    private bool _climbingUp = false;

    private bool _climbingLadder = false;
    private bool _reachedLadder = false;

    private Transform _ladderTop;
    private Transform _ladderBottom;

    private bool _inRoll = false;

    // Start is called before the first frame update
    void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Dash.performed += Dash_performed;

        _characterController = GetComponent<CharacterController>();
        if (_characterController == null)
        {
            Debug.Log("Character controller is null");
        }

        _anim = GetComponentInChildren<Animator>();
    }

    private void Dash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_characterController.enabled && !_jumping )
        {
            _inRoll = true;
            _characterController.enabled = false;
            _anim.SetBool("PlayerRoll",true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_inRoll)
        {
            if (_characterController.enabled)
            {
                HandleMovement();

            }else
            {
                HandleNonControllerMovement();
            }

        }
    }
    private void HandleLedgeClimb()
    {
        _grabbingLedge = false;
        _climbingUp = true;
        _anim.SetBool("ClimbUp", _climbingUp);
        _anim.SetBool("LedgeGrab", _grabbingLedge);

    }
    private void HandleLadderClimb()
    {

        if ((_input.Player.LadderClimb.ReadValue<float>() > 0 && transform.position.y <= _ladderTop.position.y) ||
            (_input.Player.LadderClimb.ReadValue<float>() < 0 && transform.position.y >= _ladderBottom.position.y))
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3.up * _input.Player.LadderClimb.ReadValue<float>()), Time.deltaTime);// * _speed);
            _anim.SetBool("ClimbingDown", _input.Player.LadderClimb.ReadValue<float>() < 0);
        }
    }
    private void HandleFinishLadder()
    {
        Debug.Log("Dismount start");
        _climbingLadder = false;
        _anim.SetBool("StartLadder", _climbingLadder);
        if (_input.Player.LadderClimb.ReadValue<float>() > 0)
            transform.position = _climbUpPoint.transform.position;
        _characterController.enabled = true;
    }
    private void HandleNonControllerMovement()
    {
        if (_input.Player.ClimbUp.WasPressedThisFrame() && _grabbingLedge)
        {
            HandleLedgeClimb();
        }
        else if (_climbingLadder)
        {
            if(_input.Player.LadderClimb.ReadValue<float>() != 0)
                HandleLadderClimb();
            if (_ladderTop.position.y - transform.position.y <= 1 || transform.position.y - _ladderBottom.position.y <= 1)
                HandleFinishLadder();
            _anim.SetBool("ActiveLadder", Mathf.Abs(_input.Player.LadderClimb.ReadValue<float>()) > 0);
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
                //_anim.SetBool("Jumping", _jumping);
                //_anim.SetTrigger("JumpingTrigger");
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
            //_anim.SetBool("Jumping", _jumping);
            _anim.SetTrigger("JumpingTrigger");
            _jumpsAvailable++;
            _yVelocity = _jumpHeight;
        }
    }

    public void ActivateLedgeGrab(Vector3 pos)
    {
        _grabbingLedge = true;
        _anim.SetBool("LedgeGrab", _grabbingLedge);
        _jumping = false;
        //_anim.SetBool("Jumping", _jumping);

        _anim.SetTrigger("JumpingTrigger");
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

    public void StartClimbingLadder(Vector3 position, Transform top, Transform bottom)
    {
        _ladderTop = top;
        _ladderBottom = bottom;
        _climbingLadder = true;
        _anim.SetBool("StartLadder", _climbingLadder);
        _anim.SetBool("ClimbingDown", false);
        _characterController.enabled = false;
        transform.position = position;
    }

    public void StopClimbingLadder(Vector3 position)
    {
        //_climbingLadder = false;
        _anim.SetBool("StartLadder", _climbingLadder);
        transform.position = position;
        _characterController.enabled = true;
        Debug.Log("Stopping ladder");

        if (_climbingLadder &&  (_ladderTop.position.y - transform.position.y <= 1 || transform.position.y - _ladderBottom.position.y <= 1))
        {
            Debug.Log("Stopping ladder");
            //needs to be outside of if w/s is pressed
            //trigger dismount anim
            Debug.Log("Dismount start");
            ClimbUp();
            //_anim.speed = 1;
            _climbingLadder = false;
            _anim.SetBool("StartLadder", _climbingLadder);
            transform.position = _climbUpPoint.transform.position;
            _characterController.enabled = true;
        }


    }

    public bool GetClimbingLadder()
    {
        return _climbingLadder;
    }

    public void SetReachLadder(bool reached)
    {
        _reachedLadder = reached;
    }

    public void GoToDashPoint()
    {
        _anim.SetBool("PlayerRoll", false);
        _inRoll = false;
        transform.position = _dashPoint.transform.position;
        _characterController.enabled = true;
    }
}
