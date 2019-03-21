using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPuzzleBridgeRepair : MonoBehaviour
{

    public bool ignoreAnimation;

    public int startOffset = -20;
    public float shiftTime = 5;

    [HideInInspector]
    public bool solved;

    Animator myAnimator;
    [SerializeField]
    GameObject barrier;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip(" 1 = instantly, 0 = at the end")]
    float deactivationPoint = 0.75f;

    Vector3 startPosition;

    [Header("Optional")]
    [SerializeField]
    float shakeIntensity = 0.025f;
    [SerializeField]
    bool playAudioSource;
    [SerializeField]
    bool startMelodyWithRepair;
    [SerializeField]
    int melodyID;
    [Space(5)]
    [SerializeField]
    SoundPuzzleBridgeRepair nextSegment;
    [SerializeField]
    float nextSegmentDelay = 1;
    [SerializeField]
    [Tooltip("only needed if there is more than one")]
    SoundPuzzleOrgan pipeOrgan;
    [SerializeField]
    CutsceneCameraPans cameraMove;

    // Use this for initialization
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        if (ignoreAnimation)
        {
            startPosition = transform.localPosition;
            transform.localPosition += new Vector3(0, startOffset, 0);
        }
    }

    public void RepairBridge()
    {
        Debug.Log(gameObject.name + ": Did it!");
        if (!ignoreAnimation)
        {
            myAnimator.SetTrigger("BuildUp");
            if (barrier) barrier.SetActive(false);
        }
        else StartCoroutine(MoveUp()); ;
        solved = true;

        //Debug.Log(startMelodyWithRepair + " " + pipeOrgan + " " + melodyID + " " + cameraMove);
        if (startMelodyWithRepair)
        {
            if (pipeOrgan == null)
                pipeOrgan = SoundPuzzleOrgan.instance;
            if (pipeOrgan != null)
            {
                //pipeOrgan.PlayMelodyInstant(melodyID);
                pipeOrgan.PlayOrAddToWaitingList(melodyID);
            }
        }
        if (playAudioSource)
        {
            AudioSource a = GetComponent<AudioSource>();
            if (a != null)
                a.Play();
        }
        if (cameraMove)
            cameraMove.LockAndMoveCamera();
        if (nextSegment)
            StartCoroutine(DelayedNextSegment(nextSegmentDelay));
    }

    private IEnumerator MoveUp()
    {
        float current = shiftTime;
        float smooth;
        Vector3 posShift = transform.position - startPosition;
        Vector3 nextPos;
        Vector2 shake;
        // camera returns to previous fokus
        //Debug.Log("b "+actualTime);
        while (current > deactivationPoint * shiftTime)
        {
            smooth = (1 - Mathf.Cos(current / shiftTime * Mathf.PI)) / 2.0f;

            nextPos = startPosition + smooth * posShift;
            shake = Random.insideUnitCircle;
            nextPos.x += shake.x * shakeIntensity;
            nextPos.z += shake.y * shakeIntensity;
            transform.position = nextPos;
            current -= Time.deltaTime;
            yield return null;
        }
        if (barrier) barrier.SetActive(false);
        //Debug.Log("Collider deactivated");
        while (current > 0)
        {
            smooth = (1 - Mathf.Cos(current / shiftTime * Mathf.PI)) / 2.0f;

            nextPos = startPosition + smooth * posShift;
            shake = Random.insideUnitCircle;
            nextPos.x += shake.x * shakeIntensity;
            nextPos.z += shake.y * shakeIntensity;
            transform.position = nextPos;
            current -= Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition;
    }

    private IEnumerator DelayedNextSegment(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        nextSegment.RepairBridge();
    }
}
