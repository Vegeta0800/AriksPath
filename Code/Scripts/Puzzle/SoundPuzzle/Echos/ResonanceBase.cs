/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResonanceBase : MonoBehaviour {

    [SerializeField]
    protected eTone resonatingTone = eTone.A3;

    [SerializeField]
    protected SoundPuzzleOrgan pipeOrgan;

    // Use this for initialization
    void Start () {
        Setup();
	}

    /// <summary>
    /// call this at the start of every subclass!
    /// </summary>
    protected void Setup()
    {
        if (pipeOrgan == null)
            pipeOrgan = SoundPuzzleOrgan.instance;
        if (pipeOrgan == null)
        {
            Debug.Log(gameObject.name + " " + this.name
                + ":'Lonely, i feel so lonely... i've got no Organ to play with me!'");
        }
        else
            pipeOrgan.singleNote += HearedSound;
    }

    /// <summary>
    /// decides if sound is worth a reaction
    /// </summary>
    protected void HearedSound(sSimpleNote sound)
    {
        if (sound.tone == resonatingTone && sound.timeUntilNext > 0)
        {
            sSimpleTone tone = pipeOrgan.gamut.Get(resonatingTone);
            if (tone != null)
                Resonate(sound, tone);
        }
    }

    /// <summary>
    /// specific reaction
    /// </summary>
    protected abstract void Resonate(sSimpleNote note, sSimpleTone tone);
}
