using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    [Header("REFERENCES")]
    [SerializeField]
    private CharacterController _controller;

    [SerializeField]
    private Camera _camera;

    private Vector3 _frameMovement;

    private InputManager _inputManager;

    private float _timeLeftGrounded = float.MaxValue;

    private bool _groundedLastFrame;

    private float _timeSinceGrounded;

    [Header("DIRECTION")]
    [SerializeField]
    private float _acceleration = 10f;

    [SerializeField]
    private float _deAcceleration = 15f;

    [SerializeField]
    private float _correctionAccelerationModifier = 2f;

    [SerializeField]
    private float _airMoveSpeedPenalty = 0.2f;

    [SerializeField]
    private float _noInputAirCorrection = 0.3f;

    [Header("GRAVITY")]
    [SerializeField]
    private float _maxFallSpeed = 50f;

    [SerializeField]
    private float _minFallSpeed = 30f;

    [Header("JUMPING")]
    [SerializeField]
    private float _jumpForce = 40f;

    [SerializeField]
    private float _coyoteTimeThreshold = 0.15f;

    [SerializeField]
    private float _jumpBuffer = 0.15f;

    private float _lastJumpPressed = float.MinValue;

    private bool _endedJumpEarly;

    private bool _coyoteUsable;

    [Header("DASH")]
    [SerializeField]
    private float _dashThreshold = 0.2f;

    [SerializeField]
    private float _dashLength = 0.02f;

    [SerializeField]
    private float _dashSpeed = 5f;

    [SerializeField]
    private float _dashCooldown = 0.5f;

    [SerializeField]
    private Vector3 _dashCollisionDetectorOffset;

    private Vector3 _dashDir;

    private float _dashTimeStarted = -10f;

    [Header("MOVEMENT")]
    [SerializeField]
    public float _speed = 13f;

    [SerializeField]
    private float _backwardSpeedPenalty = 0.7f;

    [SerializeField]
    private float _strafeSpeedPenalty = 0.9f;

    [Header("ROTATION")]
    [SerializeField]
    private float _rotationSpeed = 1f;

    [SerializeField]
    private Transform _cameraRoot;

    private float _targetPitch;

    private float _turnVelocity;

    private const int FULL_ROTATION = 360;

    private const float PITCH_CLAMP = 89.5f;

    public Transform Cam => _camera.transform;

    public Vector3 Velocity { get; private set; }

    public FrameInput FrameInput { get; private set; }

    public bool Dashing { get; private set; }

    public bool Grounded => _controller.isGrounded;

    private bool CanUseCoyote
    {
        get
        {
            if (_coyoteUsable && !Grounded)
            {
                return _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
            }
            return false;
        }
    }

    private bool HasBufferedJump
    {
        get
        {
            if (Grounded)
            {
                return _lastJumpPressed + _jumpBuffer > Time.time;
            }
            return false;
        }
    }

    private bool CanDash
    {
        get
        {
            if (!Dashing)
            {
                return _dashTimeStarted + _dashCooldown < Time.time;
            }
            return false;
        }
    }

    public event Action<float> Landed;

    public event Action Jumped;

    public event Action DashStarted;

    public event Action DashEnded;

    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        GatherInput();
        CalculateCollisions();
        CalculateDirection();
        CalculateGravity();
        CalculateJump();
        CalculateDash();
        Move();
    }

    private void LateUpdate()
    {
        HandleRotation();
    }

    private void GatherInput()
    {
        FrameInput = _inputManager.Input;
        _ = FrameInput.JumpDown;
        if (FrameInput.JumpDown)
        {
            _lastJumpPressed = Time.time;
        }
    }

    private void CalculateCollisions()
    {
        if (!_groundedLastFrame && Grounded)
        {
            _groundedLastFrame = true;
            _coyoteUsable = true;
            this.Landed?.Invoke(_frameMovement.y);
            _timeSinceGrounded = Time.time;
        }
        else if (_groundedLastFrame && !Grounded)
        {
            _groundedLastFrame = false;
            _timeLeftGrounded = Time.time;
        }
    }

    private void CalculateDirection()
    {
        Vector2 normalized = FrameInput.Move.normalized;
        float speedMultiplier = ((!_controller.isGrounded && normalized == Vector2.zero) ? _noInputAirCorrection : 1f);
        _frameMovement.x = Set(_frameMovement.x, normalized.x, speedMultiplier);
        _frameMovement.z = Set(_frameMovement.z, normalized.y, speedMultiplier);
        float Set(float current, float input, float num2)
        {
            float num = (((input < 0f && current > 0f) || (input > 0f && current < 0f)) ? (_acceleration * _correctionAccelerationModifier) : _acceleration);
            return Mathf.MoveTowards(current, input, ((input == 0f) ? _deAcceleration : (num * (_controller.isGrounded ? 1f : _airMoveSpeedPenalty))) * num2 * Time.deltaTime);
        }
    }

    private void CalculateGravity()
    {
        if (!Grounded)
        {
            float t = Mathf.InverseLerp(10f, 0f, Mathf.Abs(Velocity.y));
            float num = Mathf.Lerp(_maxFallSpeed, _minFallSpeed, t);
            _frameMovement.y -= num * Time.deltaTime;
        }
    }

    private void CalculateJump()
    {
        if ((FrameInput.JumpDown && CanUseCoyote) || HasBufferedJump)
        {
            _frameMovement.y = _jumpForce;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue;
            this.Jumped?.Invoke();
        }
    }

    private void CalculateDash()
    {
        if (FrameInput.DashDown && CanDash)
        {
            _dashDir = new Vector3(FrameInput.Move.x, 0f, FrameInput.Move.y).normalized;
            if (_dashDir == Vector3.zero)
            {
                _dashDir = new Vector3(0f, 0f, 1f).normalized;
            }
            _dashTimeStarted = Time.time;
            this.DashStarted?.Invoke();
            Dashing = true;
        }
        if (Dashing)
        {
            _frameMovement = _dashDir * _dashSpeed;
            _frameMovement.y = 0f;
            if (_dashTimeStarted + _dashLength < Time.time)
            {
                Dashing = false;
                this.DashEnded?.Invoke();
            }
        }
    }

    private void Move()
    {
        Vector3 vector = base.transform.forward * (_frameMovement.z * _speed * ((_frameMovement.z < 0f) ? _backwardSpeedPenalty : 1f));
        vector += base.transform.right * (_frameMovement.x * _speed * _strafeSpeedPenalty);
        vector.y = _frameMovement.y;
        _controller.Move(vector * Time.deltaTime);
        Velocity = _controller.velocity;
    }

    private void HandleRotation()
    {
        _targetPitch += FrameInput.Look.y * _rotationSpeed;
        if (_targetPitch < -360f)
        {
            _targetPitch += 360f;
        }
        else if (_targetPitch > 360f)
        {
            _targetPitch -= 360f;
        }
        _targetPitch = Mathf.Clamp(_targetPitch, -89.5f, 89.5f);
        _cameraRoot.localRotation = Quaternion.Euler(_targetPitch, 0f, 0f);
        _turnVelocity = FrameInput.Look.x * _rotationSpeed;
        base.transform.Rotate(Vector3.up * _turnVelocity);
    }
}
