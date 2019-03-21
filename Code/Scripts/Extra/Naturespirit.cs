using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Naturespirit : MonoBehaviour{

    public AudioSource Trail;
    public Transform[] points;
    private NavMeshAgent agent;
    private NavMeshPath path;
    int targetPoint = 0;
    [SerializeField] private float breakTime = 0f;
    private float count;

	// Use this for initialization
	private void Start ()
    {
        
            Trail.Play(); 
        agent = GetComponent<NavMeshAgent>();
        agent.destination = points[targetPoint].position;
    }
	
	// Update is called once per frame
	private void Update ()
    { 
        if(Vector3.Distance(transform.position, agent.destination) < 1)
        {
            count += Time.deltaTime;
            if (count >= breakTime)
            {
                targetPoint++;
                if (targetPoint == points.Length) targetPoint = 0;
                agent.destination = points[targetPoint].position;
                count = 0;
            }
        }
	}
}
