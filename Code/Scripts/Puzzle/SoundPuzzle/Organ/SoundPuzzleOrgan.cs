/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPuzzleOrgan : MonoBehaviour
{
    // ideas:
    // hand over melodies instead of storing them constantly in the instrument
    // ui to generate note sheets
    // store & share melodies between users

    #region variables
    /// <summary>
    /// used if no specific organ is given
    /// </summary>
    public static SoundPuzzleOrgan instance { get; private set; }

    [SerializeField]
    public sGamut gamut;

    public sMelody[] melodies;

    List<int> nextMelody = new List<int>();

    public int currentlyPlayingSong;

    Coroutine playingSong;
    public bool currentlyPlaying;

    PipeMelodySymbols mySymbol;

    /// <summary>
    /// At the start of a song, includes id & length
    /// </summary>
    public System.Action<int> songStart;

    public System.Action<int> songEnded;

    /*
    /// <summary>
    /// Hands over played Note and follow up
    /// </summary>
    public System.Action<sSimpleNote, sSimpleNote> playedNote;
    */

    /// <summary>
    /// The last Note of a tune
    /// </summary>
    public System.Action<sSimpleNote> lastNote;

    /// <summary>
    /// Single Note
    /// </summary>
    public System.Action<sSimpleNote> singleNote;


    AudioSource[] audioSources;
    int audioSourceIndex = 0;

    #endregion variables

    void Awake()
    {
        if (instance != null)
            Debug.Log("Two " + this.name + "s " + instance.gameObject.name + "/" + this.gameObject.name);
        instance = this;
    }

    void Start()
    {
        if (audioSources == null)
            audioSources = GetComponentsInChildren<AudioSource>();
        if (audioSources == null)
            Debug.Log(gameObject.name + " " + this.name + ": 'Keine AudioSource, kein Ton.'");
        mySymbol = GetComponentInChildren<PipeMelodySymbols>();
    }

    /// <summary>
    /// plays Song or adds it to waiting list
    /// </summary>
    /// <param name="id"></param>
    public void PlayOrAddToWaitingList(int id)
    {
        if (currentlyPlaying)
        {
            nextMelody.Add(id);
            return;
        }
        StartCoroutine(PlaySong(id));
    }

    /// <summary>
    /// skip to next Song in waiting list
    /// </summary>
    public void NextSong()
    {
        if (currentlyPlaying) StopCoroutine(playingSong);
        if (songEnded != null) songEnded(currentlyPlayingSong);
        if (nextMelody.Count < 1) return;

        int nextSong = nextMelody[0];
        nextMelody.Remove(0);
        playingSong = StartCoroutine(PlaySong(nextSong));
    }

    private IEnumerator PlaySong(int id)
    {
        if (songStart != null) songStart(id);
           currentlyPlaying = true;
        currentlyPlayingSong = id;
           sMelody currentMelodie = this.melodies[id];
        if (currentMelodie.speed <= 0)
            instance.melodies[id].speed = 1;

        for (int i = 0; i < currentMelodie.melody.Length; i++)
        {
            sSimpleNote note = currentMelodie.melody[i];
            sSimpleTone tone = gamut.Get(note.tone);

            if (tone != null)
            {
                PlayTone(tone, note.volume * currentMelodie.volume, note.pitch * currentMelodie.pitch);
                if (singleNote != null) singleNote(note);
                if (MovementScript.ghostWorldActive && mySymbol) mySymbol.ShowSymbol(tone.symbol);
            }

            yield return new WaitForSeconds(note.timeUntilNext / currentMelodie.speed);
        }
        if(mySymbol)
            mySymbol.HideSymbol();
        //Debug.Log("Song ended");
        if (songEnded != null) songEnded(id);
        currentlyPlaying = false;
        NextSong();
    }

    /// <summary>
    /// instantly plays a given Note
    /// </summary>
    public void PlayNoteInstant(sSimpleNote note)
    {
        sSimpleTone tone = gamut.Get(note.tone);
        PlayTone(tone, note.volume, note.pitch);
        if (singleNote != null) singleNote(note);
        if (MovementScript.ghostWorldActive && mySymbol)
        {
            mySymbol.ShowSymbol(tone.symbol);
            StartCoroutine(mySymbol.DelayedHideSymbol(note.timeUntilNext));
        }
    }

    /// <summary>
    /// NEEDS BUGFIX
    /// </summary>
    public void PlayMelodyInstant(int id)
    {
        Debug.Log("TODO: BUGFIX!!");
        nextMelody.Insert(0, id);
        NextSong();
    }

    public void PlayTone(sSimpleTone tone, float volumeMod, float pitchMod)
    {
        if (tone.audioClip != null)
        {
            audioSources[audioSourceIndex].clip = tone.audioClip;
            audioSources[audioSourceIndex].volume = tone.volume * volumeMod;
            audioSources[audioSourceIndex].pitch = Mathf.Clamp(tone.pitch * pitchMod, 0, 3);
            audioSources[audioSourceIndex].Play();
            audioSourceIndex = (audioSourceIndex + 1 < audioSources.Length) ? audioSourceIndex + 1 : 0;
        }
        else
            Debug.Log(gameObject.name + " tried to play empty Tone.");
    }

    /// <summary>
    /// returns the eTones of the index melody
    /// </summary>
    public eTone[] GetBasicMelodieSheet(int melodieID)
    {
        sSimpleNote[] notes = melodies[melodieID].melody;
        eTone[] result = new eTone[notes.Length];
        for (int i = 0; i < notes.Length; i++)
        {
            result[i] = notes[i].tone;
        }
        return result;
    }
}
