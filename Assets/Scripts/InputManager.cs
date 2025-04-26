using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions _actions;

    private InputAction _move;

    private InputAction _look;

    private InputAction _jump;

    private InputAction _dash;

    private InputAction _attack;

    public FrameInput Input => Gather();

    private void Awake()
    {
        _actions = new PlayerInputActions();
        _move = _actions.Player.Move;
        _look = _actions.Player.Look;
        _jump = _actions.Player.Jump;
        _dash = _actions.Player.Dash;
        _attack = _actions.Player.Attack;
    }

    private void OnEnable()
    {
        _actions.Enable();
    }

    private void OnDisable()
    {
        _actions.Disable();
    }

    private FrameInput Gather()
    {
        return new FrameInput
        {
            JumpDown = _jump.WasPressedThisFrame(),
            JumpHeld = _jump.IsPressed(),
            DashDown = _dash.WasPressedThisFrame(),
            AttackDown = _attack.WasPressedThisFrame(),
            Move = _move.ReadValue<Vector2>(),
            Look = _look.ReadValue<Vector2>()
        };
    }
}
