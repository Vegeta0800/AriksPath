using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPuzzleBridgeManager : MonoBehaviour, ISoundPuzzleSolver
{

    [SerializeField]
    SoundPuzzleBridgeRepair[] bridges = new SoundPuzzleBridgeRepair[0];

    [SerializeField]
    int[] bridgeSolveID;

    int step = 0;

    void Start()
    {
        if (bridgeSolveID.Length != bridges.Length)
        {
            Debug.Log("BridgeManager("+name + "): Bridges & IDs have diffent length");
        }
    }

    public void PuzzleStepSolved(int melodyID)
    {
        //Debug.Break();
        //Debug.Log("Step!");
        if (step < bridges.Length)
        {
            if (melodyID == bridgeSolveID[step])
            {
                bridges[step].RepairBridge();
                step++;
            }
            if (step == bridges.Length)
            {
                PuzzleCompletelySolved();
            }
        }
    }

    /// <summary>
    /// called if puzzle was completed
    /// </summary>
    public void PuzzleCompletelySolved()
    {
        //TODO: FREUEN
        Debug.Log(gameObject.name + ":" + this.name + "solved.");
    }
}
