/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneChange : MonoBehaviour {

    [HideInInspector]
    public eLevel originLevel;
    [Space(5)]
    [SerializeField]
    eLevel nextLevel;
    [Space(5)]
    [SerializeField]
    [Tooltip("Played while level is loaded.")]
    AudioSource elevatorWaitingMusic;
    [SerializeField]
    [Tooltip("Played after Level was loaded.")]
    AudioSource elevatorArrivingDing;

    [Space(5)]
    [SerializeField]
    float dingDelay = 1.0f;
    float destroyAfterDing = 0f;

    string targetSceneName;

    public static TriggerSceneChange instance { get; private set; }

    [Header("Fade Out")]
    //[Range(0.0f, 1.0f)]
    //[SerializeField]
    //float maxBlack = 1f;
    [SerializeField]
    float secondsToBlack = 0.2f;
    bool blackout;
    Texture2D blk;
    [SerializeField]
    Color black = Color.black;

    [Header("Fade In")]
    [SerializeField]
    public float FadeInSecondsBlack = 0f;
    [SerializeField]
    public float FadeInSecondsFromBlack = 0.2f;

    private void OnEnable()
    {
        originLevel = Teleport.GetLevelEnum(gameObject.scene.name);
        SceneManager.sceneLoaded += OnSceneLoaded;
        targetSceneName = Teleport.GetLevelString(nextLevel);

        blk = new Texture2D(1, 1);
    }

    public void SceneShift()
    {
        instance = this;
        if (elevatorWaitingMusic && elevatorWaitingMusic.clip != null)
            elevatorWaitingMusic.Play();
        StartCoroutine(FadeOut(secondsToBlack));

    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(gameObject.name + " starts loading: " + nextLevel);

        DontDestroyOnLoad(this);
        SceneShift();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == targetSceneName)
        {
            StartCoroutine(DelayedEnd());
            StartCoroutine(FadeIn(FadeInSecondsBlack, FadeInSecondsFromBlack));
        }
    }

    private IEnumerator DelayedEnd()
    {
        yield return new WaitForSeconds(dingDelay);
        if (elevatorArrivingDing && elevatorArrivingDing.clip != null)
        {
            elevatorArrivingDing.Play();
            destroyAfterDing = elevatorArrivingDing.clip.length;
        }
        yield return new WaitForSeconds(destroyAfterDing);
        instance = null;
        Destroy(this.gameObject);
    }

    private IEnumerator FadeOut(float _secondsToBlack)
    {
        blackout = true;
        float elapsed = 0;
        while (elapsed <= _secondsToBlack)
        {
            black.a = elapsed / _secondsToBlack;
            blk.SetPixel(0, 0, black);
            blk.Apply();
            elapsed += Time.deltaTime;
            yield return null;
        }
        black.a = 1.0f;
        blk.SetPixel(0, 0, black);
        blk.Apply();
        Teleport.TeleportToScene(nextLevel);
    }

    private IEnumerator FadeIn(float _secondsBlack, float _secondsFromBlack)
    {
        blackout = true;
        float elapsed = 0;
        black.a = 1.0f;
        blk.SetPixel(0, 0, black);
        blk.Apply();
        yield return new WaitForSeconds(_secondsBlack);

        while (elapsed <= _secondsFromBlack)
        {
            black.a = 1 - elapsed / _secondsFromBlack;
            blk.SetPixel(0, 0, black);
            blk.Apply();
            elapsed += Time.deltaTime;
            yield return null;
        }
        blackout = false;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnGUI()
    {
        if (blackout) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blk);
    }
}
