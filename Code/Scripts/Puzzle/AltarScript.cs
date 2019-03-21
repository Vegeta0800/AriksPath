using UnityEngine;

public class AltarScript : MonoBehaviour
{

    #region Declarations

    private int countOfRealStatues = 0;

    [SerializeField] private Transform[] slots;
    [SerializeField] private BoxCollider box;
    [SerializeField] private float duration;
    [SerializeField] private float duration2;

    private Material door;
    private bool doTransition;
    private bool part1;

    public AudioManager aud;

    private float t = 0.0f;

    #endregion

    //Enable Delegates
    private void OnEnable()
    {
        Inventory.onAltarPlaceObject += AltarData;
        Inventory.onStatueTaken += RealCount;
        Inputs.onUpdate += OnUpdate;
        door = GetComponent<Renderer>().material;
    }

    private void Start()
    {
        doTransition = false;
        box.enabled = true;
        door.SetFloat("_Opacity_Clip", 0.5f);
        door.SetFloat("_emission_amount", 0.0f);
    }

    private void OnUpdate()
    {

        if (doTransition == true)
        {
            t += 1.0f / duration * Time.deltaTime;

            box.enabled = false;
            door.SetFloat("_Opacity_Clip", Mathf.SmoothStep(0.5f, 1.1f, t));

            if (door.GetFloat("_Opacity_Clip") == 1.1f)
            {
                t = 0.0f;
                doTransition = false;
                aud.Play("puzzleComplete", false, false);

            }
        }

        if (part1 == true)
        {
            t += 1.0f / duration2 * Time.deltaTime;

            door.SetFloat("_emission_amount", Mathf.SmoothStep(0f, 3f, t));

            if (door.GetFloat("_emission_amount") == 3f)
            {
                t = 0.0f;
                part1 = false;
                doTransition = true;
            }
        }



    }

    //Getting data for script
    private void AltarData(bool realStatue, Transform statue)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].childCount == 0)
            {
                aud.Play("statuePlace", false, false);

                statue.position = slots[i].position;
                statue.SetParent(slots[i]);
                statue.rotation = Quaternion.Euler(0f, -42.929f, 0f);

                if (realStatue == true)
                {
                    countOfRealStatues++;
                }
                break;
            }
        }
        Debug.Log(slots[slots.Length - 1].childCount);
        Debug.Log(countOfRealStatues);

        if (countOfRealStatues >= slots.Length && slots[slots.Length - 1].childCount == 1)
        {
            part1 = true;
        }
        else
        {
            Debug.Log("not all Statues");
        }
    }

    private void RealCount()
    {
        countOfRealStatues -= 1;
    }

    //Disable Delegates
    private void OnDisable()
    {
        Inventory.onAltarPlaceObject -= AltarData;
        Inventory.onStatueTaken -= RealCount;
        Inputs.onUpdate -= OnUpdate;
    }
}
