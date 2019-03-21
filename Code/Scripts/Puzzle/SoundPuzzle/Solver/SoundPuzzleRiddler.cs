/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPuzzleRiddler : MonoBehaviour {

    [SerializeField]
    SoundPuzzleBridgeManager solver;


    [SerializeField]
    bool repeatMelodyAtSuccess = false;
    [SerializeField]
    [Tooltip("played if melody isn't repeated")]
    sSimpleTone success;
    [SerializeField]
    float delayedSuccess;
    [SerializeField]
    sSimpleTone failure;
    [SerializeField]
    float waitForFailureSound = 1;

     [Header("Optional")]
    [SerializeField]
    SoundPuzzleOrgan myOrgan;
    /*
    [SerializeField]
    [Tooltip("Will seek in own component if empty")]
    AudioSource audioSource;
    */
    [Header("Just Info")]
    [SerializeField]
    int seekedMelody = -1;
    [SerializeField]
    eTone[] riddleMelody;
    [SerializeField]
    List<eTone> enteredTones = new List<eTone>();

    public bool questioning{ get { return riddleMelody != null; } }

    [HideInInspector]
    public bool busy;

    System.Action readyAgain;

    /// <summary>
    /// triggered if a puzzle was solved, hands over melodyID
    /// </summary>
    public System.Action<int> puzzleSolved; 

    public static SoundPuzzleRiddler instance { get; private set; }

    void Awake()
    {
        if (instance != null)
            Debug.Log("Two " + this.name + " " 
                        +instance.gameObject.name + "/" + this.gameObject.name);
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        if (myOrgan == null)
            myOrgan = SoundPuzzleOrgan.instance;
        if (myOrgan == null)
        {
            Debug.Log(gameObject.name + " "+ this.name
                + ":'Lonely, i feel so lonely... i've got no Organ to play with me!'");
        }
        /*
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.Log(gameObject.name + " " + this.name
                + ":'Lonely, i feel so lonely... i've got no AudioSource to play with me!'");
        */

        if (solver== null)
            Debug.Log(gameObject.name + " " + this.name
                + ":'Give me a SoundPuzzleBridgeManager ...'");
    }

    public void PressedKey(sSimpleNote newNote)
    {
        if (busy) return;
        myOrgan.PlayNoteInstant(newNote);
        if (riddleMelody == null || riddleMelody.Length < 1) return;

        if (newNote.tone == riddleMelody[enteredTones.Count])
        {
            if (enteredTones.Count + 1 < riddleMelody.Length)
                enteredTones.Add(newNote.tone);
            else
                Success(newNote.timeUntilNext);
        }
        else
            Failure(newNote.timeUntilNext);
    }

    public void ChangeRiddle(int newMelodyID)
    {
        seekedMelody = newMelodyID;
        enteredTones.Clear();
        riddleMelody = myOrgan.GetBasicMelodieSheet(newMelodyID);
    }

    private void Success(float wait)
    {
        busy = true;
        StartCoroutine(DelayedSuccess(wait));
    }

    private void Failure(float wait)
    {
        busy = true;
        enteredTones.Clear();
        StartCoroutine(DelayedFailure(wait));
    }

    /*
    private void PlayTone(sSimpleTone tone)
    {
        audioSource.clip = tone.audioClip;
        audioSource.volume = tone.volume;
        audioSource.pitch = tone.pitch;
        audioSource.Play();
    }
    */

    IEnumerator DelayedSuccess(float baseWaitingTime)
    {
        yield return new WaitForSeconds(baseWaitingTime
                                    + ((delayedSuccess >= baseWaitingTime) ? delayedSuccess : 0));
        if (puzzleSolved != null) puzzleSolved(seekedMelody);
        if (repeatMelodyAtSuccess)
        {
            myOrgan.PlayMelodyInstant(seekedMelody);
            StartCoroutine(DontListen());
            readyAgain += TriggerSolver;
        }
        else
        {
            myOrgan.PlayTone(success, 1, 1);
            readyAgain += TriggerSolver;
            TriggerSolver();
        }
        riddleMelody = null;
    }

    IEnumerator DelayedFailure(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        myOrgan.PlayTone(failure, 1, 1);
        yield return new WaitForSeconds(waitForFailureSound);
        busy = false;
    }

    IEnumerator DontListen()
    {
        busy = true;
        sMelody song = myOrgan.melodies[seekedMelody];
        if (song.speed <= 0) song.speed = 1;
        float duration = 0;
        for (int i = 0; i < song.melody.Length; i++)
        {
            duration += song.melody[i].timeUntilNext / song.speed;
        }
        yield return new WaitForSeconds(duration);
        busy = false;
        Debug.Log("Listening again.");
        if (readyAgain != null) readyAgain();
    }

    private void TriggerSolver()
    {
        Debug.Log("TriggerSolved");
        solver.PuzzleStepSolved(seekedMelody);
        this.readyAgain -= TriggerSolver;
        busy = false;
    }
}
