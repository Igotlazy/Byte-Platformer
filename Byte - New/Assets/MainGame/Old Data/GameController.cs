using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public float interScale;
    public float interTime;
    private bool usingInter;

    public static GameController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {

    }


    void Update()
    {

    }


    public void TimeScaleSlowDown(float startSlow, float timeToRecover)
    {
        if (startSlow < 0)
        {
            startSlow = 0;
        }

        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        StartCoroutine(timeSlow(startSlow, timeToRecover));
    }
    Coroutine timeCoroutine;

    private IEnumerator timeSlow(float startSlow, float timeToRecover)
    {
        float interTime = 0;
        float step = 0;
        while (interTime <= timeToRecover)
        {
            step = interTime / timeToRecover;
            Time.timeScale = Mathf.Lerp(startSlow, 1f, step);
            Time.fixedDeltaTime = 0.02F * Time.timeScale;

            interTime += Time.unscaledDeltaTime;

            yield return null;
        }

        Time.timeScale = 1;

    }
}
