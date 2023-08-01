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

    [SerializeField]
    private int _maxJumps = 2;
    private int _jumpsAvailable = 0;

    private PlayerInputActions _input;

    private float _yVelocity = 0;

    private Vector3 _movementDirection = Vector3.zero;

    private Animator _anim;

    private bool _jumping = false;

    [SerializeField]
    private GameObject _climbUpPoint;

    private bool _grabbingLedge = false;
    private bool _climbingUp = false;

    private bool _climbingLadder = false;

    private Transform _ladderTop;
    private Transform _ladderBottom;

    private bool _inRoll = false;


    [SerializeField] private float _rollSpeed = 10f;
    [SerializeField] private float _rollDuration = 0.5f;

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

        UIManager.Instance.UpdateLives(3);
    }

    private void Dash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_characterController.enabled && !_jumping )
        {

            if (Input.GetKeyDown(KeyCode.LeftShift) && !_inRoll)
            {
                StartCoroutine(PerformDodgeRoll());
            }
        }
    }
    private void OnDestroy()
    {
        _input.Player.Dash.performed -= Dash_performed;
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
        {   //if ladder climb is positive, go up, if negative go down
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
            HandleLaddeClimbLogic();
        }
    }
    private void HandleLaddeClimbLogic()
    {
        if(_input.Player.LadderClimb.ReadValue<float>() != 0)// if moving on the ladder, handle the logic
            HandleLadderClimb();
        if (_ladderTop.position.y - transform.position.y <= 1 || transform.position.y - _ladderBottom.position.y <= 1)// at the top/bottom, exit ladder climb
            HandleFinishLadder();
        _anim.SetBool("ActiveLadder", Mathf.Abs(_input.Player.LadderClimb.ReadValue<float>()) > 0);//if the character is moving, play climb/descend ladder anim

    }
    private void HandleMovement()
    {
        _movementDirection = new Vector3(0, 0, _input.Player.HorizontalMovement.ReadValue<float>()) * _speed;
        PerformMovementAlterations(_movementDirection);
    }

    private void ApplyGravity()
    {
        _yVelocity -= _gravity;

    }

    private void PerformMovementAlterations(Vector3 _movementDirection)
    {
        RotateCharacterTowardsMovement();
        if (_characterController.isGrounded)
        {
            GroundedMovementCalculations();
        }
        else
        {
            ApplyGravity();
        }
        HandleJump();
        MoveCharacter(_movementDirection);

    }

    private void RotateCharacterTowardsMovement()
    {

        if (_input.Player.HorizontalMovement.ReadValue<float>() > 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if (_input.Player.HorizontalMovement.ReadValue<float>() < 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }

    private void GroundedMovementCalculations()
    {
        if (_jumping)
        {
            _jumping = false;
            _anim.ResetTrigger("JumpingTrigger");
        }
        _anim.SetFloat("Speed", Mathf.Abs(_movementDirection.z));
        _jumpsAvailable = 0;
        _yVelocity = 0;
    }

    private void MoveCharacter(Vector3 _movementDirection)
    {
        _movementDirection.y = _yVelocity;
        _characterController.Move(_movementDirection  * Time.deltaTime);

    }

    private void HandleJump()
    {
        if (_input.Player.Jump.WasPressedThisFrame() && _jumpsAvailable < _maxJumps)
        {
            _jumping = true;
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
        _anim.SetFloat("Speed", 0);
        transform.position = position;
    }

    public void StopClimbingLadder(Vector3 position)
    {
        Debug.Log("Is it coming here?");

        _anim.SetBool("StartLadder", _climbingLadder);
        transform.position = position;
        _characterController.enabled = true;
        Debug.Log("Stopping ladder");

        if (_climbingLadder &&  (_ladderTop.position.y - transform.position.y <= 1 || transform.position.y - _ladderBottom.position.y <= 1))
        {
            Debug.Log("Stopping ladder");
            Debug.Log("Dismount start");
            ClimbUp();
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


    private IEnumerator PerformDodgeRoll()
    {
        // Set flag to prevent multiple rolls at the same time
        _inRoll = true;

        // Trigger the dodge roll animation
        _anim.SetBool("PlayerRoll", _inRoll);

        // Calculate the dodge direction based on player input (you may need to modify this based on your character's setup)
        Vector3 dodgeDirection = transform.forward;

        // Apply the dodge roll movement
        float rollTimer = 0f;
        while (rollTimer < _rollDuration)
        {
            //_characterController.Move(dodgeDirection * _rollSpeed * Time.deltaTime);
            ApplyGravity();
            MoveCharacter(dodgeDirection * _rollSpeed);
            rollTimer += Time.deltaTime;
            yield return null;
        }

        // Reset the flag
        _inRoll = false;
        _anim.SetBool("PlayerRoll", _inRoll);
    }
}
