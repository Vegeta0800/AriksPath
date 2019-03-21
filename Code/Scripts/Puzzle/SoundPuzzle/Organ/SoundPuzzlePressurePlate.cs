/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPuzzlePressurePlate : MonoBehaviour {

    [SerializeField]
    protected bool playerOnly = true;
    [SerializeField]
    [Tooltip("doesn't react when stepping onto it")]
    public bool deaf;
    [SerializeField]
    protected float minDontReactSeconds = 1;
    protected float nextDeafDuration;
    [Space(5)]
    [SerializeField]
    protected bool fireOnce = false;
    [SerializeField]
    protected bool destroyAfterActivation = false;
    [Space(5)]
    [SerializeField]
    [Tooltip("only needed if there is more than one")]
    protected SoundPuzzleOrgan pipeOrgan;
    [SerializeField]
    [Tooltip("only needed if there is more than one")]
    protected SoundPuzzleRiddler quizMaster;
    
    [SerializeField]
    float shiftUpTime = 0.2f;
    [SerializeField]
    float shiftDownTime = 0.1f;
    [SerializeField]
    float pressedDownOffset = 10;
    Vector3 unpressedPosition;
    Vector3 pressedPosition;

    Coroutine moving;

    [Tooltip("Optional")]
    protected AudioSource pressedSound;
    protected AudioSource releasedSound;

    // Use this for initialization
    void Start ()
    {
        Setup();
    }

    protected void Setup()
    {
        if (pipeOrgan == null)
            pipeOrgan = SoundPuzzleOrgan.instance;
        if (pipeOrgan == null)
        {
            Debug.Log(gameObject.name + " " + this.name
                + ":'Lonely, i feel so lonely... i've got no Organ to play with me!'");
        }

        if (quizMaster == null)
            quizMaster = SoundPuzzleRiddler.instance;
        if (quizMaster == null)
        {
            Debug.Log(gameObject.name + " " + this.name
                + ":'Lonely, i feel so lonely... i've got no Riddler to play with me!'");
        }
        
        nextDeafDuration = minDontReactSeconds;

        if (transform.childCount > 0)
        {
            unpressedPosition = transform.GetChild(0).localPosition;
            pressedPosition = unpressedPosition;
            pressedPosition.y += pressedDownOffset;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("touched "+gameObject.name);
        if (!playerOnly || col.tag == StringCollection.TAG_PLAYER)
        {
            StepOn();
            if (!deaf)
            {
                Reaction();
                AfterReaction();
            }

            if (destroyAfterActivation)
                Destroy(this.gameObject);
            else if (fireOnce)
                Destroy(this);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (!playerOnly || col.tag == StringCollection.TAG_PLAYER)
        {
            if (releasedSound)
                releasedSound.Play();
            if (moving != null) StopCoroutine(moving);
            moving = StartCoroutine(MoveDown(false));
        }
    }

    /// <summary>
    /// player stepped onto plate
    /// </summary>
    protected void StepOn()
    {
        if (pressedSound)
            pressedSound.Play();
        if (moving != null) StopCoroutine(moving);
        moving = StartCoroutine(MoveDown(true));
    }

    /// <summary>
    /// system isn't deaf and sends reaction
    /// </summary>
    protected virtual void Reaction()
    {
        //TODO: fill, or don't fill
    }

    /// <summary>
    /// button was pressed and triggered reaction
    /// </summary>
    protected virtual void AfterReaction()
    {
        StartCoroutine(DontListen(nextDeafDuration));
    }
    
    protected virtual IEnumerator DontListen(float duration)
    {
        deaf = true;
        yield return new WaitForSeconds(duration);
        deaf = false;
    }


    private IEnumerator MoveDown(bool moveDown)
    {
        if (transform.childCount == 0) yield break;
        float current = 0;
        float smooth;
        Vector3 beginningPosition = transform.GetChild(0).localPosition;
        Vector3 posShift = (moveDown) ? pressedPosition : unpressedPosition;
        float actualTime = (moveDown) ? shiftDownTime : shiftUpTime;
        actualTime = Vector3.Distance(beginningPosition, posShift) / Mathf.Abs(pressedDownOffset) * actualTime;
        posShift -= beginningPosition;
        //Debug.Log("actualTime:" + actualTime + " beginningPosition:"
        //            + beginningPosition + " posShift" + posShift);
        if (actualTime > 0)
        {
            while (current <= actualTime)
            {
                smooth = (1 - Mathf.Cos(current / actualTime * Mathf.PI)) / 2.0f;
                transform.GetChild(0).localPosition = beginningPosition + smooth * posShift;
                current += Time.deltaTime;
                yield return null;
            }
            transform.GetChild(0).localPosition = (moveDown) ? pressedPosition : unpressedPosition;
        }
    }
}
