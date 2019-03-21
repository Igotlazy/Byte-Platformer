using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [Header("Entity Movement")]
    protected float baseInputSpeed;
    protected float knockbackVelocity;
    private float totalVelocity;

    [Header("Entity: Health")]
    public float maxHealth;
    [SerializeField]
    private float currentHealth;
    public virtual float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
        }      
    }

    public float invincibilityDuration;
    private float invincibilityCounter;

    protected float knockbackInitialDuration;
    protected float knockbackSubDuration;

    [Header("Entity: Physics")]
    public float terminalVelocity = -20f;
    public float groundRayLength = 0.12f;
    protected float baseInputMultiplier = 1.0f;
    protected float knockbackMultiplier;
    protected float outsideVelocitySource;

    public enum ControlType
    {
        CanControl,
        CannotControl,
        OtherControl
    }
    [Tooltip("0 = Can Control, 1 = Cannot Control, 2 = Other Movement")]
    public ControlType currControlType;

    [Header("Entity: State Bools")]
    public bool isInvincible;
    public bool isLookingRight;
    public bool isKnockingBack;
    [SerializeField]
    protected bool isGrounded;


    protected BoxCollider2D entityBC2D;
    protected Rigidbody2D entityRB2D;

    protected GameObject deathParticles;

    protected LayerMask groundMask;

    protected Action damageKnockReact;
    

    protected virtual void Awake()
    {
        groundMask = (1 << LayerMask.NameToLayer("Ground - Soft")) | (1 << LayerMask.NameToLayer("Ground - Hard"));

        entityRB2D = this.GetComponent<Rigidbody2D>();

        entityBC2D = this.GetComponent<BoxCollider2D>();
    }


    protected virtual void Start()
    {
        CurrentHealth = maxHealth;
        EntityLookingCheck();
    }



    protected virtual void Update()
    {
        //Flipping 
        EntityFlipControl();
        
        //Timers
        EntityTimekeeping();
    }


    protected virtual void FixedUpdate()
    {
        //Controlling Entity
        if (currControlType == ControlType.CanControl || currControlType == ControlType.CannotControl)
        {
            EntityMove();
        }
    }


