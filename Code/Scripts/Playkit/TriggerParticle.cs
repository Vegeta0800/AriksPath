/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerParticle : MonoBehaviour {

    [SerializeField]
    ParticleSystem triggeredEffect;
    // unity access logic, yeah...
    ParticleSystem.EmissionModule emissionPart;
    [Space(5)]
    [SerializeField]
    float startDelay = 1;
    [SerializeField]
    [Tooltip("Ignored, if < 1")]
    float endAfter = 0;
    [Space(5)]
    [SerializeField]
    [Tooltip("Ignored, if < 1")]
    float newEmissionRateOverTime;

    bool alreadyWaiting = false;
    [Space(10)]
    public bool useCollider = true;
    [SerializeField]
    bool playerOnly = true;
    [Space(5)]
    [SerializeField]
    bool fireOnlyOnce = true;
    public bool destroyGameObjectAfterUse = false;

    private IEnumerator DelayedStart(float startAfterTime, float endAfterTime)
    {
        yield return new WaitForSeconds(startAfterTime);
        StartEvent();
        if (endAfter > 0)
        {
            yield return new WaitForSeconds(endAfterTime);
            EndEvent();
            alreadyWaiting = false;
        }
        else if (fireOnlyOnce)
            Remove();
    }

    private void StartEvent()
    {
        emissionPart = triggeredEffect.emission;
        if (newEmissionRateOverTime > 0)
            emissionPart.rateOverTime = newEmissionRateOverTime;
        emissionPart.enabled = true;
    }

    private void EndEvent()
    {
        emissionPart.enabled = false;
        Remove();
    }

    private void OnTriggerEnter(Collider co)
    {
        //Debug.Log("Can touch this!");
        if (!playerOnly ||co.tag == "Player")
        {
            if (useCollider)
            {
                if (!alreadyWaiting)
                {
                    if (triggeredEffect)
                    {
                        Debug.Log(this.gameObject.name + " triggered Particle.");
                        StartCoroutine(DelayedStart(startDelay, endAfter));
                    }
                }
                alreadyWaiting = true;
            }
        }
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
        else
            alreadyWaiting = false;
    }
}
