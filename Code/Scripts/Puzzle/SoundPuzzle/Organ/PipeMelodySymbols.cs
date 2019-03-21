/// @author: J-D Vbk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMelodySymbols : MonoBehaviour {

    [SerializeField]
    bool lookToCamera = true;
    
    Transform cameraTransform;
    SpriteRenderer myRenderer;

    // Use this for initialization
    void Start()
    {
        if (lookToCamera)
            cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        myRenderer = GetComponent<SpriteRenderer>();
        myRenderer.enabled = false;
    }
    
    public void ShowSymbol(Sprite symbol)
    {
        if (lookToCamera) transform.LookAt(cameraTransform.position);
        myRenderer.sprite = symbol;
        if (!myRenderer.enabled)
            myRenderer.enabled = true;
    }

    public IEnumerator DelayedHideSymbol(float time)
    {
        yield return new WaitForSeconds(time);
        HideSymbol();
    }

    public void HideSymbol()
    {
        if (myRenderer.enabled) myRenderer.enabled = false;
    }
}
