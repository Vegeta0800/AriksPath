

#define CHARACTER_ROTATED_180

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour
{
    #region variables

    [SerializeField]
    bool inverted;

    [Space(5)]
    [Tooltip("Uses Player, if empty")]
    [SerializeField]
    Transform optionalTarget;
    [SerializeField]
    public Vector3 heightOffset = new Vector3(0,3,0);

    [Space(5)]
    [SerializeField]
    public float minDistance = 0.6f;
    [SerializeField]
    public float idealDistance = 5.0f;

    [Header("Rotation Input")]
    [SerializeField]
    public float horizontalAnglePerSecond = 60;
    [Space(5)]
    [SerializeField]
    public float verticalAnglePerSecond = 60;
    [SerializeField]
    [Range(-89.9f, 89.9f)]
    public float verticalAngleMin = -15f;
    [SerializeField]
    [Range(0.0f, 89.9f)]
    public float verticalAngleMax = 75f;
    
    /// <summary>
    /// distance adjustment speed
    /// </summary>
    public float adjustmentSpeed = 10;

    /// <summary>
    /// distance towards player (0,0,z)
    /// </summary>
    Vector3 calcDistance;
    
    /// <summary>
    /// current Rotation
    /// </summary>
    Quaternion rotation;

    private int layerMask = 1 << 9;
    

    GameObject anchor;

    #endregion variables

    // Use this for initialization
    void Start()
    {
        calcDistance = new Vector3(0, 0, -idealDistance);
        if (!transform.parent)
        {
            anchor = new GameObject();
            anchor.name = this.gameObject.name + " (Camera Anchor)";
            this.transform.parent = anchor.transform;
        }
        else
        {
            anchor = transform.parent.gameObject;
        }

        if (optionalTarget == null)
            optionalTarget = GameObject.FindGameObjectWithTag(StringCollection.TAG_PLAYER).transform;
        if (optionalTarget == null)
            Debug.Log("Camera couldn't find Object with Player Tag.");
        
        anchor.transform.localRotation = this.transform.rotation;
        anchor.transform.localPosition = heightOffset;

        this.transform.localRotation = Quaternion.identity;
        this.transform.localPosition = calcDistance;

    }

    private void OnEnable()
    {
        rotation = transform.rotation;
        Inputs.cameraMovement += CameraRotationInput; 
    }

    private void LateUpdate()
    {
        anchor.transform.position = optionalTarget.position + heightOffset;
        calcDistance.z = -idealDistance;

        Vector3 anchorPosition = transform.parent.position;
        //Vector3 anchorPosition = optionalTarget.position + heightOffset;
        Vector3 currentIdealPosition = rotation * calcDistance + anchorPosition;

        RaycastHit hit;
        Debug.DrawLine(anchorPosition, currentIdealPosition);
        if (Physics.Linecast(anchorPosition, currentIdealPosition,
            out hit, ~layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.layer != (int)eGameLayer.CameraIgnored)
            {
                calcDistance.z = -Mathf.Clamp(hit.distance - 0.1f, minDistance, idealDistance);
            }
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition,
                                                calcDistance,
                                                Time.deltaTime * adjustmentSpeed);
        //transform.position = Vector3.Lerp(transform.position,
        //                                rotation * calcDistance + anchorPosition,
        //                                Time.deltaTime * adjustmentSpeed);
    }

    /// <summary>
    /// changes rotation according to the player's input
    /// </summary>
    private void CameraRotationInput(float inputX, float inputY)
    {
        if (Mathf.Abs(inputX) < 0.4f)
            inputX = 0;
        if (Mathf.Abs(inputY) < 0.4f)
            inputY = 0;
        if (inputX == 0 && inputY == 0)
            return;

        Vector3 newRotation = transform.parent.localEulerAngles;
        //Vector3 newRotation = transform.eulerAngles;
        newRotation.x += (inverted) ? -inputY : inputY * verticalAnglePerSecond * Time.deltaTime;
        newRotation.y += inputX * horizontalAnglePerSecond * Time.deltaTime;

        if (newRotation.x < -180F)
            newRotation.x += 360F;
        if (newRotation.x > 180F)
            newRotation.x -= 360F;
        newRotation.x = Mathf.Clamp(newRotation.x, verticalAngleMin, verticalAngleMax);

        rotation = Quaternion.Euler(newRotation);

        transform.parent.localRotation = rotation;
        //transform.rotation = rotation;
    }

    /*
    /// <summary>
    /// changes Rotations to get target into picture
    /// </summary>
    private void RefreshFocusToTarget()
    {
        transform.LookAt(optionalTarget.position + heightOffset, Vector3.up);
    }
    */

    private void OnDisable()
    {
        Inputs.cameraMovement -= CameraRotationInput;
    }
}
