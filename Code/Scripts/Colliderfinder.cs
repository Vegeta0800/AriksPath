using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colliderfinder : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + " <= Waldo ?");
    }

    private void OnCollision(Collider other)
    {
        Debug.Log(other.gameObject.name +" <= Waldo ?");
    }
}
