using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessProjectile : MonoBehaviour {

    public GameObject Player;
    private Byte playerScript;
    public float projectileLifeTime;

    void Start ()
    {
        Invoke("DestroySelf", projectileLifeTime);
    }
	

	void Update ()
    {
		
	}

    private void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if(hitCollider.tag != "Ground" && hitCollider.tag != "Player" && hitCollider.gameObject.activeInHierarchy)
        {
            GameObject hitObject = hitCollider.gameObject;

            PossessableObject possessScript = hitObject.GetComponent<PossessableObject>();

            if (possessScript != null)
            {
                if (possessScript.isPossessable)
                {
                    //Player.GetComponent<Byte>().PPossess(hitObject);

                    possessScript.currentlyPossessed = true;
                    possessScript.Player = Player;
                }
            }
            else
            {
                Debug.Log("Didn't Respond");
            }

            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
