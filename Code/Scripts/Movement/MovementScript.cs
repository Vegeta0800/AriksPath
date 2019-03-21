
//CHARACTER is currently rotated by 180°, affects all direction related actions
# define CHARACTER_ROTATED_180
// remove the above to swap to normal targeting

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MovementScript : MonoBehaviour
{

    Vector2 movementDir;

    //public static MovementScript instance { get; private set; }

    [SerializeField]
    private CharacterController player;

    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform groundChecker;
    //[SerializeField] private Transform spawnpoint;
    [SerializeField]
    private Transform[] teleports;


    private Vector3 jump;
    private Vector3 move;

    private const int layerMask = 1 << 9;

    private float hitDistance = 0.1f;
    private float hitGround = 0.5f;//0.35f;
    private float hitAir = 0.1f;//0.15f;
    private bool isGrounded;
    private bool notJustJumped;

    [Space(10)]
    [SerializeField]
    private float jumpHeight = 3f;

    [SerializeField]
    private float smoothness;

    //private float sensitivityY = 1.5f;

    private float turnRotation;


    [SerializeField]
    private float startSpeed;
    [SerializeField]
    private float sprint;
    [SerializeField]
    private float deathHeight;
    private float speed;
    private float sprinting;


    //private float x = 0.0f;
    //private float y = 0.0f;

    [SerializeField]
    private Animator ani;
    [SerializeField]
    private Animator ani2;

    //[HideInInspector]
    /// <summary>
    /// suffocates Update
    /// </summary>
    public bool cameraAnimationLocked;

    public static bool ghostWorldActive { get; private set; }


    // Jump Sounds
    [Header("Outdated Effects")]
    public AudioSource JumpSound;

    [SerializeField]
    private AudioSource footstepWalkingL;
    [SerializeField]
    private AudioSource footstepWalkingR;

    [SerializeField]
    private AudioSource footstepRunL;
    [SerializeField]
    private AudioSource footstepRunR;

    [Header("Sound Effects")]
    [SerializeField]
    AudioSource voice;
    [SerializeField]
    AudioSource footNoiseOrigin;
    [SerializeField]
    sSimpleTone jumpStart;

    private bool pushing;

    [Header("SFX, Animation Triggered")]
    [SerializeField]
    sSimpleTone footstepWalkLeftSfx;
    [SerializeField]
    sSimpleTone footstepWalkRightSfx;
    [Space(5)]
    [SerializeField]
    sSimpleTone footstepRunLeftSfx;
    [SerializeField]
    sSimpleTone footstepRunRightSfx;
    [Space(5)]
    [SerializeField]
    sSimpleTone jumpLandingSfx;

    Vector2 movementDirection = new Vector2();
    Vector3 tempVec;
    float smoothMove;
    [Space(5)]
    [SerializeField]
    Transform leftToe;
    [SerializeField]
    Transform rightToe;
    Vector3 leftPos;
    Vector3 rightPos;
    bool landing;
    [SerializeField]
    float startLandingHeight = 2;
    //_______________________________________________________

    private void OnEnable()
    {
        //instance = this;
        SwitchScript.onDimensionSwitch += SwitchConversion;
        CollisionScript.pushingObject += Pushing;

        Inputs.onUpdate += OnUpdate;
        Inputs.playerMovement += PlayerMovement;
        Inputs.playerTriggeredAction += PlayerAction;

        speed = startSpeed;
        sprinting = startSpeed * sprint;
        notJustJumped = true;

        if (voice == null)
            voice = GetComponent<AudioSource>();
        if (footNoiseOrigin == null)
            footNoiseOrigin = GetComponent<AudioSource>();

        if (leftToe == null)
        {
            GameObject lt = GameObject.Find("Toe.L"); // ugly
            if (lt)
                leftToe = lt.transform;
            else
                Debug.Log(name + " couldn't find left Toe.");
        }
        if (rightToe == null)
        {
            GameObject rt = GameObject.Find("Toe.R"); // ugly
            if (rt)
                rightToe = rt.transform;
            else
                Debug.Log(name + " couldn't find right Toe.");
        }
    }

    private void Start()
    {
        ghostWorldActive = false;
        StartCoroutine(Spawning());
    }

    private IEnumerator Spawning()
    {

        if (SpawnPointManager.localSpawnPoint)
        {
            SpawnPointManager.localSpawnPoint.Use(player.transform);
            Scene scene = SceneManager.GetActiveScene();

            if (scene.name == "Level 1" && PlayerPrefs.GetInt("SpawnAnimation") == 0)
            {
                PlayerPrefs.SetInt("SpawnAnimation", 1);

                cameraAnimationLocked = true;

                ani.SetBool("Laying", true);

                yield return new WaitForSeconds(9f);

                cameraAnimationLocked = false;
            }
            else
            {
                ani.SetBool("Laying", false);
            }
        }
        else
        {
            PlayerPrefs.SetFloat("LastPositionX", this.transform.position.x);
            PlayerPrefs.SetFloat("LastPositionY", this.transform.position.y);
            PlayerPrefs.SetFloat("LastPositionZ", this.transform.position.z);
        }

    }

    /// <summary>
    /// act out the players input
    /// </summary>
    /// <param name="action"></param>
    private void PlayerAction(ePlayerAction action)
    {
        if (cameraAnimationLocked) return;
        switch (action)
        {
            case ePlayerAction.jump:
                if (isGrounded && Inventory.instance.noJump == false) Jumping();
                break;
            case ePlayerAction.sprint:
                if (isGrounded)
                {
                    speed = sprinting;
                }
                break;
            case ePlayerAction.walk:
                //Debug.Log("walking");
                speed = startSpeed;
                break;
        }
    }
    private void Pushing(bool t)
    {
        pushing = t;
    }
    private void PlayerMovement(float inputX, float inputY)
    {
        if ((inputX != 0 || inputY != 0) && !cameraAnimationLocked)
        {
            movementDirection.x = inputX;
            movementDirection.y = inputY;
            movementDirection.Normalize();

            // flattened & normalized Camera forward Vector
            tempVec = cam.forward;
            tempVec.y = 0;
            tempVec.Normalize();

            smoothMove = Mathf.Max(Mathf.Abs(inputX), Mathf.Abs(inputY));

            float prev = move.y;
            move.x = 0;
            move.z = 0;
            move += smoothMove * movementDirection.x * cam.right;
            move += smoothMove * movementDirection.y * tempVec;
            move *= speed;
            move.y = prev;
            //Debug.Log(smoothMove + "/"+movementDirection+"/"+ move +"/"+speed);

            // ROTATION
            tempVec = player.transform.forward;
#if CHARACTER_ROTATED_180
            turnRotation = (Mathf.Atan2(-tempVec.z, -tempVec.x) - Mathf.Atan2(move.z, move.x)) * Mathf.Rad2Deg;
#else
                turnRotation = (Mathf.Atan2(tempVec.z, tempVec.x) - Mathf.Atan2(move.z, move.x)) * Mathf.Rad2Deg;
#endif

            Vector3 newAngle = player.transform.eulerAngles;

            if (turnRotation > 180)
                turnRotation -= 360;
            else if (turnRotation < -180)
                turnRotation += 360;

            newAngle.y += turnRotation * smoothness * Time.deltaTime;
            player.transform.eulerAngles = newAngle;

            // ANIMATION
            if (pushing == false)
            {
                if (speed == sprinting)
                {
                    if (ghostWorldActive == true)
                    {
                        ani.SetFloat("Walkcycle", 0.84f);
                    }
                    else
                    {
                        ani.SetFloat("Walkcycle", 0.84f);
                    }

                }
                else
                {
                    if (ghostWorldActive == true)
                    {
                        ani.SetFloat("Walkcycle", 2f);
                    }
                    else
                    {
                        ani.SetFloat("Walkcycle", 0.49f);
                    }
                }
            }
            else
            {
                ani.SetFloat("Walkcycle", 1.2f);
            }

        }
        else
        {
            move.x = 0f;
            move.z = 0f;
            StopWalking();
        }
    }

    //jumping + Sound
    private void Jumping()
    {
        //JumpSound.Play();
        PlayShout(jumpStart);

        notJustJumped = false;
        move.y = jumpHeight;
        ani.SetBool("Landing", false);


        ani.SetBool("Jump", true);

        //if (ghostWorldActive == true)
        //{
        //    ani.SetFloat("MaskedJump", 1f);
        //}
        //else
        //{
        //    ani.SetFloat("MaskedJump", 0f);
        //}

        Invoke("InvokeJumpBool", 0.2f);
    }

    /// <summary>
    /// activating GroundCheck again (called from Jumping, by Invoke)
    /// </summary>
    private void InvokeJumpBool()
    {
        notJustJumped = true;
        ani.SetBool("Jump", false);
    }

    //checks if player is on ground
    private void Grounded(bool ground, Transform objectHit)
    {
        if (ground != isGrounded)
        {
            if (ground == true)
            {
                if (move.y <= deathHeight || objectHit.gameObject.CompareTag("Death"))
                {
                    FadeToDeath();
                }

                notJustJumped = true;
                isGrounded = true;
                ani.SetBool("Landing", true);
                // set through Checkpoints
                //PlayerPrefs.SetFloat("LastPositionX", player.transform.position.x);
                //PlayerPrefs.SetFloat("LastPositionY", player.transform.position.y);
                //PlayerPrefs.SetFloat("LastPositionZ", player.transform.position.z);
                hitDistance = hitGround;
            }
            else
            {
                isGrounded = false;
                hitDistance = hitAir;
            }
        }

        if (isGrounded == true)
        {
            move.y = 0.0f;
            ani.SetBool("inAir", false);
        }
        else
        {
            move.y -= 9.81f * Time.deltaTime;
            ani.SetBool("inAir", true);
        }
    }

    private void OnUpdate()
    {
        RaycastHit hit;
        //InLandingDistance();

        if (Physics.Raycast(groundChecker.position, Vector3.down, out hit, hitDistance,
                            ~layerMask, QueryTriggerInteraction.Ignore) && notJustJumped)
        {
            Grounded(true, hit.transform);
        }
        else
        {
                CheckLandingDistance();
                Grounded(false, hit.transform);
        }

        /*
        if (move == Vector3.zero)
        {
            StopWalking();
        }
        */
        player.Move(move * Time.deltaTime);
    }

    private void CheckLandingDistance()
    {
        leftPos = leftToe.position;
        rightPos = rightToe.position;
        if (leftPos.y < rightPos.y)
            rightPos.y = leftPos.y;
        else
            leftPos.y = rightPos.y;
        leftPos = (leftPos + rightPos) / 2.0f;
        RaycastHit hit;
        if (Physics.Raycast(leftPos, Vector3.down, out hit, startLandingHeight,
                    ~layerMask, QueryTriggerInteraction.Ignore) && notJustJumped)
        {
            ani.SetBool("Landing", true);
        }
    }

    private void FadeToDeath()
    {
        ani2.SetTrigger("FadeOut2");
        StartCoroutine(OnDeath());
    }

    private IEnumerator OnDeath()
    {
        player.transform.position = new Vector3(PlayerPrefs.GetFloat("LastPositionX"), PlayerPrefs.GetFloat("LastPositionY"), PlayerPrefs.GetFloat("LastPositionZ"));
        yield return new WaitForSeconds(1f);
    }

    private void SwitchConversion(bool t, bool r)
    {
        ghostWorldActive = t;
    }

    /// <summary>
    /// Stops Movement Animations
    /// </summary>
    public void StopWalking()
    {
        if (ghostWorldActive == true)
        {
            ani.SetFloat("Walkcycle", 1.6f);
        }
        else
        {
            ani.SetFloat("Walkcycle", 0.0f);
        }
    }

    #region Sfx

    /// <summary>
    /// plays the given sfx
    /// </summary>
    private void PlaySound(sSimpleTone sound)
    {
        if (sound.audioClip != null)
        {
            footNoiseOrigin.clip = sound.audioClip;
            footNoiseOrigin.volume = sound.volume;
            footNoiseOrigin.pitch = sound.pitch;
            footNoiseOrigin.Play();
        }
        else
            Debug.Log(this.name + " tried playing empty Sfx.");
    }

    /// <summary>
    /// plays shouts of the character
    /// </summary>
    /// <param name="shout"></param>
    private void PlayShout(sSimpleTone shout)
    {
        if (shout.audioClip != null)
        {
            if (voice.clip == null || !voice.isPlaying)
            {
                voice.clip = shout.audioClip;
                voice.volume = shout.volume;
                voice.pitch = shout.pitch;
                voice.Play();
            }
        }
    }

    /// <summary>
    /// called by Events in Animation
    /// </summary>
    public void PlayStepSound(int leftFootGreaterZero)
    {
        Debug.Log("Tap.");
        if (leftFootGreaterZero > 0)
        {
            PlaySound(footstepWalkLeftSfx);
        }
        else
        {
            PlaySound(footstepWalkRightSfx);
        }
    }

    /// <summary>
    /// called by Events in Animation
    /// </summary>
    public void PlayRunSound(int leftFootGreaterZero)
    {
        Debug.Log("Tap!!");
        if (leftFootGreaterZero > 0)
        {
            PlaySound(footstepRunLeftSfx);
        }
        else
        {
            PlaySound(footstepRunRightSfx);
        }
    }

    /// <summary>
    /// needs to be called in Animation
    /// </summary>
    public void PlayLandingSound()
    {
        PlaySound(jumpLandingSfx);
    }
    #endregion Sfx

    private void OnDisable()
    {
        //if (instance == this) instance = null;
        SwitchScript.onDimensionSwitch -= SwitchConversion;
        CollisionScript.pushingObject -= Pushing;


        Inputs.onUpdate -= OnUpdate;
        Inputs.playerMovement -= PlayerMovement;
        Inputs.playerTriggeredAction -= PlayerAction;
    }
}
