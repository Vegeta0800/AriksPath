using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SceneAndItsTransitions
{
    public eLevel otherLevel;
    [Space(5)]
    [Tooltip("Position and Rotation")]
    public Transform EntryPlayerPosition;
    [Tooltip("Optional")]
    public Transform EntryCameraRotation;
    //[Space(5)]
    //public TriggerSceneChange ExitTrigger;
}

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField]
    [Header("Object position used, if no specific found")]
    SceneAndItsTransitions[] sceneSpecificSpawnPoints;

    public static SpawnPointManager localSpawnPoint { get; private set; }
    [Header("Intro Camera (optional)")]
    [SerializeField]
    bool useIntroCamera = true;
    [SerializeField]
    CutsceneCameraPans introCamera;
    //[SerializeField]
    //Transform introCameraTransform;
    [SerializeField]
    float introCameraDelay = 1f;

    private void OnEnable()
    {

        localSpawnPoint = this;
    }

    /*
    void Awake()
    {
        Debug.Log(gameObject.name + " scene: "+gameObject.scene.name);
    }
    */

    public void Use(Transform target)
    {
        // positioning

        Positioning(target);

        if (target.tag == StringCollection.TAG_PLAYER)
        {
            PlayerPrefs.SetFloat("LastPositionX", target.position.x);
            PlayerPrefs.SetFloat("LastPositionY", target.position.y);
            PlayerPrefs.SetFloat("LastPositionZ", target.position.z);
        }

        // intro camera
        if (useIntroCamera && introCamera != null /*&& introCameraTransform != null*/)
        {
            MovementScript ms = target.GetComponent<MovementScript>();
            if (ms != null)
                ms.cameraAnimationLocked = true;

            Cameras cam = Camera.main.GetComponent<Cameras>();
            if (cam)
                cam.enabled = false;

            // Camera.main.transform.position = introCameraTransform.position;
            // Camera.main.transform.rotation = introCameraTransform.rotation;
            StartCoroutine(DelayedAction());
        }
    }

    private IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(introCameraDelay);
        introCamera.LockAndMoveCamera();
    }

    private void Positioning(Transform target)
    {
        if (TriggerSceneChange.instance)
        {
            eLevel origin = TriggerSceneChange.instance.originLevel;

            for (int i = 0; i < sceneSpecificSpawnPoints.Length; i++)
            {
                if (sceneSpecificSpawnPoints[i].otherLevel == origin)
                {
                    Transform nextTransform = sceneSpecificSpawnPoints[i].EntryPlayerPosition;
                    target.position = nextTransform.position;
                    target.rotation = nextTransform.rotation;

                    Cameras cam = Camera.main.GetComponent<Cameras>();
                    nextTransform = sceneSpecificSpawnPoints[i].EntryCameraRotation;
                    if (nextTransform != null)
                    {
                        cam.transform.rotation = nextTransform.rotation;
                    }
                    else
                    {
                        Vector3 targetRotation = target.eulerAngles;
                        targetRotation.y += 180; // rotated character...

                        if (cam.transform.parent)
                            cam.transform.parent.eulerAngles = targetRotation;
                    }

                    return;
                }
            }
        }
        target.position = this.transform.position;
        target.rotation = this.transform.rotation;
    }

    private void OnDisable()
    {
        if (localSpawnPoint == this)
            localSpawnPoint = null;
    }
}
