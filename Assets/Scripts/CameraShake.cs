using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    public float shakeDuration;

    public float shakeAmount = 0.7f;

    public float decreaseFactor = 1f;

    private Vector3 originalPos;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        originalPos = base.transform.localPosition;
    }

    private void Update()
    {
        if (shakeDuration > 0f)
        {
            base.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            base.transform.localPosition = originalPos;
        }
    }

    public void Play(float strength = 0.3f, float duration = 0.2f)
    {
        shakeAmount = strength;
        shakeDuration = duration;
    }
}
