using UnityEngine;
using UnityEngine.PostProcessing;

[ExecuteInEditMode]
public class MaskTransition : MonoBehaviour
{
    public delegate void OnTransitionFinished();
    public static OnTransitionFinished onTransitionFinished;

    [SerializeField] private Material maskTransition;
    [SerializeField] private Material skyBoxTransition;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float duration;
    private bool onSwitch;
    private bool ghostSwitch;

    [SerializeField] private PostProcessingProfile real;
    [SerializeField] private PostProcessingProfile ghost;

    [SerializeField] private PostProcessingBehaviour post;

    private float t = 0.0f;

    private int mask;

    private void OnEnable()
    {
        Inputs.onUpdate += OnUpdate;
        SwitchScript.onTransitionSwitch += SwitchConversion;
    }


    private void Start()
    {
        mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Normal World only");
        mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Ghost World only"));

        post.profile = real;

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
        maskTransition.SetTexture("_TargetTexture", rt);

        maskTransition.SetFloat("_Slider", 0f);
        cam.gameObject.SetActive(false);
        skyBoxTransition.SetFloat("_Ghostsky", 0f);
    }

    private void OnUpdate()
    {
        if (onSwitch == true)
        {
            cam.gameObject.SetActive(true);

            t += 1.0f / duration * Time.deltaTime;

            if (ghostSwitch == true)
            {
                maskTransition.SetFloat("_Slider", Mathf.SmoothStep(0f, 1f, t));
                skyBoxTransition.SetFloat("_Ghostsky", Mathf.SmoothStep(0f, 1f, t));
            }
            else
            {
                post.profile = real;
                mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Normal World only");
                mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Ghost World only"));

                maskTransition.SetFloat("_Slider", Mathf.SmoothStep(1f, 0f, t));
                skyBoxTransition.SetFloat("_Ghostsky", Mathf.SmoothStep(1f, 0f, t));

            }

        }

        if (ghostSwitch == true)
        {
            if (onSwitch == true && maskTransition.GetFloat("_Slider") == 1f)
            {
                t = 0.0f;
                onSwitch = false;
                maskTransition.SetFloat("_Slider", 0f);
                cam.gameObject.SetActive(false);
                post.profile = ghost;
                if (onTransitionFinished != null)
                {
                    onTransitionFinished();
                }

                mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Ghost World only");
                mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Normal World only"));
            }
        }
        else
        {
            if (onSwitch == true && maskTransition.GetFloat("_Slider") == 0f)
            {
                t = 0.0f;
                onSwitch = false;
                cam.gameObject.SetActive(false);

                if (onTransitionFinished != null)
                {
                    onTransitionFinished();
                }

            }
        }

    }

    private void SwitchConversion(bool switching, bool switchToGhost)
    {
        onSwitch = switching;
        ghostSwitch = switchToGhost;
    }

    private void OnRenderImage(RenderTexture sourceImage, RenderTexture destinationImage)
    {
        Graphics.Blit(sourceImage, destinationImage, maskTransition);
    }

    private void OnDisable()
    {
        Inputs.onUpdate -= OnUpdate;
        SwitchScript.onTransitionSwitch -= SwitchConversion;
    }
}
