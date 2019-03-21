/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// controls camera pan when the pipes play a melody
/// </summary>
public class CutsceneCameraPans : MonoBehaviour
{

    [Header("Behaviour")]
    public bool useCollider = true;
    public bool rotatePlayerTowardsTarget = false;
    [Space(5)]
    public bool fireOnlyOnce = true;
    public bool destroyGameObjectAfterUse = false;

    [Header("Targetpositions & follow Up Pan")]
    [SerializeField]
    Transform target;
    [SerializeField]
    Transform cameraPosition;
    [SerializeField]
    CutsceneCameraPans followUpMovement;

    GameObject cam;

    [Header("Times")]
    [SerializeField]
    float cameraMinMoveDuration = 0.5f;
    [SerializeField]
    float cameraMaxMoveDuration = 1.5f;
    [SerializeField]
    float watchDuration = 1;
    Transform playerTransform;

    /* // Project:Nola
    CameraController camScript;
    Nola.Character.CharacterInputControl moveScript;
    /*/ // Project:Arik's Path
    Cameras camScript;
    MovementScript moveScript;
    /**/

    public static bool previousMove;
    public static Quaternion originalRotation;
    public static Vector3 originalPosition;

    /*
    void Awake()
    {
        Debug.Log(this.gameObject.name);
    }
    */

    public void LockAndMoveCamera()
    {
        if (cam == null)
        {
            if (cameraMinMoveDuration > cameraMaxMoveDuration)
            {
                Debug.LogWarning(gameObject.name + ": Min < Max, du Otto!");
                cameraMinMoveDuration = cameraMaxMoveDuration;
            }
            cam = GameObject.FindWithTag("MainCamera");
            playerTransform = GameObject.FindWithTag("Player").transform;

            /* // Project:Nola
            camScript = cam.gameObject.GetComponent<CameraController>();
            moveScript = playerTransform.GetComponent<Nola.Character.CharacterInputControl>();
            /*/ // Project:Arik's Path
            camScript = cam.gameObject.GetComponent<Cameras>();
            moveScript = playerTransform.GetComponent<MovementScript>();
            /**/

        }
        camScript.enabled = false;

        /* // Project:Nola
        moveScript.enabled = false;
        /*/ // Project:Arik's Path
        moveScript.cameraAnimationLocked = true;
        moveScript.StopWalking();
        /**/

        StartCoroutine(CameraShift());
    }

    IEnumerator CameraShift()
    {
        float current = 0;
        Quaternion startRotation = cam.transform.rotation;
        Quaternion playerStartRotation = playerTransform.rotation;

        Vector3 startPosition = cam.transform.position;
        Vector3 positionShift = (cameraPosition != null) ? cameraPosition.position : cam.transform.position;
        /*
        if (playerTransform.position.y + heightOffset > startPosition.y)
        {
            positionShift.y = playerTransform.position.y + heightOffset;
        }
        */
        Vector3 targetPosition = (target) ? target.transform.position : (startPosition + camScript.transform.forward);
        Vector3 targetDir = (targetPosition - positionShift).normalized;
        positionShift -= startPosition;

        var endRotation = Quaternion.LookRotation(
            targetDir, Vector3.up);
        Quaternion playerEndRotation;
        {
            targetPosition.y = playerTransform.position.y;
            //TODO: CHARACTER is currently rotated by 180°, affects all direction related actions
            playerEndRotation = Quaternion.LookRotation(
                targetPosition - playerTransform.position, Vector3.up);
        }

        //rotateTowardsTarget
        float smooth;
        //TODO: CHARACTER is currently rotated by 180°, affects all direction related actions
        float actualTime = cameraMinMoveDuration + (1 + Vector3.Dot(-cam.transform.forward, targetDir)) / 2.0f * (cameraMaxMoveDuration - cameraMinMoveDuration);
        // camera turns to target
        while (current <= actualTime)
        {
            //smooth = (current / actualTime);
            smooth = (1 - Mathf.Cos(current / actualTime * Mathf.PI)) / 2.0f;
            cam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, smooth);
            if (rotatePlayerTowardsTarget)
                playerTransform.rotation = Quaternion.Slerp(playerStartRotation, playerEndRotation, smooth);
            cam.transform.position = startPosition + smooth * positionShift;
            current += Time.deltaTime;
            yield return null;
        }

        // camera is looking at target
        cam.transform.rotation = endRotation;
        cam.transform.position = startPosition + positionShift;
        {
            /*
            float mop = MusicScript.instance.timePerSound;
            yield return new WaitForSeconds(mop * soundLengthPart);
            if (CollisionScript.onPlateStepped != null)
                CollisionScript.onPlateStepped(id, mel, melod);
                */
            yield return new WaitForSeconds(watchDuration);
        }

        if (!previousMove)
        {
            originalRotation = startRotation;
            originalPosition = startPosition;
            previousMove = true;
        }

        if (!followUpMovement)
        {
            // camera returns to player
            startPosition = originalPosition;
            startRotation = originalRotation;

            endRotation = cam.transform.rotation;
            positionShift = cam.transform.position - startPosition;

            //var endRotation = Quaternion.LookRotation(
            //    targetDir, Vector3.up);


            current = 0;
            while (current <= actualTime)
            {
                //smooth = (current / actualTime);
                smooth = (1 - Mathf.Cos(current / actualTime * Mathf.PI)) / 2.0f;
                cam.transform.rotation = Quaternion.Slerp(endRotation, startRotation, smooth);
                cam.transform.position = (startPosition + positionShift) - smooth * positionShift;
                current += Time.deltaTime;
                yield return null;
            }
            cam.transform.rotation = startRotation;
            cam.transform.position = startPosition;
            camScript.enabled = true;
            /* // Project:Nola
            moveScript.enabled = true;
            /*/ // Project:Arik's Path
            moveScript.cameraAnimationLocked = false;
            /**/

            previousMove = false;
        }
        else
        {
            followUpMovement.LockAndMoveCamera();
        }
        Remove();
    }

    private void OnTriggerEnter(Collider co)
    {
        if (co.tag == "Player")
        {
            Debug.Log(this.gameObject.name + " triggered.");
            if (useCollider)
            {
                LockAndMoveCamera();
            }
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
    }
}