//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   
        // [FUNCTIONS] //

    public void EntityMove()
    {
        //Horizontal Movement
        totalVelocity = (baseInputSpeed * baseInputMultiplier) + (knockbackVelocity * knockbackMultiplier) + outsideVelocitySource;
        entityRB2D.velocity = new Vector2(totalVelocity, entityRB2D.velocity.y);

        //Vertical Movement
        float yVel = Mathf.Clamp(entityRB2D.velocity.y, terminalVelocity, 100.0f);
        entityRB2D.velocity = new Vector2(entityRB2D.velocity.x, yVel);
    }


    public void EntityLookingCheck()
    {
        float localScale = this.transform.localScale.x;
        if (localScale >= 0)
        {
            isLookingRight = true;
        }
        else
        {
            isLookingRight = false;
        }
    }


    public void EntityFlipControl()
    {
        if (flipControlLockCounter > 0)
        {
            flipControlLockCounter -= Time.deltaTime;
        }
        else
        {
            if (baseInputSpeed < 0.0f && isLookingRight == true)
            {
                EntityFlip();
            }
            else if (baseInputSpeed > 0.0f && isLookingRight == false)
            {
                EntityFlip();
            }
        }
    }

    protected float flipControlLockCounter;


    public void EntityFlip()
    {
        isLookingRight = !isLookingRight;
        Vector2 localScale = this.transform.localScale;
        localScale.x *= -1;
        this.transform.localScale = localScale;
    }


    protected void IsGrounded()
    {
        Vector2 leftRayOrigin = new Vector2(entityBC2D.bounds.center.x - entityBC2D.bounds.extents.x, entityBC2D.bounds.center.y - entityBC2D.bounds.extents.y);
        Vector2 rightRayOrigin = new Vector2(entityBC2D.bounds.center.x + entityBC2D.bounds.extents.x, entityBC2D.bounds.center.y - entityBC2D.bounds.extents.y);

        //Left Raycast
        bool leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, groundRayLength, groundMask);
        Debug.DrawRay(leftRayOrigin, Vector2.down * groundRayLength, Color.red);

        //Right Raycast
        bool rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, groundRayLength, groundMask);
        Debug.DrawRay(rightRayOrigin, Vector2.down * groundRayLength, Color.red);

        if (leftHit || rightHit)
        {
            isGrounded =  true;
        }
        else
        {
            isGrounded =  false;
        }
        
    }


    public void EntityHit(Attack receivedAttack)
    {
        if (!isInvincible)
        {
            float midHealth = CurrentHealth;
            midHealth -= receivedAttack.damageValue;
            CurrentHealth = Mathf.Clamp(midHealth, 0.0f, maxHealth);

            if (CurrentHealth <= 0)
            {
                EntityDeath();
            }
            else
            {
                invincibilityCounter = invincibilityDuration;
                isInvincible = true;

                if (receivedAttack.doesKnockback)
                {
                    if (receivedAttack.damageSource.transform.position.x > gameObject.transform.position.x)
                    {
                        receivedAttack.knockback.x *= -1;
                    }

                    if(damageKnockReact != null)
                    {
                        damageKnockReact.Invoke();
                    }
                    //Knockback
                    StartKnockback(receivedAttack.knockback, 0.2f, 0.5f);
                }
            }
        }
    }


    public void EntityDeath()
    {
        if (gameObject.CompareTag("Player"))
        {
            //Not decided yet.
            Debug.Log("Player Death");
        }

        if (gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy is Dying" + deathParticles);
            Instantiate(deathParticles, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
        }
    }

    protected void StartKnockback(Vector2 knock, float xInitialDur, float xSubDur)
    {
        if (lastKnockback != null)
        {
            StopCoroutine(lastKnockback);
        }
        lastKnockback = StartCoroutine(KnockbackCouroutine(knock, xInitialDur, xSubDur));
    }
    Coroutine lastKnockback;
    protected WaitForFixedUpdate waitForFix = new WaitForFixedUpdate();


    private IEnumerator KnockbackCouroutine(Vector2 knock, float initialDur, float subDur)
    {
        isKnockingBack = true;
        float internalTime = 0;
        float knockbackSubLerpTime = 0;

        if(knock.y != 0)
        {
            entityRB2D.velocity = new Vector2(entityRB2D.velocity.x, knock.y);
        }
        if(knock.x != 0)
        {
            baseInputMultiplier = 0;
            knockbackMultiplier = 1;

            knockbackVelocity = knock.x;

            while (!(baseInputMultiplier == 1 && knockbackMultiplier == 0))
            {
                if (internalTime <= (initialDur + subDur))
                {
                    knockbackSubLerpTime = (internalTime - initialDur) / subDur;
                    baseInputMultiplier = Mathf.Lerp(0f, 1f, knockbackSubLerpTime);
                    knockbackMultiplier = Mathf.Lerp(1f, 0f, knockbackSubLerpTime);
                }
                else
                {
                    baseInputMultiplier = 1;
                    knockbackMultiplier = 0;
                }

                internalTime += Time.fixedDeltaTime;
                yield return waitForFix;
            }
        }


        isKnockingBack = false;      
    }

    protected void EntityTimekeeping()
    {
        //Invincibility
        if (invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
            Debug.Log(isInvincible);
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
        }
    }

    protected void EntityControlTypeSet(ControlType givenType, bool controlTypeInstaStop)
    {
        if(returnToControlType != null)
        {
            StopCoroutine(returnToControlType);
        }

        currControlType = givenType;

        if (controlTypeInstaStop == true)
        {
            this.entityRB2D.velocity = new Vector2(0f, 0f);
        }
    }

    protected void EntityControlTypeSet(ControlType givenType, bool controlTypeInstaStop, float returnTime)
    {
        EntityControlTypeSet(givenType, controlTypeInstaStop);
        returnToControlType = StartCoroutine(ReturnToControlType(returnTime));
    }
    Coroutine returnToControlType;

    private IEnumerator ReturnToControlType(float returnTime)
    {
        float starTime = 0;

        while(starTime <= returnTime)
        {
            starTime += Time.deltaTime;
            yield return null;
        }

        currControlType = ControlType.CanControl;
    }
}
