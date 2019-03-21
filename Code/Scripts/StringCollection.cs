using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringCollection {

    // LEVELNAMES
    // if youre change something do it in eLevel (EnumCollection) as well
    public const string LEVEL_mainMenue = "Main Menue";
    public const string LEVEL_altarLevel = "Level 1";
    public const string LEVEL_underTheTree = "Grove";
    public const string LEVEL_soundBridge = "Musiclevel";
    public const string LEVEL_trackingSwamp = "Endlevel";
    public const string LEVEL_finalScene = "FinalScene"; //in development

    #region Input
    // INPUT Axis
    public const string INPUT_MOVEMENT_HORIZONTAL = "Horizontal";
    public const string INPUT_MOVEMENT_VERTICAL = "Vertical";

    // INPUT Button
    public const string INPUT_JUMP = "Jump";
    public const string INPUT_INTERACT = "Interact";
    public const string INPUT_SWITCH_DIMENSION = "SwitchDimension";
    public const string INPUT_TELEPORT = "Teleport";
    public const string INPUT_PAUSE = "Cancel";

    // INPUT Camera
    public const string INPUT_CAMERA_HORIZONTAL = "Camera_Horizontal";
    public const string INPUT_CAMERA_VERTICAL = "Camera_Vertical";


    // INPUT Sprint
    public const string INPUT_SPRINTController = "SprintAxis";
    public const string INPUT_SPRINTKeyBoard = "SprintButton";
    #endregion Input

    // TAGS
    public const string TAG_PLAYER = "Player";
    public const string TAG_MAINCAMERA = "MainCamera";

    // MATERIALS
    public const string MATERIAL_EmissionColor = "_Emission_Color";
}
