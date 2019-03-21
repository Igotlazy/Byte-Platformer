using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastProjectile : MonoBehaviour {

    public float projectileLifeTime;
    public Attack attackObject;
    public GameObject onDeathParticles;

    void Start()
    {
        Destroy(gameObject, projectileLifeTime);
    }


    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.tag == "Enemy" && hitCollider.gameObject.activeInHierarchy)
        {
            GameObject hitObject = hitCollider.gameObject;

            LivingEntity enemyScript = hitObject.GetComponent<LivingEntity>();

            if (enemyScript != null)
            {
                enemyScript.EntityHit(attackObject);
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(onDeathParticles != null)
        {
            Instantiate(onDeathParticles, transform.position, Quaternion.identity);
        }
    }
}
