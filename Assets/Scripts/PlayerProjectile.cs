using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField]
    private SphereCollider _collider;

    [SerializeField]
    private Rigidbody _rb;

    [SerializeField]
    private float _timeTillDeath = 10f;

    [SerializeField]
    private float _explosionRadius = 1f;

    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private float _colliderExpandTime = 0.5f;

    [SerializeField]
    private float _colliderExpandMultiplier = 1.5f;

    private bool _hasHit;

    private float _goalSize;

    public void Init(float speed)
    {
        _rb.velocity = transform.forward * speed;
        _goalSize = _collider.radius * _colliderExpandMultiplier;
        Destroy(gameObject, _timeTillDeath);
    }

    private void Update()
    {
        _collider.radius = Mathf.MoveTowards(_collider.radius, _goalSize, _colliderExpandTime * Time.deltaTime);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (_hasHit)
    //    {
    //        return;
    //    }
    //    _hasHit = true;
    //    //Collider[] array = Physics.OverlapSphere(base.transform.position, _explosionRadius);
    //    //for (int i = 0; i < array.Length; i++)
    //    //{
    //    //    if (array[i].TryGetComponent<IHit>(out var component))
    //    //    {
    //    //        NewPlayer newPlayer = Object.FindObjectOfType<NewPlayer>();
    //    //        Vector3 vector = collision.contacts[0].point - newPlayer.transform.position;
    //    //        component.Hit(vector.normalized, isMelee: false);
    //    //    }
    //    //}
    //    //if (!collision.transform.TryGetComponent<Titan>(out var _))
    //    //{
    //    //    Object.Instantiate(_explosionPrefab, base.transform.position, Quaternion.identity);
    //    //}
    //    Destroy(gameObject);
    //}
}
