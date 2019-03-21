using UnityEngine;

public class Inputs : MonoBehaviour
{

    //public delegate void OnKeyPressedDelegate(KeyCode keyCode);
    //public delegate void OnKeyHoldingDelegate(KeyCode keyCode);
    //public delegate void OnKeyReleaseDelegate(KeyCode keyCode);
    //public delegate void OnAxisInputDelegate(float value, uint id);
    public delegate void OnUpdateDelegate();

    //public static OnKeyPressedDelegate onKeyPressed;
    //public static OnKeyHoldingDelegate onKeyHolding;
    //public static OnKeyReleaseDelegate onKeyRelease;
    //public static OnAxisInputDelegate onAxisInput;
    public static OnUpdateDelegate onUpdate;

    /// <summary>
    /// replaces on  key pressed
    /// </summary>
    public static System.Action<ePlayerAction> playerTriggeredAction;

    /// <summary>
    /// the movement input for the character
    /// </summary>
    public static System.Action<float, float> playerMovement;

    /// <summary>
    /// the movement input for the camera
    /// </summary>
    public static System.Action<float, float> cameraMovement;

    private bool noUpdate;

    [Header("Level Teleport")]
    [SerializeField]
    KeyCode mainMenue = KeyCode.F1;
    [SerializeField]
    KeyCode altarLevel = KeyCode.F2;
    [SerializeField]
    KeyCode underTheTree = KeyCode.F3;
    [SerializeField]
    KeyCode soundBridge = KeyCode.F4;
    [SerializeField]
    KeyCode trackingSwamp = KeyCode.F5;
    [SerializeField]
    KeyCode finalScene = KeyCode.F6;
    [SerializeField]
    KeyCode wildCardLevel = KeyCode.F7;

    float tempFloat;
    float previousRightTrigger;


    private void OnEnable()
    {
        SwitchScript.onPauseMenu += PauseMenu;
    }

    private void Update()
    {
        #region Pause
        if (Input.GetButtonDown(StringCollection.INPUT_PAUSE))
        {
            if (playerTriggeredAction != null)
                playerTriggeredAction(ePlayerAction.pause);
        }
        #endregion

        if (noUpdate == false)
        {
            #region Character Movement
            if (playerMovement != null)
            {
                playerMovement(Input.GetAxis(StringCollection.INPUT_MOVEMENT_HORIZONTAL),
                    Input.GetAxis(StringCollection.INPUT_MOVEMENT_VERTICAL));
            }
            #endregion

            #region Camera Movement
            if (cameraMovement != null)
            {/*
                float x = Input.GetAxis(StringCollection.INPUT_CAMERA_HORIZONTALController);
                if (x == 0)
                    x = Input.GetAxis(StringCollection.INPUT_CAMERA_HORIZONTALController);
                float y = */
                //Debug.Log(Input.GetAxis(StringCollection.INPUT_CAMERA_HORIZONTALController) +"/"+
                //    Input.GetAxis(StringCollection.INPUT_CAMERA_VERTICALController));
                cameraMovement(Input.GetAxis(StringCollection.INPUT_CAMERA_HORIZONTAL),
                    Input.GetAxis(StringCollection.INPUT_CAMERA_VERTICAL));
            }
            #endregion

             #region Player Triggered Actions

            if (playerTriggeredAction != null)
            {
                #region Jump
                if (Input.GetButtonDown(StringCollection.INPUT_JUMP))
                {
                    playerTriggeredAction(ePlayerAction.jump);
                }
                #endregion

                #region Interact
                if (Input.GetButtonDown(StringCollection.INPUT_INTERACT))
                {
                    playerTriggeredAction(ePlayerAction.interact);
                }
                #endregion

                #region Shift Dimension
                if (Input.GetButtonDown(StringCollection.INPUT_SWITCH_DIMENSION))
                {
                    if (Inventory.OwnsMask || Inventory.instance.alwaysOwnsMaskHere)
                        playerTriggeredAction(ePlayerAction.shift);
                }
                #endregion

                #region Sprint & Walk

                // Sprint / Walk
                tempFloat = Input.GetAxis(StringCollection.INPUT_SPRINTController);
                if (tempFloat != previousRightTrigger)
                {
                    if (tempFloat < 0.5 && previousRightTrigger > 0.5)
                        playerTriggeredAction(ePlayerAction.walk);

                    else if (tempFloat > 0.5 && previousRightTrigger < 0.5)
                        playerTriggeredAction(ePlayerAction.sprint);

                    previousRightTrigger = tempFloat;
                }
                else if (Input.GetButtonDown(StringCollection.INPUT_SPRINTKeyBoard))
                {
                    playerTriggeredAction(ePlayerAction.sprint);
                }
                else if (Input.GetButtonUp(StringCollection.INPUT_SPRINTKeyBoard))
                {
                    playerTriggeredAction(ePlayerAction.walk);
                }

                #endregion Sprinting

                #region Teleport
                if (Input.GetButtonDown(StringCollection.INPUT_TELEPORT))
                {
                    playerTriggeredAction(ePlayerAction.teleport);
                }
                #endregion

            }
            #endregion PlayerTriggered Actions

            #region Level Teleports

            if (Input.GetKeyDown(mainMenue))
            {
                Teleport.TeleportToScene(eLevel.mainMenue);
            }
            else if (Input.GetKeyDown(altarLevel))
            {
                Teleport.TeleportToScene(eLevel.altarLevel);
            }
            else if (Input.GetKeyDown(underTheTree))
            {
                Teleport.TeleportToScene(eLevel.underTheTree);
            }
            else if (Input.GetKeyDown(soundBridge))
            {
                Teleport.TeleportToScene(eLevel.soundBridge);
            }
            else if (Input.GetKeyDown(trackingSwamp))
            {
                Teleport.TeleportToScene(eLevel.trackingSwamp);
            }
            else if (Input.GetKeyDown(finalScene))
            {
                Teleport.TeleportToScene(eLevel.finalScene);
            }
            else if (Input.GetKeyDown(wildCardLevel))
            {
                Teleport.WildCard();
            }

            if (Input.GetKey(KeyCode.Joystick1Button6))
            {
                if (Input.GetKey(KeyCode.Joystick1Button0))
                    Teleport.TeleportToScene(eLevel.altarLevel);

                if (Input.GetKey(KeyCode.Joystick1Button1))
                    Teleport.TeleportToScene(eLevel.underTheTree);

                if (Input.GetKey(KeyCode.Joystick1Button3))
                    Teleport.TeleportToScene(eLevel.soundBridge);

                if (Input.GetKey(KeyCode.Joystick1Button2))
                    Teleport.TeleportToScene(eLevel.trackingSwamp);

                if (Input.GetKey(KeyCode.Joystick1Button5))
                    Teleport.TeleportToScene(eLevel.finalScene);
            }

            #endregion

            #region Update

            if (onUpdate != null)
            {
                onUpdate();
            }

            #endregion
        }
    }

    private void PauseMenu(bool paused)
    {
        noUpdate = paused;
    }

    private void OnDisable()
    {
        SwitchScript.onPauseMenu -= PauseMenu;
    }
}
