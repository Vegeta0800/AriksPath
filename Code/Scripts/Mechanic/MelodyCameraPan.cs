/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controls camera pan when the pipes play a melody
/// </summary>
public class MelodyCameraPan : MonoBehaviour
{
    
    [SerializeField]
    Transform viewTarget;
    [SerializeField]
    Transform camPosition;

    GameObject cam;
    Cameras camScript;
    [SerializeField]
    float cameraMinMoveDuration = 0.5f;
    [SerializeField]
    float cameraMaxMoveDuration = 1.5f;

    Transform playerTransform;
    MovementScript moveScript;

    public System.Action reachedTarget;

    float actualTime;
    Quaternion startRotation;
    Quaternion endRotation;
    Vector3 startPosition;
    Vector3 positionShift;

    [HideInInspector]
    int mySong;
    SoundPuzzleOrgan myOrgan;
    /// <summary>
    /// moves camera
    /// </summary>
    /// <param name="songID">amount of sounds in melody?</param>
    public void LockAndMove(int songID, SoundPuzzleOrgan organ)
    {
        mySong = songID;
        myOrgan = organ;
        if (viewTarget == null) viewTarget = myOrgan.transform;
        organ.songEnded += MelodyEnded;

        if (cam == null)
        {
            if (cameraMinMoveDuration > cameraMaxMoveDuration)
            {
                Debug.LogWarning(gameObject.name + ": Min < Max, du Otto!");
                cameraMinMoveDuration = cameraMaxMoveDuration;
            }
            cam = GameObject.FindWithTag("MainCamera");
            camScript = cam.gameObject.GetComponent<Cameras>();
            //if (viewTarget == null)
            //    viewTarget = GameObject.Find(defaultTargetName).transform;
            playerTransform = GameObject.FindWithTag(StringCollection.TAG_PLAYER).transform;
            moveScript = playerTransform.GetComponent<MovementScript>();
        }

        camScript.enabled = false;
        moveScript.cameraAnimationLocked = true;
        moveScript.StopWalking();
        StartCoroutine(CameraShift());
    }

    IEnumerator CameraShift()
    {
        float current = 0;
        startRotation = cam.transform.rotation;
        Quaternion playerStartRotation = playerTransform.rotation;

        startPosition = cam.transform.position;
        /*
        Vector3 positionShift = startPosition;
        if (playerTransform.position.y + heightOffset > startPosition.y)
        {
            positionShift.y = playerTransform.position.y + heightOffset;
        }

        positionShift -= startPosition;
        */
        positionShift = ((camPosition)
                    ? camPosition.position : camScript.transform.position);
        Vector3 targetDir = (viewTarget.transform.position - positionShift).normalized;
        positionShift -= startPosition;

        endRotation = Quaternion.LookRotation(
            targetDir, Vector3.up);
        Quaternion playerEndRotation;
        {
            Vector3 targetPosition = viewTarget.transform.position;
            targetPosition.y = playerTransform.position.y;
            //TODO: CHARACTER is currently rotated by 180°, affects all direction related actions
            playerEndRotation = Quaternion.LookRotation(
                playerTransform.position - targetPosition, Vector3.up);
        }

        float smooth;
        //TODO: CHARACTER is currently rotated by 180°, affects all direction related actions
        actualTime = cameraMinMoveDuration
            + (1 + Vector3.Dot(-cam.transform.forward, targetDir)) / 2.0f
                * (cameraMaxMoveDuration - cameraMinMoveDuration);
        // camera turns to target
        //Debug.Log("a " + actualTime);
        while (current <= actualTime)
        {
            //smooth = (current / actualTime);
            smooth = (1 - Mathf.Cos(current / actualTime * Mathf.PI)) / 2.0f;
            cam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, smooth);
            playerTransform.rotation = Quaternion.Slerp(playerStartRotation, playerEndRotation, smooth);
            cam.transform.position = startPosition + smooth * positionShift;
            current += Time.deltaTime;
            yield return null;
        }
        // camera is looking at target
        cam.transform.rotation = endRotation;
        cam.transform.position = startPosition + positionShift;
        if (reachedTarget != null) reachedTarget();
    }

    private void MelodyEnded(int id)
    {
        if (id == mySong)
        {
            myOrgan.songEnded -= MelodyEnded;
            StartCoroutine(ReturnToStartPosition());
        }
    }

    private IEnumerator ReturnToStartPosition()
    {
        float current = 0;
        float smooth;
        // camera returns to previous fokus
        //Debug.Log("b "+actualTime);
        while (current <= actualTime)
        {
            smooth = (1 - Mathf.Cos(current / actualTime * Mathf.PI)) / 2.0f;

            cam.transform.rotation = Quaternion.Slerp(endRotation, startRotation, smooth);
            cam.transform.position = (startPosition + positionShift) - smooth * positionShift;
            current += Time.deltaTime;
            yield return null;
        }
        cam.transform.rotation = startRotation;
        cam.transform.position = startPosition;
        camScript.enabled = true;
        moveScript.cameraAnimationLocked = false;
    }
}
