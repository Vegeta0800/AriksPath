using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneGlow : MonoBehaviour
{
    [SerializeField]
    GameObject[] affectedObjects = new GameObject[0];

    [SerializeField]
    bool playerOnly = true;
    [Space(5)]
    [SerializeField]
    bool fireOnlyOnce = false;
    bool firedAlready = false;
    [SerializeField]
    public bool destroyGameObjectAfterUse = false;

    [Header("Shaderforge (optional)")]
    [SerializeField]
    bool shaderForgeMaterial = false;
    [SerializeField]
    string SFEmissionVariableName;

    [Header("Behaviour")]
    [SerializeField]
    float warmUp = 0.5f;
    [SerializeField]
    float coolDown = 0.5f;
    [Space(5)]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float baseEmission = 0.25f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float maxEmission = 1.0f;

    List<Material> affectedMaterials = new List<Material>();
    List<Color> baseColor;

    float shaderPropertyID;

    bool active = false;
    bool windup = false;
    float current = 0;
    Coroutine change;

    private void OnEnable()
    {
        if (warmUp <= 0) warmUp = 0.001f;
        if (coolDown <= 0) coolDown = 0.001f;

        if (!shaderForgeMaterial)
            shaderPropertyID = Shader.PropertyToID("_EmissionColor");
        else
        {
            SFEmissionVariableName = "_"+ SFEmissionVariableName.Trim();
               shaderPropertyID = Shader.PropertyToID(SFEmissionVariableName);
        }
        if (affectedObjects.Length < 1)
            Debug.Log(name + " ZoneGlow doesn't contain any targets.");


        if (!shaderForgeMaterial)
            baseColor = new List<Color>();

            for (int i = 0; i < affectedObjects.Length; i++)
        {

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                foreach (Renderer randy in renderers)
                {
                    foreach (Material matty in randy.materials)
                    {
                        //
                        //if (matty.name == "GlowOrange" || matty.name == "GlowOrange (Instance)"
                        //    ||matty.name == "Mushroom Big II" || matty.name == "Mushroom Big II (Instance)"
                        //    )
                        //{
                        //
                        affectedMaterials.Add(matty);
                        if (!shaderForgeMaterial)
                            baseColor.Add(matty.GetColor("_EmissionColor"));
                        //}
                    }
                }
            }
        }
        if (affectedMaterials.Count < 1)
        {
            Debug.Log(gameObject.name + " no matching Material");
            Destroy(this);
        }
        for(int i = 0; i < affectedMaterials.Count; i++)
        {
            if (!shaderForgeMaterial)
                affectedMaterials[i].SetColor("_EmissionColor", baseColor[i] * Mathf.LinearToGammaSpace(baseEmission));
            else
                affectedMaterials[i].SetFloat(SFEmissionVariableName, baseEmission);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (change != null)
        {
            if (!windup)
            {
                StopCoroutine(change);
                windup = true;
                StartCoroutine(ChangeGlow());
            }
        }
        else if (!active)
        {
            windup = true;
            StartCoroutine(ChangeGlow());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (change != null)
        {
            if (windup)
            {
                StopCoroutine(change);
                windup = false;
                StartCoroutine(ChangeGlow());
            }
        }
        else if (active)
        {
            windup = false;
            StartCoroutine(ChangeGlow());
        }
    }

    private IEnumerator ChangeGlow()
    {
        float duration;
        if (windup)
        {
            duration = warmUp;
            current = 0;
        }
        else
        {
            duration = coolDown;
            current = duration;
        }
        Debug.Log("active: "+active+" windup: "+windup+" - "+current+"/"+duration);

        float emission;
        {
            float glow;
            float shift = maxEmission - baseEmission;
            while (current >= 0 && current <= duration)
            {
                glow = (1 - Mathf.Cos(current / duration * Mathf.PI)) / 2;
                emission = baseEmission + glow * shift;

                for (int i = 0; i < affectedMaterials.Count; i++)
                {
                    if (!shaderForgeMaterial)
                        affectedMaterials[i].SetColor("_EmissionColor", baseColor[i] * Mathf.LinearToGammaSpace(emission));
                    else
                        affectedMaterials[i].SetFloat(SFEmissionVariableName, emission);
                }

                current += (windup) ? Time.deltaTime : -Time.deltaTime;
                yield return null;
            }
        }

        active = (windup) ? true: false;
        emission = (active) ? maxEmission : baseEmission;
        for (int i = 0; i < affectedMaterials.Count; i++)
        {
            if (!shaderForgeMaterial)
                affectedMaterials[i].SetColor("_EmissionColor", baseColor[i] * Mathf.LinearToGammaSpace(emission));
            else
                affectedMaterials[i].SetFloat(SFEmissionVariableName, emission);
        }
        if (!active) Remove();
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