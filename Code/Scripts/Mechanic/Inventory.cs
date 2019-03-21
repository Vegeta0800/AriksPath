using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    #region Declarations

    public delegate void OnAltarPlaceObject(bool realStatue, Transform objectTransform);
    public delegate void OnFirePlaceObject(GameObject gameObject, bool t);
    public delegate void OnStatueTaken();
    public delegate void OnCubeStateChange(GameObject gameObject);

    public static OnAltarPlaceObject onAltarPlaceObject;
    public static OnFirePlaceObject onFirePlaceObject;
    public static OnCubeStateChange onCubeStateChange;
    public static OnStatueTaken onStatueTaken;

    private GameObject Cube;
    private GameObject bush;
    private GameObject fire;
    [SerializeField] private Transform hand;
    [SerializeField] private Transform blockInv;
    [SerializeField] private Transform statueInv;


    [SerializeField] private Camera cam;

    private bool pickUp;

    private bool inAltarRange;
    private bool inFireRange;

    private bool ghostWorldActive;

    private int layerMask = 1 << 9;

    [SerializeField] private Transform player;
    [SerializeField] private Transform real;
    [SerializeField] private Transform ghost;

    [SerializeField] private float blockX;
    [SerializeField] private float blockY;
    [SerializeField] private float blockZ;

    private bool pickUpStart;
    public bool pickingUp;

    public bool maskUp;

    [SerializeField] private Animator ani;

    private Quaternion rotationOfFire;

    public AudioManager aud;

    [Space(5)]
    [SerializeField]
    [Tooltip("For Development: Grants Mask Ability. (Independent of collecting the Mask)")]
    public bool alwaysOwnsMaskHere = false;
    /// <summary>
    /// Check if the player collected the mask
    /// </summary>
    public static bool OwnsMask = false;
    public GameObject mask;
    public bool picked;
    public bool noJump;
    public static Inventory instance { get; private set; }

    float deltaTime = 0.0f;

    #endregion
    //Enable Delegates
    private void OnEnable()
    {
        instance = this;
        //if (OwnsMask) alwaysOwnsMaskHere = true;

        Inputs.playerTriggeredAction += PlayerAction;
        Inputs.onUpdate += OnUpdate;

        CollisionScript.onPickUpRadiusEnter += CollisionScriptConversionPickup;
        CollisionScript.onAltarRadiusEnter += CollisionScriptConversionAltar;
        CollisionScript.onFireRadiusEnter += CollisionScriptConversionFire;


        SwitchScript.onDimensionSwitch += SwitchScriptConversion;

        if (OwnsMask)
        {
            if (mask != null)
                mask.SetActive(true);
        }
        else
        {
            if (mask != null)
                mask.SetActive(false);
        }
    }

    private void OnUpdate()
    {
        if (fire != null)
        {
            fire.transform.rotation = rotationOfFire;
        }

        //if (pickUpStart)
        //{

        //    Cube.transform.position = Vector3.Lerp(Cube.transform.position, player.position + new Vector3(player.TransformDirection(-Vector3.forward).x * blockX, blockY, player.TransformDirection(-Vector3.forward).z * blockZ), 10f * Time.deltaTime);
        //}

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void PlayerAction(ePlayerAction action)
    {

        if (action == ePlayerAction.interact)
            InteractWithEnvironment();
    }

    private IEnumerator PickingUpBool()
    {
        yield return new WaitForSeconds(2f);
        pickingUp = false;
    }

    //Check player input
    private void InteractWithEnvironment()
    {
        if (pickingUp == false)
        {
            if (pickUp == true && picked == false && Cube != null && ((cam.cullingMask & (1 << Cube.layer)) != 0))
            {
                pickingUp = true;
                StartCoroutine(PickingUpBool());
                if (Cube.transform.CompareTag("PickUp"))
                {
                    Cube.transform.parent = null;
                    StartCoroutine(PickingUp(false));
                }
                else if (Cube.transform.CompareTag("FirePickUp") && ghostWorldActive)
                {
                    FirePickUp();
                }
                else if (Cube.transform.CompareTag("Statue"))
                {
                    if (Cube.transform.parent != null)
                    {
                        if (Cube.transform.parent.CompareTag("Slot"))
                        {
                            if (onStatueTaken != null)
                            {
                                onStatueTaken();
                            }
                            Cube.transform.parent = null;
                        }
                    }
                    StartCoroutine(PickingUp(true));


                }

            }

            else if (Cube != null && picked == true)
            {
                pickingUp = true;
                StartCoroutine(PickingUpBool());
                if (Cube.CompareTag("FirePickUp"))
                {
                    if (inFireRange == true)
                    {
                        if (onFirePlaceObject != null)
                        {
                            onFirePlaceObject(bush, true);
                        }
                    }
                    DropFire();
                }
                else if (Cube.CompareTag("Statue"))
                {

                    if (inAltarRange == true)
                    {
                        if (onAltarPlaceObject != null)
                        {
                            onAltarPlaceObject(true, Cube.transform);

                        }
                        StartCoroutine(Drop(true, true));

                    }
                    else
                    {
                        StartCoroutine(Drop(true, false));
                    }


                }
                else
                {
                    StartCoroutine(Drop(false, false));
                }
            }

        }
    }

    #region Pickup
    private void FirePickUp()
    {
        ani.SetBool("FireUp", true);

        picked = true;
        fire = Instantiate(Cube, hand.position, Quaternion.Euler(0, 0, 0), hand);

        rotationOfFire = fire.transform.rotation;
        fire.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);

        if (onCubeStateChange != null)
        {
            onCubeStateChange(fire);
        }

        pickingUp = false;
    }

    private IEnumerator PickingUp(bool statue)
    {
        picked = true;
        noJump = true;
        if (statue)
        {
            ani.SetBool("StatueUp", true);
            Cube.GetComponent<Rigidbody>().isKinematic = true;
            Cube.transform.SetParent(statueInv);

            yield return new WaitForSeconds(1f);
            pickingUp = false;

            aud.Play("itemCollect", false, false);

            if (Cube.layer == 10 || Cube.layer == 11)
            {
                Cube.layer = 12;

                if (Cube.transform.childCount != 0)
                {
                    for (int i = 0; i < Cube.transform.childCount; i++)
                    {
                        Cube.transform.GetChild(i).gameObject.layer = 12;
                    }
                }

            }

            Cube.transform.position = statueInv.position;
            Cube.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        }
        else
        {
            ani.SetBool("PickUp", true);
            Cube.GetComponent<Rigidbody>().isKinematic = true;

            Cube.transform.SetParent(blockInv);

            yield return new WaitForSeconds(1f);
            pickingUp = false;

            aud.Play("itemCollect", false, false);

            if (Cube.layer == 10 || Cube.layer == 11)
            {
                Cube.layer = 12;

                if (Cube.transform.childCount != 0)
                {
                    for (int i = 0; i < Cube.transform.childCount; i++)
                    {
                        Cube.transform.GetChild(i).gameObject.layer = 12;
                    }
                }

            }

            Cube.transform.position = blockInv.position;
            //Cube.transform.eulerAngles = statueInv.eulerAngles;/*Cube.transform.position = player.position + new Vector3(player.TransformDirection(-Vector3.forward).x * blockX, blockY, player.TransformDirection(-Vector3.forward).z * blockZ);*/

        }


        yield return new WaitForSeconds(1f);

        pickingUp = false;
    }


    #endregion

    #region Drop
    //Drop function
    private IEnumerator Drop(bool dropStatue, bool t)
    {
        if (dropStatue == true && t == false)
        {
            ani.SetBool("StatueUp", false);

            yield return new WaitForSeconds(1f);
            pickingUp = false;

            Cube.transform.parent = null;

            if (ghostWorldActive)
            {
                if (Cube.layer == 12)
                {
                    Cube.layer = 10;

                    if (Cube.transform.childCount != 0)
                    {
                        for (int i = 0; i < Cube.transform.childCount; i++)
                        {
                            Cube.transform.GetChild(i).gameObject.layer = 10;
                        }
                    }
                }

            }
            else
            {
                if (Cube.layer == 12)
                {
                    Cube.layer = 11;

                    if (Cube.transform.childCount != 0)
                    {
                        for (int i = 0; i < Cube.transform.childCount; i++)
                        {
                            Cube.transform.GetChild(i).gameObject.layer = 11;
                        }
                    }
                }

            }
            Cube.GetComponent<Rigidbody>().isKinematic = false;


        }
        else if (dropStatue == true && t == true)
        {
            ani.SetBool("StatueUp", false);

            yield return new WaitForSeconds(1f);
            pickingUp = false;

        }
        else
        {
            ani.SetBool("PickUp", false);

            yield return new WaitForSeconds(1f);

            pickingUp = false;

            aud.Play("blockDown", false, false);

            Cube.transform.parent = null;

            if (ghostWorldActive)
            {
                if (Cube.layer == 12)
                {
                    Cube.layer = 10;

                    if (Cube.transform.childCount != 0)
                    {
                        for (int i = 0; i < Cube.transform.childCount; i++)
                        {
                            Cube.transform.GetChild(i).gameObject.layer = 10;
                        }
                    }
                }

                Cube.transform.SetParent(ghost);
            }
            else
            {
                if (Cube.layer == 12)
                {
                    Cube.layer = 11;

                    if (Cube.transform.childCount != 0)
                    {
                        for (int i = 0; i < Cube.transform.childCount; i++)
                        {
                            Cube.transform.GetChild(i).gameObject.layer = 11;
                        }
                    }
                }

                Cube.transform.SetParent(real);
            }
            Cube.GetComponent<Rigidbody>().isKinematic = false;

        }

        yield return new WaitForSeconds(1f);

        picked = false;
        noJump = false;
        pickingUp = false;
        Cube = null;
    }

    private void DropFire()
    {
        ani.SetBool("FireUp", false);
        pickingUp = false;
        picked = false;
        Destroy(fire);
        fire = null;
        Cube = null;
    }

    #endregion

    #region ScriptData
    //ScriptData
    private void CollisionScriptConversionPickup(bool pick, GameObject cube)
    {
        if (picked == false)
        {
            pickUp = pick;
            Cube = cube;
        }
    }
    private void CollisionScriptConversionAltar(bool altar)
    {
        inAltarRange = altar;
    }
    private void CollisionScriptConversionFire(bool fire, GameObject game)
    {
        inFireRange = fire;
        bush = game;
    }
    private void SwitchScriptConversion(bool ghostWorld, bool destroyCube)
    {
        ghostWorldActive = ghostWorld;

        if (destroyCube == true)
        {
            DropFire();
        }
    }
    #endregion

    //Disable Delegates
    private void OnDisable()
    {
        Inputs.playerTriggeredAction -= PlayerAction;
        Inputs.onUpdate -= OnUpdate;

        CollisionScript.onPickUpRadiusEnter -= CollisionScriptConversionPickup;
        CollisionScript.onAltarRadiusEnter -= CollisionScriptConversionAltar;
        CollisionScript.onFireRadiusEnter -= CollisionScriptConversionFire;


        SwitchScript.onDimensionSwitch -= SwitchScriptConversion;
    }
}
