using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchScript : MonoBehaviour
{

    #region Declarations

    public static SwitchScript instance { get; private set; }

    public delegate void OnDimensionSwitch(bool switched, bool destroyCube);
    public delegate void OnTransitionSwitch(bool onSwitch, bool switchToGhost);
    public delegate void OnPauseMenu(bool paused);

    public static OnDimensionSwitch onDimensionSwitch;
    public static OnTransitionSwitch onTransitionSwitch;
    public static OnPauseMenu onPauseMenu;

    //[SerializeField] private GameObject realCam;
    //[SerializeField] private GameObject ghostCam;

    [SerializeField]
    private GameObject ghost;
    [SerializeField]
    private GameObject real;

    [SerializeField]
    private GameObject ghostLight;
    [SerializeField]
    private GameObject realLight;

    [SerializeField]
    private GameObject maskOnHead;
    [SerializeField]
    private GameObject maskOnBack;

    [SerializeField]
    private Animator ani;

    private bool available = true;

    private bool realActive = true;

    private GameObject pickUp;
    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private EventSystem eventSystem;

    public AudioManager aud;

    private int layerMask = 1 << 9;

    #endregion


    //Enables Delegates
    private void OnEnable()
    {
        Inputs.playerTriggeredAction += PlayerAction;
        Inventory.onCubeStateChange += InventoryScriptData;

        CollisionScript.inObjectTrigger += InObjectStanding;

        MaskTransition.onTransitionFinished += TransitionFinished;

        Buttons.onResumed += OnResumed;
    }

    private void Start()
    {
        ColliderFunction(real, false, 11);
        ColliderFunction(ghost, true, 10);

        aud.Play("realBackround", false, false);
        aud.Play("realAtmo", false, false);

        aud.Play("ghostBackround", false, false);
        aud.Play("ghostAtmo", false, false);

        aud.Play("ghostBackround", true, true);
        aud.Play("ghostAtmo", true, true);

        ghostLight.SetActive(false);
        realLight.SetActive(true);

        //ghostCam.SetActive(false);
        //realCam.SetActive(true);
    }

    private void PlayerAction(ePlayerAction action)
    {
        // shift
        if (action == ePlayerAction.shift && available == true)
        {
            available = false;

            aud.Play("transition", false, false);

            if (realActive == true)
            {
                SwitchDimension(true);
            }
            else
            {
                SwitchDimension(false);
            }
        }
        // pause
        else if (action == ePlayerAction.pause)
        {
            PauseMenu();
        }
    }

    private void PauseMenu()
    {
        if (pauseMenu.activeInHierarchy == true)
        {
            pauseMenu.SetActive(false);
            if (onPauseMenu != null)
            {
                onPauseMenu(false);
            }

            Time.timeScale = 1f;



            if (realActive == true)
            {
                aud.Play("realAtmo", false, true);
                aud.Play("realBackround", false, true);
            }
            else
            {
                aud.Play("ghostAtmo", false, true);
                aud.Play("ghostBackround", false, true);
            }
        }
        else
        {
            if (onPauseMenu != null)
            {
                onPauseMenu(true);
            }

            pauseMenu.SetActive(true);
            eventSystem.SetSelectedGameObject(GameObject.Find("Resume"));

            if (realActive == true)
            {
                aud.Play("realAtmo", true, true);
                aud.Play("realBackround", true, true);
            }
            else
            {
                aud.Play("ghostAtmo", true, true);
                aud.Play("ghostBackround", true, true);
            }
            Time.timeScale = 0f;
        }

    }

    private void InventoryScriptData(GameObject gameObject)
    {
        pickUp = gameObject;
    }
    private void TransitionFinished()
    {
        available = true;
    }
    private void InObjectStanding(bool condition)
    {
        available = condition;
    }
    private void OnResumed()
    {
        PauseMenu();
    }

    //Switching dimensions
    private void SwitchDimension(bool ghostSwitch)
    {

        if (ghostSwitch == true)
        {
            if (onTransitionSwitch != null)
            {
                onTransitionSwitch(true, true);
            }

            StartCoroutine(AnimationBool("MaskOn", false));
        }
        else
        {
            if (onTransitionSwitch != null)
            {
                onTransitionSwitch(true, false);
            }

            StartCoroutine(AnimationBool("MaskOff", true));
        }


    }

    private IEnumerator AnimationBool(string animation, bool realWorld)
    {
        //ghostCam.SetActive(true);
        //realCam.SetActive(true);

        ani.SetBool(animation, true);
        ani.SetLayerWeight(1, 1f);

        if (animation == "MaskOff")
        {
            yield return new WaitForSeconds(0.4f);
        }
        else
        {
            yield return new WaitForSeconds(0.8f);

            if (realWorld == true)
            {
                if (onDimensionSwitch != null)
                {
                    onDimensionSwitch(false, false);
                }
            }
            else
            {
                if (onDimensionSwitch != null)
                {
                    onDimensionSwitch(true, false);
                }
            }
        }

        yield return new WaitForEndOfFrame();


        ani.SetLayerWeight(1, 0f);
        ani.SetBool(animation, false);

        if (realWorld == true)
        {
            aud.Play("ghostBackround", true, true);
            aud.Play("realBackround", false, true);

            aud.Play("ghostAtmo", true, true);
            aud.Play("realAtmo", false, true);

            ColliderFunction(real, false, 11);
            ColliderFunction(ghost, true, 10);

            if (pickUp != null)
            {
                pickUp = null;

                if (onDimensionSwitch != null)
                {
                    onDimensionSwitch(false, true);
                }
            }
            else
            {
                if (onDimensionSwitch != null)
                {
                    onDimensionSwitch(false, false);
                }
            }

            realActive = true;

            maskOnBack.SetActive(true);
            maskOnHead.SetActive(false);

            ghostLight.SetActive(false);
            realLight.SetActive(true);

            //ghostCam.SetActive(false);
            //realCam.SetActive(true);

        }
        else
        {
            aud.Play("realBackround", true, true);
            aud.Play("ghostBackround", false, true);

            aud.Play("realAtmo", true, true);
            aud.Play("ghostAtmo", false, true);

            ColliderFunction(real, true, 11);
            ColliderFunction(ghost, false, 10);

            if (onDimensionSwitch != null)
            {
                onDimensionSwitch(true, false);
            }

            realActive = false;

            maskOnBack.SetActive(false);
            maskOnHead.SetActive(true);

            ghostLight.SetActive(true);
            realLight.SetActive(false);

            //ghostCam.SetActive(true);
            //realCam.SetActive(false);

        }
    }

    private void ColliderFunction(GameObject game, bool trigger, int layer)
    {
        for (int i = 0; i < game.transform.childCount; i++)
        {
            if (game.transform.GetChild(i).GetComponent<Collider>() != null)
            {
                if (game.transform.GetChild(i).gameObject.layer == layer)
                {
                    foreach (Collider c in game.transform.GetChild(i).GetComponents<Collider>())
                    {
                        c.isTrigger = trigger;
                    }

                    if (game.transform.GetChild(i).GetComponent<Rigidbody>() != null)
                    {
                        game.transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = trigger;
                    }
                }
            }
        }
    }

    //Disables Delegates
    private void OnDisable()
    {
        Inputs.playerTriggeredAction -= PlayerAction;
        Inventory.onCubeStateChange -= InventoryScriptData;

        CollisionScript.inObjectTrigger -= InObjectStanding;

        MaskTransition.onTransitionFinished -= TransitionFinished;

        Buttons.onResumed -= OnResumed;
    }
}
