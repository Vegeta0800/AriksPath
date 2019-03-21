using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonanceShake : ResonanceBase {
    
    [Header("Shake")]
    [SerializeField]
    float shakeDistance = 0.01f;

	// Use this for initialization
	void Start () {
        Setup();
    }

    protected override void Resonate(sSimpleNote note, sSimpleTone tone)
    {
        StartCoroutine(Shivering(note.timeUntilNext, note.volume * tone.volume));
    }

    IEnumerator Shivering(float duration, float intensity)
    {
        Vector3 startPosition = transform.localPosition;
        float current = 0;
        while (current <= duration)
        {
            //smooth = (1 - Mathf.Cos(current / duration * Mathf.PI)) / 2.0f;
            transform.localPosition = startPosition + Random.insideUnitSphere * shakeDistance;
            current += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = startPosition;
    }
}
