using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundPuzzleSolver {

    void PuzzleStepSolved(int melodyID);

    void PuzzleCompletelySolved();
}
