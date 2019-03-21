/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPuzzleTrigger : SoundPuzzlePressurePlate
{
    [Header("My Song")]
    [SerializeField]
    int melodyID;
    [SerializeField]
    bool setAsNextPuzzle;
    [SerializeField]
    MelodyCameraPan cameraPan;

    // Use this for initialization
    void Start()
    {
        Setup();
        
        if (melodyID < 0 || melodyID > pipeOrgan.melodies.Length - 1)
            Debug.Log(gameObject.name
             + ":'" + melodyID + ", den Song kennt keiner!'");

        if (cameraPan == null)
            cameraPan = GetComponent<MelodyCameraPan>();
        if (cameraPan != null)
            cameraPan.reachedTarget += DemandMelody;
    }

    /// <summary>
    /// system isn't deaf and sends reaction
    /// </summary>
    protected override void Reaction()
    {
        if (!quizMaster.busy)
        {
            if (setAsNextPuzzle && quizMaster)
            {
                quizMaster.puzzleSolved += GotSolved;
                quizMaster.ChangeRiddle(melodyID);
            }
            if (cameraPan)
                cameraPan.LockAndMove(melodyID,pipeOrgan);
            else
                DemandMelody();
        }
        base.Reaction();
    }

    /// <summary>
    /// button was pressed and triggered reaction
    /// </summary>
    protected override void AfterReaction()
    {
        sMelody song = pipeOrgan.melodies[melodyID];
        if (song.speed <= 0) song.speed = 1;
        float melodyDuration = 0;
        for (int i = 0; i < song.melody.Length; i++)
        {
            melodyDuration += song.melody[i].timeUntilNext / song.speed;
        }
        nextDeafDuration = (melodyDuration > minDontReactSeconds)
                            ? melodyDuration : minDontReactSeconds;
        base.AfterReaction();
    }

    /// <summary>
    /// triggered when camera reaches target
    /// </summary>
    private void DemandMelody()
    {
        Debug.Log(pipeOrgan.name + "->"+melodyID);
        //Debug.Log("Triggered Organ '"+pipeOrgan+ "' in " +gameObject.name);
        if (pipeOrgan)
            pipeOrgan.PlayOrAddToWaitingList(melodyID);
    }

    /// <summary>
    /// activated by Riddler
    /// </summary>
    /// <param name="ID"></param>
    private void GotSolved(int songID)
    {
        if (songID == melodyID)
        {
            setAsNextPuzzle = false;
            cameraPan = null;
        }
    }
}
