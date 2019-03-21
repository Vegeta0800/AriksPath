using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSymbolPositioning : MonoBehaviour
{
    [SerializeField]
    float visibilityDistance = 20.0f;

    Renderer rend;
    Transform camTransform;
    Transform playerTransform;

    Vector3 rotation;

    [Header("Optional")]
    [SerializeField]
    Transform uprightAnchor;

    private void OnEnable()
    {
        GameObject tempGO = GameObject.FindGameObjectWithTag(StringCollection.TAG_MAINCAMERA);
        if (tempGO)
            camTransform = tempGO.transform;
        tempGO = GameObject.FindGameObjectWithTag(StringCollection.TAG_PLAYER);
        if (tempGO)
            playerTransform = tempGO.transform;

        rend = GetComponent<Renderer>();
        if (rend && camTransform && playerTransform)
            Inputs.onUpdate += OnUpdate;
#if UNITY_EDITOR
        else
            Debug.Log(this.name + " couldn't find Renderer, Camera or Player.");
#endif

    }

    private void OnUpdate()
    {
            if (uprightAnchor)
            {
                rotation = camTransform.eulerAngles;
                /*
                tempVec = camTransform.forward;
                turnRotation = (Mathf.Atan2(-tempVec.z, ;
                */
                rotation.x = 0;
                rotation.z = 0;

                uprightAnchor.eulerAngles = rotation;
            }

            if (Vector3.Distance(playerTransform.position, this.transform.position) <= visibilityDistance)
            {
                rend.enabled = true;
                this.transform.rotation = camTransform.rotation;
            }
            else
            {
                rend.enabled = false;
            }
    }

    private void OnDisable()
    {
        if (rend)
            Inputs.onUpdate -= OnUpdate;
    }
}
