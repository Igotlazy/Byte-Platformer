using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRotate : MonoBehaviour
{
    [SerializeField]
    Vector3 rotateVector;
    [SerializeField]
    float speed;
    float smallestSpeed = 3;

    void Start()
    {
        rotateVector.x = Random.Range(-1f, 1f);
        rotateVector.y = Random.Range(-1f, 1f);
        rotateVector.z = Random.Range(-1f, 1f);
        rotateVector = rotateVector.normalized;
        speed = Random.Range(-7f, 7f);
        if(speed < smallestSpeed && speed >= 0)
        {
            speed = smallestSpeed;
        }
        if(speed > -smallestSpeed)
        {
            speed = -smallestSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateVector * Time.deltaTime * speed);
    }
}
