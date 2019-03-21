using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessMovablePlatform : PossessableObject {

    public GameObject platformToMove;
    public bool moveInX;
    public bool moveInY;
    public Vector2 movementSpeed;
    private Vector2 movingVector;
    [Tooltip("Right, Left, Up, Down")]
    public Vector4 constraints;
    public Vector4 constraintsPlus;
    private BoxCollider2D platformBC2D;

    private void Start()
    {
        platformBC2D = platformToMove.GetComponent<BoxCollider2D>();
        constraintsPlus = new Vector4(platformToMove.transform.position.x + constraints.x, platformToMove.transform.position.x - constraints.y,
            platformToMove.transform.position.y + constraints.z, platformToMove.transform.position.y - constraints.w);
    }


    void Update ()
    {
		if (currentlyPossessed)
        {

            float xBounds = platformBC2D.bounds.extents.x;
            float yBounds = platformBC2D.bounds.extents.y;
            float xInput = Input.GetAxis("Horizontal");
            float yInput = Input.GetAxis("Vertical");


            if (moveInX && 
                ((constraintsPlus.y < (platformToMove.gameObject.transform.position.x - xBounds) && xInput < 0) ||  
                (constraintsPlus.x > (platformToMove.gameObject.transform.position.x + xBounds) && xInput > 0)))
            {
                movingVector.x = (xInput * movementSpeed.x);              
            }
            else
            {
                movingVector.x = 0f;
            }

            if (moveInY &&
                ((constraintsPlus.w < (platformToMove.gameObject.transform.position.y - yBounds) && yInput < 0) || 
                (constraintsPlus.z > (platformToMove.gameObject.transform.position.y + yBounds) & yInput > 0)))
            {
                movingVector.y = (yInput * movementSpeed.y);
            }
            else
            {
                movingVector.y = 0f;
            }

            platformToMove.transform.Translate(movingVector * Time.deltaTime);           
        }
	}
}
