using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonanceCameraPan : ResonanceBase
{
    [Header("Camera Pans")]
    [SerializeField]
    int matchingSong;

    [SerializeField]
    CutsceneCameraPans[] pans = new CutsceneCameraPans[0];
    int nextPan;

    [SerializeField]
    bool destroyComponentAfterLast = true;


    // Use this for initialization
    void Start()
    {
        Setup();
        pipeOrgan.singleNote -= HearedSound;//remove default
        pipeOrgan.songStart += StartHearingSong;
        pipeOrgan.songEnded += EndHearingSong;
    }

    private void StartHearingSong(int songID)
    {
        if (songID == matchingSong)
            pipeOrgan.singleNote += HearedSound;
    }

    private void EndHearingSong(int songID)
    {
        if (songID == matchingSong)
            pipeOrgan.singleNote -= HearedSound;
    }

    protected override void Resonate(sSimpleNote note, sSimpleTone tone)
    {
        if (nextPan < pans.Length && pans[nextPan] != null)
        {
            pans[nextPan].LockAndMoveCamera();
        }
        nextPan++;
        if (nextPan >= pans.Length)
        {
            pipeOrgan.singleNote -= HearedSound;
            pipeOrgan.songStart -= StartHearingSong;
            pipeOrgan.songEnded -= EndHearingSong;
            if (destroyComponentAfterLast)
                Destroy(this);
        }
    }

}
