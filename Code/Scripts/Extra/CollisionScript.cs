using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CollisionScript : MonoBehaviour {
    #region Declarations

    public delegate void OnPickUpRadiusEnter(bool pick, GameObject cube);
    public delegate void OnAltarRadiusEnter(bool inAltarRange);
    public delegate void OnFireRadiusEnter(bool inFireRange, GameObject game);
    public delegate void InObjectTrigger(bool available);
    public delegate void PushingObject(bool t);
    public delegate void OnPlateStepped(int id, string mel, int melod);

    public static OnPickUpRadiusEnter onPickUpRadiusEnter;
    public static OnAltarRadiusEnter onAltarRadiusEnter;
    public static OnFireRadiusEnter onFireRadiusEnter;
    public static InObjectTrigger inObjectTrigger;
    public static PushingObject pushingObject;
    public static OnPlateStepped onPlateStepped;

    private AudioManager aud;
    private MovementScript mov;

    private bool pushable;
    private int layerMask = 1 << 9;

    private bool firstMelo = true;
    private bool secondMelo = false;
    private bool thirdMelo = false;

    [SerializeField] private bool playMelo;
    [SerializeField] private bool freezeY;
    [SerializeField] private float force;
    [SerializeField] private Animator ani;
    [SerializeField] private Animator ani2;
    private int ID;
    [SerializeField] private int musicID;
    [SerializeField] private string musicSound;

    private bool onSwitch;
    [SerializeField] private Material arik;
    [SerializeField] private string credit;
    [SerializeField] private ParticleSystem ariks;
    [SerializeField] private GameObject bild;
    [SerializeField] private float duration;
    [SerializeField] private float creditsDuration;
    private float t = 0.0f;
    private float tt = 0.0f;


    #endregion

    private void OnEnable()
    {
        //MusicScript.onMeldodyFinish += Conversion;
        ID = musicID;
    }

    private void Start()
    {
        aud = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        
        if(arik != null)
        {
            arik.SetFloat("_ArikDissolve", 0f);
        }


        aud.Play("pushing", false, false);
        aud.Play("pushing", true, true);
    }

    private void Update()
    {
        if (arik != null)
        {
            if (onSwitch)
            {
                Debug.Log(tt);
                tt += Time.deltaTime;
                t += 1.0f / duration * Time.deltaTime;

                ariks.gameObject.SetActive(true);

                arik.SetFloat("_ArikDissolve", Mathf.SmoothStep(0f, 1f, t));

            }

            if(tt >= 11f)
            {
                ani2.SetTrigger("B");
            }

            if(tt >= creditsDuration)
            {
                onSwitch = false;
                PlayerPrefs.DeleteKey("SpawnAnimation");
                PlayerPrefs.DeleteKey("level");
                PlayerPrefs.Save();
                Inventory.OwnsMask = false;
                ani.SetTrigger("T");
            }
        }

    }

    private int FigureID(int id)
    {
        int i = 0;

        if (secondMelo)
        {
            switch (id)
            {
                case 4:
                    i = 1;
                    break;
                case 2:
                    i = 2;
                    break;
                case 3:
                    i = 3;
                    break;
            }
        }

        else if (thirdMelo)
        {
            switch (id)
            {
                case 1:
                    i = 3;
                    break;
                case 2:
                    i = 1;
                    break;
                case 3:
                    i = 5;
                    break;
                case 4:
                    i = 2;
                    break;

            }
        }

        return i;
    }

    private void Conversion(int id)
    {
        if(id == 0)
        {
            if (firstMelo)
            {
                ID = musicID;
            }
            else if (secondMelo)
            {
                ID = FigureID(musicID);
            }
            else if (thirdMelo)
            {
                ID = FigureID(musicID);
            }
        }
        else if(id == 1)
        {
            firstMelo = false;
            secondMelo = true;
            ID = FigureID(musicID);

        }
        else if(id == 2)
        {
            secondMelo = false;
            thirdMelo = true;
            ID = FigureID(musicID);
        }
        else if(id == 3)
        {
            Debug.Log("EE");
        }
    }


    //If player enters a Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.CompareTag("PickUp") || this.CompareTag("FirePickUp") || this.CompareTag("Statue") || this.CompareTag("FakeStatue"))
            {
                if (onPickUpRadiusEnter != null)
                {
                    Debug.Log("EE");
                    onPickUpRadiusEnter(true, this.gameObject);
                }
            }
            else if (this.CompareTag("Dissolve"))
            {
                mov = GameObject.Find("ArikNoMask").GetComponent<MovementScript>();

                mov.cameraAnimationLocked = true;

                onSwitch = true;
               
            }
            else if (this.CompareTag("Altar"))
            {
                if (onAltarRadiusEnter != null)
                {
                    onAltarRadiusEnter(true);
                }
            }
            else if (this.CompareTag("Fire"))
            {
                if (onFireRadiusEnter != null)
                {
                    onFireRadiusEnter(true, this.gameObject);
                }
            }
            else if (this.gameObject.layer == 11 || this.gameObject.layer == 10)
            {
                if (inObjectTrigger != null)
                {
                    inObjectTrigger(false);
                }
            }
            else if (this.CompareTag("Pushable"))
            {
                Vector3 dir = other.transform.forward;

                if (Physics.Raycast(transform.position, dir, 3f, layerMask, QueryTriggerInteraction.Ignore))
                {
                    pushable = false;
                }
                else
                {
                    pushable = true;
                }

                if (pushable)
                {
                    dir = dir.normalized;

                    aud.Play("pushing", false, true);

                        transform.position -= dir * force * Time.deltaTime;
                    if (pushingObject != null)
                    {
                        pushingObject(true);
                    }
                }
            }
            else if (this.CompareTag("Win"))
            {
                SceneManager.LoadScene("Hub Welt");
            }
            else if (this.CompareTag("Plate"))
            {
                if (playMelo == true)
                {
                    //MelodyCameraPan mover = gameObject.GetComponent<MelodyCameraPan>();
                    
                    if (musicID == 1)
                    {
                        //if (mover)
                        //    mover.LockAndMoveCamera(ID, musicSound, 4);
                         if (onPlateStepped != null)
                        {
                            onPlateStepped(ID, musicSound, 4);
                        }
                    }
                    else if (musicID == 2)
                    {
                        //if (mover)
                        //    mover.LockAndMoveCamera(ID, musicSound, 5);
                         if (onPlateStepped != null)
                        {
                            onPlateStepped(ID, musicSound, 5);
                        }
                    }
                    else if (musicID == 3)
                    {
                        //if (mover)
                        //   mover.LockAndMoveCamera(ID, musicSound, 6);
                         if (onPlateStepped != null)
                        {
                            onPlateStepped(ID, musicSound, 6);
                        }
                    }
                }
                else
                {
                    if (firstMelo)
                    {
                        if (onPlateStepped != null)
                        {
                            onPlateStepped(ID, musicSound, 1);
                        }
                    }
                    else if (secondMelo)
                    {
                        if (onPlateStepped != null)
                        {
                            onPlateStepped(ID, musicSound, 2);
                        }

                        switch (ID)
                        {
                            case 4:
                                ID = 6;
                                break;
                            case 2:
                                ID = 4;
                                break;
                            case 3:
                                ID = 5;
                                break;

                        }

                    }
                    else if (thirdMelo)
                    {
                        if (onPlateStepped != null)
                        {
                            onPlateStepped(ID, musicSound, 3);
                        }

                        switch (ID)
                        {
                            case 4:
                                ID = 6;
                                break;
                            case 1:
                                ID = 4;
                                break;
                            case 2:
                                ID = 7;
                                break;
                            case 6:
                                ID = 8;
                                break;

                        }

                    }

                }


            }

        }
    }

    //If player is in a Trigger
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StringCollection.TAG_PLAYER))
        {
            if (this.CompareTag("PickUp") || this.CompareTag("FirePickUp") || this.CompareTag("Statue") || this.CompareTag("FakeStatue"))
            {
                if (onPickUpRadiusEnter != null)
                {
                    onPickUpRadiusEnter(true, this.gameObject);
                }
            }
            else if (this.CompareTag("Altar"))
            {
                if (onAltarRadiusEnter != null)
                {
                    onAltarRadiusEnter(true);
                }
            }
            else if (this.CompareTag("Fire"))
            {
                if (onFireRadiusEnter != null)
                {
                    onFireRadiusEnter(true, this.gameObject);
                }
            }
            else if (this.gameObject.layer == 11 || this.gameObject.layer == 10)
            {
                if (inObjectTrigger != null)
                {
                    inObjectTrigger(false);
                }
            }
            else if (this.CompareTag("Pushable"))
            {
                Vector3 dir = other.transform.forward;

                if (Physics.Raycast(transform.position, dir, 5f, layerMask, QueryTriggerInteraction.Ignore))
                {
                    pushable = false;

                }
                else
                {
                    pushable = true;
                }

                if (pushable)
                {
                    dir = dir.normalized;

                    aud.Play("pushing", false, true);

                    transform.position -= dir * force * Time.deltaTime;
                    if (pushingObject != null)
                    {
                        pushingObject(true);
                    }

                }
            }
        }
    }

    //If player exits a Trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringCollection.TAG_PLAYER))
        {
            if (this.CompareTag("PickUp") || this.CompareTag("FirePickUp") || this.CompareTag("Statue") || this.CompareTag("FakeStatue"))
            {
                if (onPickUpRadiusEnter != null)
                {
                    onPickUpRadiusEnter(false, this.gameObject);
                }
            }
            else if (this.CompareTag("Altar"))
            {
                if (onAltarRadiusEnter != null)
                {
                    onAltarRadiusEnter(false);
                }
            }
            else if (this.CompareTag("Fire"))
            {
                if (onFireRadiusEnter != null)
                {
                    onFireRadiusEnter(false, this.gameObject);
                }
            }
            else if (this.gameObject.layer == 11 || this.gameObject.layer == 10)
            {
                if (inObjectTrigger != null)
                {
                    inObjectTrigger(true);
                }
            }
            else if (this.CompareTag("Pushable"))
            {
                if (pushingObject != null)
                {
                    pushingObject(false);
                }

                aud.Play("pushing", true, true);

            }
        }
    }

    private void OnDisable()
    {
        //MusicScript.onMeldodyFinish -= Conversion;
    }
}
