using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity {

    // Use this for initialization

    [Header("Enemy")]
    public float movementSpeed;
    public LayerMask playerLayerMask;
    public GameObject onDeathParticles;
    public bool isImmuneToHazards;

    protected override void Awake()
    {
        base.Awake();
    }


    protected override void Start ()
    {
        base.Start();
        deathParticles = onDeathParticles;
	}
	
	// Update is called once per frame
	protected override void Update ()
    {
        base.Update();

        EnemyContactAttack();
	}

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (currControlType == 0)
        {
            EnemyMove();
        }
    }

    float changeDirectionTimer = 2;
    float changeDirectionCounter;

    private void EnemyMove()
    {
        baseInputSpeed = (movementSpeed);

        changeDirectionCounter -= Time.deltaTime;
        if(changeDirectionCounter < 0)
        {
            movementSpeed *= -1;
            changeDirectionCounter = changeDirectionTimer;
        }
    }

    private void EnemyContactAttack()
    {      
       Collider2D[] playersToDamage = Physics2D.OverlapBoxAll(entityBC2D.bounds.center, new Vector2(entityBC2D.bounds.size.x, entityBC2D.bounds.size.y), 0, playerLayerMask);
        foreach (Collider2D col in playersToDamage)
        {
            LivingEntity livingScript = col.gameObject.GetComponent<LivingEntity>();
            if(livingScript != null)
            {
                Attack contactAttack = new Attack(1f, true, this.gameObject, new Vector2(9f, 9f));
                livingScript.EntityHit(contactAttack);
            }
        }

    }

    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(entityBC2D.bounds.center, new Vector2(entityBC2D.bounds.size.x, entityBC2D.bounds.size.y));
    }
    */
}
