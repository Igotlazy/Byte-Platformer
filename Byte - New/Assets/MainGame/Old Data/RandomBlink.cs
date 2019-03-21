using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlink : MonoBehaviour {

    Animator animator;
    float timeToNextBlink;
    public float minTime;
    public float maxTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        timeToNextBlink = Random.Range(minTime, maxTime);
    }
    void Start ()
    {
		
	}
	

	void Update ()
    {
		if(Time.time > timeToNextBlink)
        {
            Blink();
            timeToNextBlink = Time.time + Random.Range(minTime, maxTime);
        }
	}

    void Blink()
    {
        animator.SetTrigger("Blink");
    }
}
