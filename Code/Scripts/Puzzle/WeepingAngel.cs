//@author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeepingAngel : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Uses Player, if empty")]
    Transform stalkedTarget;
    [SerializeField]
    [Tooltip("Uses Object, if empty")]
    Transform rotatedPart;
    Renderer rend;
    bool hasTarget = false;

    [Space(5)]
    [SerializeField]
    float reactionTime = 1;
    [SerializeField]
    [Range(5.0f, 180f)]
    float maxAnglePerSecond = 20;
    const float UPDATETACT = 0.5f;
    float countDown;

    [Space(5)]
    [SerializeField]
    [Tooltip("completely optional")]
    AudioSource sound;
    [SerializeField]
    [Range(0.1f, 180)]
    float soundMinAngle = 10;

    //[SerializeField]
    /*
    [Header("OnTriggerEnter (optional)")]
    [SerializeField]
    CutsceneCameraPans cameraPan;
    */
    private void OnEnable()
    {
        if (stalkedTarget == null)
        {
            GameObject target;
            target = GameObject.FindGameObjectWithTag(StringCollection.TAG_PLAYER);
            if (target)
                stalkedTarget = target.transform;
        }
        if (rotatedPart == null)
            rotatedPart = transform;

        if (stalkedTarget)
            rend = rotatedPart.GetComponentInChildren<Renderer>();

        hasTarget = (stalkedTarget && rend);

        if (hasTarget)
            Inputs.onUpdate += OnUpdate;
    }

    private void OnUpdate()
    {
        if (rend.isVisible)
        {
            countDown = reactionTime;
        }
        else
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                //Debug.Log("Klick");
                
                //(ghostworld OR not ghostworld only) AND(real world OR not real world only)
                if ((MovementScript.ghostWorldActive
                        || rotatedPart.gameObject.layer != (int)eGameLayer.GhostWorldOnly)
                    && (!MovementScript.ghostWorldActive
                        || rotatedPart.gameObject.layer != (int)eGameLayer.NormalWorldOnly))
                {
                    // angle towards player
                    //Debug.Log("Knack!");
                    Vector3 newAngle = rotatedPart.transform.eulerAngles;
                    Vector3 targetPosition = stalkedTarget.position - transform.position;

                    Vector3 tempVec = rotatedPart.transform.forward;
                    float turnRotation = (Mathf.Atan2(tempVec.z, tempVec.x)
                                        - Mathf.Atan2(targetPosition.z, targetPosition.x))
                                        * Mathf.Rad2Deg;

                    // shorter path
                    if (turnRotation > 180)
                        turnRotation -= 360;
                    else if (turnRotation < -180)
                        turnRotation += 360;

                    // limited speed
                    if (turnRotation > maxAnglePerSecond * UPDATETACT)
                    {
                        turnRotation = maxAnglePerSecond;
                    }
                    else if (turnRotation < -maxAnglePerSecond * UPDATETACT)
                    {
                        turnRotation = -maxAnglePerSecond;
                    }
                    
                    // rotate statue
                    newAngle.y += turnRotation;
                    rotatedPart.transform.eulerAngles = newAngle;
                    //Debug.Log("Knirsch "+turnRotation);

                    if (sound && !sound.isPlaying && sound.clip != null)
                    {
                        if (Mathf.Abs(turnRotation) >= soundMinAngle)
                            sound.Play();
                    }
                }
                countDown = UPDATETACT;
            }
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == StringCollection.TAG_PLAYER)
        {
            if (sound && sound.clip != null)
            {
                if (!sound.isPlaying) sound.Play();
            }
            if (cameraPan)
            {
                cameraPan.LockAndMoveCamera();
            }
        }
    }
    */
    private void OnDisable()
    {
        if (hasTarget)
            Inputs.onUpdate -= OnUpdate;
    }
}
