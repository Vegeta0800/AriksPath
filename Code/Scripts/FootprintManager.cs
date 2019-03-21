using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FootprintManager : MonoBehaviour
{

    [SerializeField]
    float activateDistance = 10.0f;

    Material[] footPrints;
    Renderer[] rend;

    const float TACT = 0.1f;
    float elapsed = 0;

    [SerializeField]
    Transform[] revealers = new Transform[0];

    void OnEnable()
    {
        Inputs.onUpdate += OnUpdate;
    }

    // Use this for initialization
    void Start()
    {
        rend = GetComponentsInChildren<Renderer>();
        footPrints = new Material[rend.Length];
        for (int i = 0; i < footPrints.Length; i++)
        {
            footPrints[i] = rend[i].material;
            rend[i].enabled = false;
        }
        /*
        if (footPrints != null)
        {
            Debug.Log("Bing:" + footPrints.Length+"/"+rend.Length);
        }
        else
            Debug.Log("MEEP");
            */
    }

    void OnUpdate()
    {
        Scene scene = SceneManager.GetActiveScene();


        if (scene.name == "Endlevel")
        {

            elapsed += Time.deltaTime;
            if (elapsed >= TACT)
            {
                float distance = float.MaxValue;
                float minDistance;
                for (int i = 0; i < rend.Length; i++)
                {
                    minDistance = float.MaxValue;
                    for (int j = 0; j < revealers.Length; j++)
                    {
                        distance = Vector3.Distance(rend[i].transform.position,
                                                    revealers[j].transform.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            //Debug.DrawLine(rend[i].transform.position,
                            //                        revealers[j].transform.position);
                            //Debug.Log(distance);
                        }
                    }

                    if (distance <= activateDistance)
                    {
                        if (!rend[i].enabled)
                            rend[i].enabled = true;
                    }
                    else
                    {
                        if (rend[i].enabled)
                            rend[i].enabled = false;
                    }
                }
                elapsed -= TACT;
            }
        }

    }

    void OnDisable()
    {
        Inputs.onUpdate -= OnUpdate;
    }

}
