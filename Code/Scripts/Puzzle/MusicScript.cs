using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Obsolete("Partly replaced by PipeOrgan", true)]
public class MusicScript : MonoBehaviour {

    public float timePerSound = 1;
    public float timeBetweenMelodies = 1.5f;
    public float timeUntilPuzzleSolvedSound = 0.5f;

    public delegate void OnMeldodyFinish(int id);
    public static OnMeldodyFinish onMeldodyFinish;

    [Space(5)]
    public AudioManager aud;

    [SerializeField] private BoxCollider box;
    [SerializeField] private Material door;
    [SerializeField] private float duration;
    [SerializeField] private float duration2;

    private bool doTransition;
    private bool part1;

    private float t = 0.0f;

    #region FirstMelodyDec

    [Header("First Melody")]
    private List<bool> fMeloBools = new List<bool>();
    //private List<bool> fBools = new List<bool>();
    int rightElements;
    private List<string> fMelodies = new List<string>();
    [SerializeField] private List<string> fMelodiesReal;
    private int fMeloLength = 4;
    private int hiddentries = 0;

    #endregion

    #region SecondMelodyDec

    [Header("First Melody")]
    private List<bool> sMeloBools = new List<bool>();
    private List<bool> sBools = new List<bool>();
    private List<string> sMelodies = new List<string>();
    [SerializeField] private List<string> sMelodiesReal;
    private int sMeloLength = 6;
    private int hiddentries2 = 0;
    #endregion

    #region ThirdMelodyDec

    [Header("First Melody")]
    private List<bool> tMeloBools = new List<bool>();
    private List<bool> tBools = new List<bool>();
    private List<string> tMelodies = new List<string>();
    [SerializeField] private List<string> tMelodiesReal;
    private int tMeloLength = 8;
    private int hiddentries3 = 0;

    #endregion

    public static MusicScript instance {get; private set;}
    [HideInInspector]
    public PipeMelodySymbols soundSymbolVisualisation;

