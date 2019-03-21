/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TriggerSound : MonoBehaviour
{
    [SerializeField]
    AudioSource sound;

    [SerializeField]
    bool playerOnly = true;
    [Space(5)]
    [SerializeField]
    bool fireOnlyOnce = true;
    bool firedAlready;
    [SerializeField]
    public bool destroyGameObjectAfterUse = false;
    public AudioMixerGroup audioMixer;
    private void OnTriggerEnter(Collider other)
    {
        if (!firedAlready)
        {
            if (!playerOnly || other.tag == StringCollection.TAG_PLAYER)
            {
                PlaySound();
                if (fireOnlyOnce)
                {
                    firedAlready = true;
                    if (sound && sound.clip != null)
                        StartCoroutine(DelayedRemove(sound.clip.length));
                }
            }
        }
    }

    private void PlaySound()
    {
        if (sound == null)
            sound.GetComponent<AudioSource>();
        Debug.Log(name + " attemps to play Sound.");
        if (sound && sound.clip != null && !sound.isPlaying)
            sound.Play();
    }

    private IEnumerator DelayedRemove(float finishedAfter)
    {
        yield return new WaitForSeconds(finishedAfter);
        Remove();
    }

    public void Remove()
    {
        if (fireOnlyOnce)
        {
            //Debug.Log("Remove");
            if (destroyGameObjectAfterUse)
            {
                //Debug.Log("Remove all");
                Destroy(this.gameObject);
            }
            else
            {
                //Debug.Log("RemoveComponent");
                Destroy(this);
            }
        }
    }
}
