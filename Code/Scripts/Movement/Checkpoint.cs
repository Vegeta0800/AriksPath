using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == StringCollection.TAG_PLAYER)
        {
            Debug.Log("Checkpoint touched.");
            PlayerPrefs.SetFloat("LastPositionX", other.transform.position.x);
            PlayerPrefs.SetFloat("LastPositionY", other.transform.position.y);
            PlayerPrefs.SetFloat("LastPositionZ", other.transform.position.z);
        }
    }
}
