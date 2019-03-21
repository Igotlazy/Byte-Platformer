using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHierarchy : MonoBehaviour {

    public GameObject marker;
    public float radius = 5f;
    public float markerMoveSpeed;
    public Vector3 markerToCenter;
    private float horizontalMovement;
    private float verticalMovement;

    private Rigidbody2D markerRB2D;

    void Start ()
    {
        markerRB2D = marker.GetComponent<Rigidbody2D>();
	}

    void Update()
    {
        float clampedTimeScale = Mathf.Clamp(Time.timeScale, 0.01f, 100f);
        horizontalMovement = (Input.GetAxisRaw("Horizontal") * (markerMoveSpeed / clampedTimeScale));
        verticalMovement = (Input.GetAxisRaw("Vertical") * (markerMoveSpeed / clampedTimeScale));

       Vector3 centerPosition = transform.localPosition; 
        float distance = Vector3.Distance(marker.transform.position, centerPosition); 

        if (distance > radius) //If the distance is less than the radius, it is already within the circle.
        {
            Vector3 currentMarkerToCenter = marker.transform.position - centerPosition; //~GreenPosition~ - *BlackCenter*
            currentMarkerToCenter *= radius / distance; //Multiply by radius //Divide by Distance
            Vector3 newLocation = centerPosition + currentMarkerToCenter; //*BlackCenter* + all that Math
            marker.transform.position = newLocation;
        }

        markerToCenter = marker.transform.position - centerPosition;
       
    }


    void LateUpdate()
    {      
        Vector3 bytePosition = Byte.instance.transform.position;
        transform.position = bytePosition;     
	}

    private void FixedUpdate()
    {
        markerRB2D.velocity = new Vector2(horizontalMovement, verticalMovement);
    }
}
