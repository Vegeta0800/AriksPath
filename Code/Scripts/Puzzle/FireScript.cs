using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FireScript : MonoBehaviour {

    private GameObject burning;
    private GameObject burning2;
    private BoxCollider box;
    public AudioSource DornBurn;

    [SerializeField] private Material thornbush;

    private List<bool> bools = new List<bool>();

    [SerializeField] private float duration;

    private GameObject fire;
    private float t = 0.0f;
    private bool burn;

    private void OnEnable()
    {
        Inventory.onFirePlaceObject += PlaceFire;
        Inputs.onUpdate += OnUpdate;
    }

    private void Start()
    {

        burn = false;

        thornbush.SetFloat("_Burning", 0f);
    }

    private void OnUpdate()
    {
            if (t >= duration)
            {
                Debug.Log("e");
                t = 0.0f;
                burn = false;
                bools.Clear();
                box = null;
                fire = null;
                burning = null;
                burning2 = null;
            }
        if(burn == true && fire != null)
        {
            t += 1.0f / duration * Time.deltaTime;

            burning.SetActive(true);
            burning2.SetActive(true);


            for (int i = 0; i < fire.transform.childCount; i++)
            {
                if (fire.transform.GetChild(i).GetComponent<Renderer>() != null)
                {
                    fire.transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("_Burning", Mathf.SmoothStep(0f, 1f, t));
                }
            }

            box.enabled = false;
       
        }
    }

    private void PlaceFire(GameObject game, bool t)
    {

        if (t)
        {
            DornBurn.Play();

            burn = true;
            fire = game;

            burning = fire.transform.Find("PS_Fire").gameObject;
            burning2 = fire.transform.Find("PS_Fire (1)").gameObject;
            box = fire.GetComponent<BoxCollider>();
        }
    }

    private void OnDisable()
    {
        Inventory.onFirePlaceObject -= PlaceFire;
        Inputs.onUpdate -= OnUpdate;
    }

}
