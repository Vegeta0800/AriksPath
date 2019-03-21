using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingElement : MonoBehaviour {

    [SerializeField]
    Transform[] bouncingElements = new Transform[0];

    [SerializeField]
    Vector3 amplitudeShift = new Vector3(0,2,0);
    [SerializeField]
    Vector3 amplitudeRotation = new Vector3(0, -45, 0);
    [SerializeField]
    float periodLength = 5.0f;

    float current = 0;
    float temp;

    Vector3 oldposition;
    Vector3 oldRotation;
    Vector3 positionShift;
    Vector3 rotationShift;
    // Use this for initialization
    private void OnEnable()
    {
        if (bouncingElements.Length > 0)
        {
            Inputs.onUpdate += OnUpdate;
            if (periodLength <= 0) periodLength = 0.05f;
        }
    }

    private void OnUpdate()
    {
        temp = (1 - Mathf.Cos(current / periodLength * Mathf.PI * 2)) / 2.0f;
        oldposition = temp * amplitudeShift;
        oldRotation = temp * amplitudeRotation;

        current += Time.deltaTime;
        if (current > periodLength) current -= periodLength;

        temp = (1 - Mathf.Cos(current / periodLength * Mathf.PI * 2)) / 2.0f;
        positionShift = temp * amplitudeShift - oldposition;
        rotationShift = temp * amplitudeRotation - oldRotation;

        for (int i = 0; i < bouncingElements.Length; i++)
        {
            bouncingElements[i].position += positionShift;
            bouncingElements[i].eulerAngles += rotationShift;
        }
    }

    private void OnDisable()
    {
        Inputs.onUpdate -= OnUpdate;
    }
}
