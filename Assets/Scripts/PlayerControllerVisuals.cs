using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerControllerVisuals : MonoBehaviour
{
    private IPlayer _player;

    private bool _hasInput;

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private Camera _cam;

    [SerializeField]
    private Transform _tiltParent;

    [SerializeField]
    private float _maxCameraTilt = 2.5f;

    [SerializeField]
    private float _camTiltSpeed = 16f;

    [SerializeField]
    private Transform _ImNoBoDYParent;

    [SerializeField]
    private AnimationCurve _ImNoBoDYCurve;

    [SerializeField]
    private float _ImNoBoDYCurveSpeed = 2f;

    private float _ImNoBoDY;

    [SerializeField]
    private AudioClip[] _footstepClips;

    [SerializeField]
    private float _timeBetweenSteps = 0.1f;

    private float _stepTime;

    [Header("JUMP")]
    [SerializeField]
    private AudioClip[] _jumpClips;

    [Header("Landing drop")]
    [SerializeField]
    private AnimationCurve _landingDropCurve;

    [SerializeField]
    [Range(0f, 10f)]
    private float _landingDropAmount = 1f;

    [SerializeField]
    private float _minLandingDropTime = 0.3f;

    [SerializeField]
    private float _maxLandingDropTime = 1f;

    [SerializeField]
    private float _maxLandingForce = -30f;

    [SerializeField]
    private Transform _landingDropTransform;

    [SerializeField]
    private AudioClip[] _landingClips;

    [Header("DASH")]
    [SerializeField]
    private AudioClip _dashClip;

    [SerializeField]
    private float _dashChromatic = 0.5f;

    [SerializeField]
    private float _dashChromaticFade = 0.2f;

    [SerializeField]
    private ParticleSystem _speedLines;

    [SerializeField]
    private float _maxSpeedLines;

    private float _dashStarted;

    [Header("WOOSH")]
    [SerializeField]
    private float _minVelWoosh = 5f;

    [SerializeField]
    private float _maxVelWoosh = 30f;

    [SerializeField]
    private AudioSource _wooshSource;

    [SerializeField]
    private float _maxWooshVol = 0.6f;

    [SerializeField]
    private float _wooshSpeedGain = 5f;

    private PostProcessVolume _volume;

    private ChromaticAberration _chrome;

    private DepthOfField _depth;

    private bool _usePost;

    private float _goalDepthDistance;

    [SerializeField]
    private LayerMask _depthHitLayers;

    [SerializeField]
    private float _depthAdjustSpeed = 1f;

    private void Awake()
    {
        _player = GetComponentInParent<IPlayer>();
        InitPost();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _player.Landed += PlayerOnLanded;
        _player.DashStarted += PlayerOnDashStarted;
        _player.DashEnded += PlayerOnDashEnded;
        _player.Jumped += PlayerOnJumped;
    }

    private void OnDestroy()
    {
        _player.Landed -= PlayerOnLanded;
        _player.DashStarted -= PlayerOnDashStarted;
        _player.DashEnded -= PlayerOnDashEnded;
        _player.Jumped -= PlayerOnJumped;
    }

    private void Update()
    {
        if (_hasInput && _player.FrameInput.Move == Vector2.zero)
        {
            _hasInput = false;
        }
        else if (!_hasInput && _player.FrameInput.Move != Vector2.zero)
        {
            _hasInput = true;
        }
        HandleCameraTilt();
        HandleCameraBob();
        HandleFootsteps();
        HandleSpeedLines();
        HandlePost();
        HandleWoosh();
    }

    private void HandleCameraTilt()
    {
        Vector3 euler = new Vector3(0f, 0f, Mathf.Lerp(_maxCameraTilt, 0f - _maxCameraTilt, Mathf.InverseLerp(-1f, 1f, _player.FrameInput.Move.x)));
        _tiltParent.transform.localRotation = Quaternion.RotateTowards(_tiltParent.transform.localRotation, Quaternion.Euler(euler), _camTiltSpeed * Time.deltaTime);
    }

    private void HandleCameraBob()
    {
        if (_hasInput && _player.Grounded)
        {
            float t = Mathf.InverseLerp(0f, 1f, _player.FrameInput.Move.magnitude);
            float num = Mathf.Lerp(0f, _ImNoBoDYCurveSpeed, t);
            _ImNoBoDY += num * Time.deltaTime;
            float time = Mathf.PingPong(_ImNoBoDY, 1f);
            _ImNoBoDYParent.localPosition = new Vector3(0f, _ImNoBoDYCurve.Evaluate(time), 0f);
        }
    }

    private void HandleFootsteps()
    {
        if (_hasInput && _player.Grounded)
        {
            _stepTime += Time.deltaTime * (Mathf.InverseLerp(0f, 20f, _player.Velocity.magnitude) + 0.2f);
            if (_stepTime >= _timeBetweenSteps)
            {
                _audioSource.PlayRandom(_footstepClips);
                _stepTime = 0f;
            }
        }
    }

    private void PlayerOnJumped()
    {
        _audioSource.PlayRandom(_jumpClips);
    }

    private void PlayerOnLanded(float landingForce)
    {
        StartCoroutine(ExecuteLandingDrop(landingForce));
    }

    private IEnumerator ExecuteLandingDrop(float landingForce)
    {
        float num = Mathf.InverseLerp(0f, _maxLandingForce, landingForce);
        float totalTime = Mathf.Lerp(_minLandingDropTime, _maxLandingDropTime, num);
        _audioSource.PlayRandom(_landingClips, num);
        for (float t = 0f; t <= 1f; t += Time.deltaTime / totalTime)
        {
            float y = _landingDropCurve.Evaluate(t) * _landingDropAmount;
            _landingDropTransform.localPosition = new Vector3(0f, y);
            yield return new WaitForEndOfFrame();
        }
    }

    private void PlayerOnDashStarted()
    {
        _dashStarted = Time.time;
        _audioSource.PlayOneShot(_dashClip, 0.8f);
        StartCoroutine(DashChromatic());
    }

    private void PlayerOnDashEnded()
    {
    }

    private void HandleSpeedLines()
    {
        Vector3 velocity = _player.Velocity;
        velocity.y = 0f;
        float t = Mathf.InverseLerp(15f, 20f, velocity.magnitude);
        ParticleSystem.EmissionModule emission = _speedLines.emission;
        emission.rateOverTime = Mathf.Lerp(0f, _maxSpeedLines, t);
    }

    private void HandleWoosh()
    {
        float target = 0f;
        if (!_player.Grounded)
        {
            Vector3 velocity = _player.Velocity;
            velocity.y = 0f;
            float t = Mathf.InverseLerp(_minVelWoosh, _maxVelWoosh, velocity.magnitude);
            target = Mathf.Lerp(0f, _maxWooshVol, t);
        }
        _wooshSource.volume = Mathf.MoveTowards(_wooshSource.volume, target, _wooshSpeedGain * Time.deltaTime);
    }

    private void InitPost()
    {
        _volume = Object.FindObjectOfType<PostProcessVolume>();
        _usePost = _volume != null;
        if (_usePost)
        {
            _volume.profile.TryGetSettings<ChromaticAberration>(out _chrome);
            _volume.profile.TryGetSettings<DepthOfField>(out _depth);
            _goalDepthDistance = _depth.focusDistance;
        }
    }

    private IEnumerator DashChromatic()
    {
        if (_usePost && !(_chrome == null))
        {
            _chrome.intensity.value = _dashChromatic;
            while (_chrome.intensity.value > 0f)
            {
                _chrome.intensity.value -= Time.deltaTime / _dashChromaticFade;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void HandlePost()
    {
        if (_usePost)
        {
            Depth();
        }
    }

    private void Depth()
    {
        if (!(_depth == null))
        {
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out var hitInfo, 500f, _depthHitLayers))
            {
                _goalDepthDistance = hitInfo.distance;
            }
            _depth.focusDistance.value = Mathf.MoveTowards(_depth.focusDistance.value, _goalDepthDistance, _depthAdjustSpeed * Time.deltaTime);
        }
    }
}
