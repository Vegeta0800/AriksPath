/// @author: J-D Vbk

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeanEyesBehindBack : MonoBehaviour
{

    Material mat;

    float current;
    const float TACT = 0.1f;

    float previousDotProduct;
    Vector3 dirFromPlayer;

    [SerializeField]
    [Range(0.0f, 150f)]
    float swapAngle = 120f;
    float swapDot;
    //[SerializeField]
    [Range(30.0f, 180f)]
    float maxAngle = 150f;
    //float maxDot;

    //[SerializeField]
    Transform player;
    [SerializeField]
    bool targetNot180turned = false;

    [SerializeField]
    Color baseColor = Color.yellow;
    [SerializeField]
    Color changeColor = Color.red;

    float temp;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(StringCollection.TAG_PLAYER).transform;
        mat = GetComponent<Renderer>().material;
        mat.SetColor(StringCollection.MATERIAL_EmissionColor, baseColor);
        Inputs.onUpdate += OnUpdate;
        /*
        if (swapDot > maxDot)
        {
            temp = swapDot;
            swapDot = maxDot;
            maxDot = temp;
        }
        if (swapDot == maxDot)
            Debug.Log(name.ToUpper() + ": USE DIFFERENT ANGLES!");
        maxDot = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
        */
        swapDot = Mathf.Cos(swapAngle * Mathf.Deg2Rad);
    }

    private void OnUpdate()
    {
        current += Time.deltaTime;
        if (current >= TACT)
        {
            current -= TACT;
            dirFromPlayer = (transform.position - player.position);
            //dirFromPlayer = (player.position - transform.position);
            dirFromPlayer.y = 0;
            dirFromPlayer.Normalize();
            //Debug.Log("Dir:"+dirFromPlayer+" - "+transform.position + "/"+ player.position);
            //Debug.DrawRay(player.position, dirFromPlayer);
            //if (targetNot180turned) dirToPlayer *= -1;
            temp = Vector3.Dot(dirFromPlayer, player.forward);
            //Debug.Log(temp + "/"+minDot+"/"+maxDot);
            if (temp != previousDotProduct)
            {
                if (temp < swapDot && previousDotProduct >= swapDot)
                    mat.SetColor(StringCollection.MATERIAL_EmissionColor, baseColor);
                else if (temp >= swapDot && previousDotProduct < swapDot)
                    mat.SetColor(StringCollection.MATERIAL_EmissionColor, changeColor);
                /*
                else if (temp >= maxDot && previousDotProduct < maxDot)
                    mat.SetColor(StringCollection.MATERIAL_EmissionColor, changeColor);
                else
                {
                    previousDotProduct = (temp - swapDot) / (maxDot - swapDot); //ugly
                    mat.SetColor(StringCollection.MATERIAL_EmissionColor, (1-previousDotProduct) * changeColor
                                    + previousDotProduct * baseColor);
                }
                */
            }
            previousDotProduct = temp;
        }
    }

    private void OnDestroy()
    {
        Inputs.onUpdate -= OnUpdate;
    }
}
