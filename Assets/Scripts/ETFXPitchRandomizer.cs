using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETFXPitchRandomizer : MonoBehaviour
{
    public float randomPercent = 10f;

    private void Start()
    {
        base.transform.GetComponent<AudioSource>().pitch *= 1f + Random.Range((0f - randomPercent) / 100f, randomPercent / 100f);
    }
}
