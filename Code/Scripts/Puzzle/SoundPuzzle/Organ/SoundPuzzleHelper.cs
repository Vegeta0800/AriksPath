/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eTone
{
    // WARNIG:
    // adding entries might change existing melodies
    Break = 0,
    B3 = 36,
    Bb3 = 35,
    A3 = 34,
    Ab3 = 33,
    G3 = 32,
    Gb3 = 31,
    F3 = 30,

    E3 = 29,
    Eb3 = 28,
    D3 = 27,
    Db3 = 26,
    C3 = 25
}

public enum eMelody
{
    easy, //firstShort,
    hard, //secondMiddle,
    impossible //thirdLong
}

[System.Serializable]
public class sGamut
{
    public sSimpleTone B3;
    public sSimpleTone Bb3;
    public sSimpleTone A3;
    public sSimpleTone Ab3;
    public sSimpleTone G3;
    public sSimpleTone Gb3;
    public sSimpleTone F3;

    public sSimpleTone E3;
    public sSimpleTone Eb3;
    public sSimpleTone D3;
    public sSimpleTone Db3;
    public sSimpleTone C3;

    public sSimpleTone Get(eTone t)
    {
        switch (t)
        {
            case eTone.A3:
                return A3;
            case eTone.B3:
                return B3;
            case eTone.C3:
                return C3;
            case eTone.D3:
                return D3;
            case eTone.Break:
                return null;
            default:
                Debug.Log("Gamut: Nonexistant Tone.");
                return null;
        }
    }
}

[System.Serializable]
public class sSimpleTone
{
    public AudioClip audioClip;
    [Range(0.0f, 1.0f)]
    public float volume = 1;
    [Range(0.0f, 3.0f)]
    public float pitch = 1;
    [Space(5)]
    [Tooltip("Optional")]
    public Sprite symbol;
}

[System.Serializable]
public class sSimpleNote
{
    public eTone tone;
    [Range(0.0f, 1.0f)]
    public float volume = 1;
    [Range(0.0f, 3.0f)]
    public float pitch = 1;
    //public float speed;
    public float timeUntilNext = 0.5f;
}

[System.Serializable]
public class sMelody
{
    public float speed = 1;
    [Range(0.0f, 1.0f)]
    public float volume = 1;
    [Range(0.0f, 3.0f)]
    public float pitch = 1;
    public sSimpleNote[] melody;
}