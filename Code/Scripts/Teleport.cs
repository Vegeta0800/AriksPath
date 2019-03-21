using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    public static Teleport instance {get; private set;}

    [SerializeField]
    string wildCardLevel = "";

    [Header("Within this Level")]
    [SerializeField]
    [Tooltip("Player if empty.")]
    Transform teleportedElement;
    [SerializeField]
    Transform[] teleportPositions = new Transform[0];
    [SerializeField]
    private int nextInSceneTeleportIndex = 0;

    static bool loadingLevel = false;

    private void Awake()
    {
        if (instance != null)
            Debug.Log("Two Teleporters: " + instance.name + "/" + this.name);
        instance = this;
    }

    private void OnEnable()
    {
        Inputs.playerTriggeredAction += TeleportInScene;
    }

    /// <summary>
    /// move within a scene
    /// </summary>
    private void TeleportInScene(ePlayerAction action)
    {
        if (action != ePlayerAction.teleport) return;

        if (instance == null || instance.teleportPositions.Length < 1) return;
        instance.InstanceTeleportInScene();
    }
    /// <summary>
    /// the instance specific logic behind in scene teleports
    /// </summary>
    private void InstanceTeleportInScene()
    {
        if (nextInSceneTeleportIndex >= teleportPositions.Length)
        {
            nextInSceneTeleportIndex = 0;
        }
        if (teleportPositions[nextInSceneTeleportIndex] != null)
        {
            if (teleportedElement == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(StringCollection.TAG_PLAYER);
                if (player)
                    teleportedElement = player.transform;
                else
                {
                    Debug.Log(gameObject.scene + ": Teleport("
                                + gameObject.name + ") found nothing with Player-Tag!");
                    return;
                }
            }

            teleportedElement.position = teleportPositions[nextInSceneTeleportIndex].position;
            teleportedElement.rotation = teleportPositions[nextInSceneTeleportIndex].rotation;
        }
        nextInSceneTeleportIndex++;
    }

    /// <summary>
    /// move to another scene
    /// </summary>
    /// <param name="levelID"></param>
    public static void TeleportToScene(eLevel levelID)
    {
        if (loadingLevel) return;
        loadingLevel = true;
        string nextLoaded = GetLevelString(levelID);
        if (nextLoaded.Length < 1)
            Debug.Log("Teleport to " + levelID + "isn't implemented.");
        StartLoading(nextLoaded);
    }

    /// <summary>
    /// try to load the level given to instance by name
    /// </summary>
    public static void WildCard()
    {
        if (loadingLevel) return;
        loadingLevel = true;
        if (instance == null)
        {
            Debug.Log("No TeleportScript placed.");
            return;
        }
        instance.wildCardLevel.Trim();
        if (instance.wildCardLevel.Length > 0)
        {
            StartLoading(instance.wildCardLevel);
        }
        else
            Debug.Log(instance.gameObject.name
                         + " tried to load WildCard without SceneName.");
    }

    public static string GetLevelString(eLevel levelID)
    {
        string nextLoaded = "";
        switch (levelID)
        {
            case eLevel.altarLevel:
                nextLoaded = StringCollection.LEVEL_altarLevel;
                break;
            case eLevel.finalScene:
                nextLoaded = StringCollection.LEVEL_finalScene;
                break;
            case eLevel.mainMenue:
                nextLoaded = StringCollection.LEVEL_mainMenue;
                break;
            case eLevel.soundBridge:
                nextLoaded = StringCollection.LEVEL_soundBridge;
                break;
            case eLevel.trackingSwamp:
                nextLoaded = StringCollection.LEVEL_trackingSwamp;
                break;
            case eLevel.underTheTree:
                nextLoaded = StringCollection.LEVEL_underTheTree;
                break;
            default:
                Debug.Log("Doesn't know " +levelID+" yet.");
                break;
        }
        return nextLoaded;
    }

    public static eLevel GetLevelEnum(string entry)
    {
        if (entry == StringCollection.LEVEL_altarLevel)
            return eLevel.altarLevel;
        else if (entry == StringCollection.LEVEL_finalScene)
            return eLevel.finalScene;
        if (entry == StringCollection.LEVEL_mainMenue)
            return eLevel.mainMenue;
        if (entry == StringCollection.LEVEL_soundBridge)
            return eLevel.soundBridge;
        if (entry == StringCollection.LEVEL_trackingSwamp)
            return eLevel.trackingSwamp;
        if (entry == StringCollection.LEVEL_underTheTree)
            return eLevel.underTheTree;
        else
        {
            Debug.Log("Teleport doesn't know " + entry + " yet.");
            return eLevel.mainMenue;
        }
    }

    private static void StartLoading(string nextLoaded)
    {
        Debug.Log("...tries to load Scene: " + nextLoaded);
        SceneManager.LoadScene(nextLoaded);
        loadingLevel = false;
    }

    private void OnDisable()
    {
        Inputs.playerTriggeredAction -= TeleportInScene;
    }
}