    void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Two instance of MusicScript:"
                +instance.gameObject.name+"/"+this.gameObject.name);
        instance = this;
    }

    void OnEnable ()
    {
        Inputs.onUpdate += OnUpdate;
        CollisionScript.onPlateStepped += Conversion;
	}

    private void Conversion(int id, string melo, int melod)
    {

        Debug.Log(melod);

        if(melod == 1)
        {
            FirstMelody(id, melo);
        }
        else if(melod == 2)
        {
            SecondMelody(id, melo);
        }
        else if(melod == 3)
        {
            ThirdMelody(id, melo);
        }
        else if(melod == 4)
        {
            StartCoroutine(PlayMelo(1));
        }
        else if (melod == 5)
        {
            StartCoroutine(PlayMelo(2));
        }
        else if (melod == 6)
        {
            StartCoroutine(PlayMelo(3));
        }
    }

    /// <summary>
    /// Plays the actual melody
    /// </summary>
    private IEnumerator PlayMelo(int id)
    {
        if(id == 1)
        {
            foreach (string s in fMelodiesReal)
            {
                aud.Play(s, false, false);
                //soundSymbolVisualisation.ShowSymbol(StringToSoundEnum(s));
                yield return new WaitForSeconds(timePerSound);
            }
        }
        else if (id == 2)
        {
            foreach (string s in sMelodiesReal)
            {
                aud.Play(s, false, false);

                //soundSymbolVisualisation.ShowSymbol(StringToSoundEnum(s));
                yield return new WaitForSeconds(timePerSound);
            }
        }
        else if (id == 3)
        {
            foreach (string s in tMelodiesReal)
            {
                aud.Play(s, false, false);

                //soundSymbolVisualisation.ShowSymbol(StringToSoundEnum(s));
                yield return new WaitForSeconds(timePerSound);
            }
        }
        soundSymbolVisualisation.HideSymbol();
    }

    #region FirstMelody

    private void FirstMelody(int i, string melo)
    {
        hiddentries += 1;

        aud.Play(melo, false, false);

        if (i == fMeloBools.Count + 1)
        {
            fMeloBools.Add(true);
            fMelodies.Add(melo);
        }
        else
        {
            fMeloBools.Add(false);
            fMelodies.Add(melo);
        }

        if (hiddentries > fMeloLength)
        {
            StartCoroutine(Reset());
            return;
        }

        if (fMeloBools.Count == fMeloLength)
        {
            StartCoroutine(Reset());
        }
    }

    /// <summary>
    /// plays player created melody and correct version
    /// </summary>
    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);

        foreach (string s in fMelodies)
        {
            aud.Play(s, false, false);
            yield return new WaitForSeconds(timePerSound);
        }

        yield return new WaitForSeconds(timeBetweenMelodies);

        foreach (string s in fMelodiesReal)
        {
            aud.Play(s, false, false);
            yield return new WaitForSeconds(timePerSound);
        }

        yield return new WaitForSeconds(timeUntilPuzzleSolvedSound);

        foreach(bool b in fMeloBools)
        {
            if(b == true)
            {
                //Debug.Log("E");
                //fBools.Add(true);
                rightElements++;
            }
            else
            {
                break;
            }
        }


        //if (rightElements fBools.Count == fMeloLength)
        if (rightElements == fMeloLength)
        {
            aud.Play("puzzleComplete", false, false);

            if (onMeldodyFinish != null)
            {
                onMeldodyFinish(1);
            }
        }
        else
        {
            aud.Play("statuePlace", false, false);

            if (onMeldodyFinish != null)
            {
                onMeldodyFinish(0);
            }
        }

        hiddentries = 0;
        fMeloBools.Clear();
        fMelodies.Clear();
        //fBools.Clear();
        rightElements = 0;
    }
    #endregion

    #region SecondMelody

    private void SecondMelody(int i, string melo)
    {
        hiddentries2 += 1;

        aud.Play(melo, false, false);

        if (i == sMeloBools.Count + 1)
        {
            sMeloBools.Add(true);
            sMelodies.Add(melo);
        }
        else
        {
            sMeloBools.Add(false);
            sMelodies.Add(melo);
        }

        if (hiddentries2 > sMeloLength)
        {
            StartCoroutine(Reset2());
            return;
        }

        if (sMeloBools.Count == sMeloLength)
        {
            StartCoroutine(Reset2());
        }
    }

    private IEnumerator Reset2()
    {
        yield return new WaitForSeconds(1f);

        foreach (string s in sMelodies)
        {
            aud.Play(s, false, false);
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(2f);

        foreach (string s in sMelodiesReal)
        {
            aud.Play(s, false, false);
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);

        foreach (bool b in sMeloBools)
        {
            if (b == true)
            {
                sBools.Add(true);
            }
            else
            {
                break;
            }
        }


        if (sBools.Count == sMeloLength)
        {
            aud.Play("puzzleComplete", false, false);

            if (onMeldodyFinish != null)
            {
                onMeldodyFinish(2);
            }
        }
        else
        {
            aud.Play("statuePlace", false, false);

            if (onMeldodyFinish != null)
            {
                onMeldodyFinish(0);
            }
        }

        hiddentries2 = 0;
        sMeloBools.Clear();
        sMelodies.Clear();
        sBools.Clear();
    }
    #endregion

    #region ThirdMelody

    private void ThirdMelody(int i, string melo)
    {
        hiddentries3 += 1;

        aud.Play(melo, false, false);

        if (i == tMeloBools.Count + 1)
        {
            tMeloBools.Add(true);
            tMelodies.Add(melo);
        }
        else
        {
            tMeloBools.Add(false);
            tMelodies.Add(melo);
        }

        if (hiddentries3 > tMeloLength)
        {
            StartCoroutine(Reset3());
            return;
        }

        if (tMeloBools.Count == tMeloLength)
        {
            StartCoroutine(Reset3());
        }
    }

    private IEnumerator Reset3()
    {
        foreach (string s in tMelodies)
        {
            aud.Play(s, false, false);
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(2f);

        foreach (string s in tMelodiesReal)
        {
            aud.Play(s, false, false);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);

        foreach (bool b in tMeloBools)
        {
            if (b == true)
            {
                tBools.Add(true);
            }
            else
            {
                break;
            }
        }


        if (tBools.Count == tMeloLength)
        {
            aud.Play("puzzleComplete", false, false);

            if (onMeldodyFinish != null)
            {
                onMeldodyFinish(3);
            }
        }
        else
        {
            aud.Play("statuePlace", false, false);

            if (onMeldodyFinish != null)
            {
                onMeldodyFinish(0);
            }
        }

        hiddentries3 = 0;
        tMeloBools.Clear();
        tMelodies.Clear();
        tBools.Clear();
    }
    #endregion

    private void OnUpdate()
    {

        if (doTransition == true)
        {
            t += 1.0f / duration * Time.deltaTime;

            box.enabled = false;
            door.SetFloat("_Opacity_Clip", Mathf.SmoothStep(0.5f, 1.1f, t));

            if (door.GetFloat("_Opacity_Clip") == 1.1f)
            {
                t = 0.0f;
                doTransition = false;
                aud.Play("puzzleComplete", false, false);

            }
        }

        if (part1 == true)
        {
            t += 1.0f / duration2 * Time.deltaTime;

            door.SetFloat("_emission_amount", Mathf.SmoothStep(0f, 3f, t));

            if (door.GetFloat("_emission_amount") == 3f)
            {
                t = 0.0f;
                part1 = false;
                doTransition = true;
            }
        }
    }

    public static eTone StringToSoundEnum(string sound)
    {
        if (sound.Equals("firstSound"))
            return eTone.C3;
        else if (sound.Equals("secondSound"))
            return eTone.A3;
        else if (sound.Equals("thirdSound"))
            return eTone.B3;
        else if (sound.Equals("fourthSound"))
            return eTone.D3;
        else return eTone.Break;
    }

    void OnDisable()
    {
        Inputs.onUpdate -= OnUpdate;
        CollisionScript.onPlateStepped -= Conversion;

    }
}
