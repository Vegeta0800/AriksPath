using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraPan {

    //System.Action reachedTarget;

    void LockAndMove();

    void LockAndMove(float watchingDuration);
}
