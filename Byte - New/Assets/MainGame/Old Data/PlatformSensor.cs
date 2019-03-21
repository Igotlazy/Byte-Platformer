using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSensor : MonoBehaviour {

    public BoxCollider2D sensorBoxCollider2D;
    public BoxCollider2D platformBoxCollider2D;
    public float sensorHeight = 0.1f;
    public bool variableWidth;
    public bool variableHeight;


    void Start ()
    {
        SetWidth();
        SetHeight();
    }
	
	void LateUpdate ()
    {
		if (variableWidth)
        {
            SetWidth();
        }

        if (variableHeight)
        {
            SetHeight();
        }
	}



    private void SetWidth()
    {
        float boxColliderSizeX = platformBoxCollider2D.size.x;

        sensorBoxCollider2D.size = new Vector2(boxColliderSizeX, sensorHeight);
    }

    private void SetHeight()
    {
        float boxColliderHeightY = (platformBoxCollider2D.size.y / 2);
        float yOffSet = (boxColliderHeightY + (sensorHeight / 2));

        sensorBoxCollider2D.offset = new Vector2 (0f, yOffSet);
    }

    
    private void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.tag == "Player" || hitCollider.tag == "Enemy")
        {
            hitCollider.transform.parent = this.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D hitCollider)
    {
        if ((hitCollider.tag == "Player" || hitCollider.tag == "Enemy") && hitCollider.transform.parent == this.gameObject)
        {
            hitCollider.transform.parent = null;
        }
    }



}
