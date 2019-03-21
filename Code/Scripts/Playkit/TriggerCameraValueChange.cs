using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraValueChange : MonoBehaviour
{

    Cameras camScript;

    [SerializeField]
    float shiftLength = 1.0f;
    bool setCameraRotation;
    Vector3 newRotation = new Vector3(30, 0, 0);

    [Header("Values")]
    [SerializeField]
    Vector3 newHeightOffSet = new Vector3(0, 3, 0);
    [SerializeField]
    float newMinDistance = 0.6f;
    [SerializeField]
    float newIdealDistance = 5.0f;

    [SerializeField]
    float newHorizontalPerSecond = 60;
    [SerializeField]
    float newVerticalPerSecond = 60;
    [SerializeField]
    [Range(-89.9f, 89.9f)]
    float newVerticalMin = -15f;
    [SerializeField]
    [Range(-89.9f, 89.9f)]
    float newVerticalMax = 75f;

    float newAdjustmentSpeed = 10;

    bool alreadyChanging;
    [Space(10)]
    public bool useCollider = true;
    [SerializeField]
    bool playerOnly = true;
    [Space(5)]
    [SerializeField]
    bool fireOnlyOnce = true;
    public bool destroyGameObjectAfterUse = false;

    // Use this for initialization
    void Start()
    {
        GameObject cam = GameObject.FindWithTag("MainCamera");
        camScript = cam.GetComponent<Cameras>();
    }

    private void OnTriggerEnter(Collider co)
    {
        //Debug.Log("Can touch this!");
        if (!playerOnly || co.tag == "Player")
        {
            if (useCollider)
            {
                if (!alreadyChanging)
                {

                    Debug.Log(this.gameObject.name + " triggered Camera.");
                    StartCoroutine(ChangeValues());
                }
                alreadyChanging = true;
            }
        }
    }

    private IEnumerator ChangeValues()
    {
        float current = 0;
        float smooth;
        Vector3 startRotation = camScript.transform.localEulerAngles;

        Vector3 startHeightOffset = camScript.heightOffset;
        float startMinDistance = camScript.minDistance;
        float startIdealDistance = camScript.idealDistance;

        float startHorizontalPerSecond = camScript.horizontalAnglePerSecond;

        float startVerticalPerSecond = camScript.verticalAnglePerSecond;
        float startVerticalMin = camScript.verticalAngleMin;
        float startVerticalMax = camScript.verticalAngleMax;

        float startAdjustmentSpeed = camScript.adjustmentSpeed;

        while (current <= shiftLength)
        {
            //smooth = (current / actualTime);
            smooth = (1 - Mathf.Cos(current / shiftLength * Mathf.PI)) / 2.0f;
            if (setCameraRotation)
                camScript.transform.localEulerAngles = startRotation + smooth * (newRotation - startRotation);


            camScript.heightOffset = startHeightOffset + smooth * (newHeightOffSet - startHeightOffset);

            camScript.minDistance = startMinDistance + smooth * (newMinDistance - startMinDistance);
            camScript.idealDistance = startIdealDistance + smooth * (newIdealDistance - startIdealDistance);

            camScript.horizontalAnglePerSecond = startHorizontalPerSecond + smooth * (newHorizontalPerSecond - startHorizontalPerSecond);
            
            camScript.verticalAnglePerSecond = startVerticalPerSecond + smooth * (newVerticalPerSecond - startVerticalPerSecond);
            camScript.verticalAngleMin = startVerticalMin + smooth * (newVerticalMin - startVerticalMin);
            camScript.verticalAngleMax = startVerticalMax + smooth * (newVerticalMax - startVerticalMax);

            camScript.adjustmentSpeed = startAdjustmentSpeed + smooth * (newAdjustmentSpeed - startAdjustmentSpeed);

            current += Time.deltaTime;
            yield return null;
        }


    }

    public void Remove()
    {
        if (fireOnlyOnce)
        {
            //Debug.Log("Remove");
            if (destroyGameObjectAfterUse)
            {
                //Debug.Log("Remove all");
                Destroy(this.gameObject);
            }
            else
            {
                //Debug.Log("RemoveComponent");
                Destroy(this);
            }
        }
        else
            alreadyChanging = false;
    }
}
