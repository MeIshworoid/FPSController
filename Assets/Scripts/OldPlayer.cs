using UnityEngine;

public class OldPlayer : MonoBehaviour
{
    public static OldPlayer Instance;

    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField]
    private PlayerProjectile _projectile;

    [SerializeField]
    private float _fireRate = 0.5f;

    [SerializeField]
    private float _projectileSpeed = 2f;

    private float _lastFired = float.MinValue;

    private Transform _cam;

    private void Awake()
    {
        Instance = this;
        _cam = base.transform.GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0) || !(_lastFired + _fireRate < Time.time))
        {
            return;
        }

        _lastFired = Time.time;
        Quaternion rotation = Quaternion.LookRotation(_spawnPoint.forward);

        if (Physics.Raycast(_cam.position, _cam.forward, out var hitInfo, 100f))
        {
            rotation = Quaternion.LookRotation(hitInfo.point - _spawnPoint.position);
        }

        Instantiate(_projectile, _spawnPoint.position, rotation).Init(_projectileSpeed);
    }
}
