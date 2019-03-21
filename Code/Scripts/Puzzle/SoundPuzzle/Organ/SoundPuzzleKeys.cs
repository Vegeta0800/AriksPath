/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPuzzleKeys : SoundPuzzlePressurePlate
{
    [Header("Played Note")]
    [SerializeField]
    sSimpleNote myNote;
    
    /// <summary>
    /// system isn't deaf and sends reaction
    /// </summary>
    protected override void Reaction()
    {
        if (quizMaster && !quizMaster.busy)
        {
            quizMaster.PressedKey(myNote);
        }

        base.Reaction();
    }
}
