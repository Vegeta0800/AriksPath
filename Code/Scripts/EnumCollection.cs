/// @author J-D Vbk

public enum eLevel
{
    // if youre adding one change StringCollection as well
    mainMenue,
    altarLevel,
    underTheTree,
    soundBridge,
    trackingSwamp,
    finalScene
}

public enum ePlayerAction
{
    jump,
    interact,
    shift,
    sprint,
    walk, //end sprint
    pause,
    teleport
}

public enum eGameLayer
{
    GhostWorldOnly = 10,
    NormalWorldOnly = 11,
    CameraIgnored = 13,
    Camera = 14
}
