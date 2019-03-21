/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonanceGlow : ResonanceBase
{
    [Header("Glow")]
    [SerializeField]
    bool shaderForgeMaterial;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float reachPeak = 0.3f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float leavePeak = 0.7f;
    [Space(5)]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float baseEmission = 0.25f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float resonaceEmission = 1.0f;

    List<Material> affectedMaterials = new List<Material>();
    Color baseColor;

    float shaderPropertyID;

    // Use this for initialization
    void Start()
    {
        Setup();

        reachPeak = Mathf.Clamp(reachPeak, 0.001f, 0.999f);
        leavePeak = Mathf.Clamp(leavePeak, 0.001f, 0.999f);

        if (!shaderForgeMaterial)
            shaderPropertyID = Shader.PropertyToID("_EmissionColor");
        else
            shaderPropertyID = Shader.PropertyToID("_button_emission");

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
                        baseColor = matty.GetColor("_EmissionColor");
                    //}
                }
            }
        }
        if (affectedMaterials.Count < 1)
        {
            Debug.Log(gameObject.name + " no matching Material");
            Destroy(this);
        }
        foreach (Material mat in affectedMaterials)
        {
            if (!shaderForgeMaterial)
                mat.SetColor("_EmissionColor", baseColor * Mathf.LinearToGammaSpace(baseEmission));
            else
                mat.SetFloat("_button_emission", baseEmission);
        }
    }

    protected override void Resonate(sSimpleNote note, sSimpleTone tone)
    {
        StartCoroutine(GlowUp(note.timeUntilNext));
    }


    private IEnumerator GlowUp(float duration)
    {
        yield return null;
        float currentTime = 0;
        float glow;
        float shift = (resonaceEmission - baseEmission);
        float emission;
        // build Up
        float steptime = duration * reachPeak;
        while (currentTime <= steptime)
        {
            glow = (1 - Mathf.Cos(currentTime / steptime * Mathf.PI)) / 2;
            emission = baseEmission + glow * shift;
            foreach (Material mat in affectedMaterials)
            {
                if (!shaderForgeMaterial)
                    mat.SetColor("_EmissionColor", baseColor * Mathf.LinearToGammaSpace(emission));
                else
                    mat.SetFloat("_button_emission", emission);
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds((duration * leavePeak) - currentTime);

        steptime  = duration - duration * leavePeak;
        currentTime = 0;
        // cool Down
        while (currentTime <= steptime)
        {
            glow = (1 - Mathf.Cos(currentTime / steptime * Mathf.PI)) / 2;
            emission = baseEmission + (1 - glow) * shift;
            foreach (Material mat in affectedMaterials)
            {
                if (!shaderForgeMaterial)
                    mat.SetColor("_EmissionColor", baseColor * Mathf.LinearToGammaSpace(emission));
                else
                    mat.SetFloat("_button_emission", emission);
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        foreach (Material mat in affectedMaterials)
        {
            if (!shaderForgeMaterial)
                mat.SetColor("_EmissionColor", baseColor * Mathf.LinearToGammaSpace(baseEmission));
            else
                mat.SetFloat("_button_emission", baseEmission);
        }
    }

    private void OnDestroy()
    {
        pipeOrgan.singleNote -= HearedSound;
    }
}
