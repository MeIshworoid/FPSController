using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayer
{
    FrameInput FrameInput { get; }

    Vector3 Velocity { get; }

    bool Grounded { get; }

    bool Dashing { get; }

    Transform Cam { get; }

    event Action Jumped;

    event Action<float> Landed;

    event Action DashStarted;

    event Action DashEnded;
}
