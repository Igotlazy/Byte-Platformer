using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Did hit " + collision.gameObject.name);
        if(collision == Byte.instance.hurtBox)
        {
            Byte.instance.RespawnByte();
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
            if(enemyScript != null && !enemyScript.isImmuneToHazards)
            {
                enemyScript.EntityDeath();
            }
        }
    }
}
