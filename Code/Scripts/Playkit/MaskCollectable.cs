using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskCollectable : MonoBehaviour
{
    [SerializeField]
    Transform movedElement;

    [Header("Visualisation")]
    [SerializeField]
    float finalDistance = 0.5f;
    [SerializeField]
    float moveTime = 5f;
    [Space(5)]
    [SerializeField]
    Vector3 finalScale = new Vector3(1, 1, 1);

    [Space(5)]
    [SerializeField]
    ParticleSystem moveParticles;
    [SerializeField]
    ParticleSystem endParticles;
    [SerializeField]
    float endParticleDuration = 1.0f;
    [Space(5)]
    [SerializeField]
    CutsceneCameraPans cameraPan;

    MovementScript playerMovement;
    public AudioManager aud;

    public Inventory inv;

    [SerializeField] private GameObject maskOnBack;

    Vector3 positionShift;

    private bool inRange;

    bool alreadyTriggered;

    private void OnEnable()
    {
        Inputs.playerTriggeredAction += PlayerAction;
    }


    // Use this for initialization
    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag(StringCollection.TAG_PLAYER).GetComponent<MovementScript>();
    }

    private void PlayerAction(ePlayerAction action)
    {
        if (action == ePlayerAction.interact && !alreadyTriggered && inRange)
        {
            alreadyTriggered = true;

            Debug.Log("e");
            if (cameraPan)
                cameraPan.LockAndMoveCamera();
            StartCoroutine(WanderingMask());
            Inventory.OwnsMask = true;
        }
    }

    private IEnumerator WanderingMask()
    {
            aud.Play("maskPickUp", false, false);
        ParticleSystem.EmissionModule moveParticleEmission;
        if (moveParticles)
        {
            moveParticleEmission = moveParticles.emission;
            moveParticleEmission.enabled = true;
        }
        
        playerMovement.cameraAnimationLocked = true;

        Vector3 start = movedElement.transform.position;
        positionShift = (playerMovement.transform.position - start);
        float distance = positionShift.magnitude;
        positionShift *= (distance - finalDistance) / distance;

        Vector3 baseScale = movedElement.transform.localScale;
        Vector3 scaleShift = finalScale - baseScale;

        Vector3 baseRotation = movedElement.transform.eulerAngles;
        Vector3 rotationShift = playerMovement.transform.eulerAngles;
        //rotationShift.y -= 180;
        rotationShift -= baseRotation;
        while (rotationShift.y > 180)
            rotationShift.y -= 360;
        while (rotationShift.y < -180)
            rotationShift.y += 360;

        float current = 0;
        float smooth;
        Debug.Log(baseRotation + "/" + rotationShift);
        while (current <= moveTime)
        {
            smooth = (1 - Mathf.Cos((current / moveTime) * Mathf.PI)) / 2.0f;
            //Debug.Log(smooth);
            movedElement.transform.position = start + smooth * positionShift;
            movedElement.transform.localScale = baseScale + smooth * scaleShift;
            if (current / moveTime <= 0.75f)
                movedElement.transform.eulerAngles = baseRotation + 4.0f/3.0f * smooth * rotationShift;
            current += Time.deltaTime;
            yield return null;
        }

        if (moveParticles)
            moveParticleEmission.enabled = false;
        if (endParticles)
            StartCoroutine(EndParticle());
        else
        {
            playerMovement.cameraAnimationLocked = false;
            Destroy(this.gameObject);
            if (maskOnBack.activeInHierarchy == false)
            {
                maskOnBack.SetActive(true);
            }

            inv.pickingUp = false;
        }
    }

    private IEnumerator EndParticle()
    {
        ParticleSystem.EmissionModule endParticleEmission = endParticles.emission;
        endParticleEmission.enabled = true;
        yield return new WaitForSeconds(endParticleDuration);
        endParticleEmission.enabled = false;
        playerMovement.cameraAnimationLocked = false;
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;
    }

    private void OnDisable()
    {
        Inputs.playerTriggeredAction -= PlayerAction;
    }
}