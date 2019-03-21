using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Byte : LivingEntity
{
    public static Byte instance;

    [Header("Byte: Movement")]
    public float InputMultiplier;
    public override float CurrentHealth {
        get
        {
            return base.CurrentHealth;
        }
        set
        {
            base.CurrentHealth = value;
            Debug.Log("helt set");
            HealthSetEvent?.Invoke(CurrentHealth);
        }
    }
    public delegate void HealthSet(float health);
    public event HealthSet HealthSetEvent;

    public delegate void EnergySet(float currentEnergy, float maxEnery);
    public event EnergySet EnergySetEvent;

    public int currentEnergy;
    [SerializeField]
    private int maxEnergy = 5;
    public int CurrentEnergy
    {
        get
        {
            return currentEnergy;
        }
        set
        {
            if(!(CurrentEnergy == maxEnergy - 1 && CurrentHealth == maxHealth))
            {
                int mid = (int)Mathf.Clamp(value, 0f, maxEnergy);
                if (mid == maxEnergy)
                {
                    currentEnergy = 0;
                    CurrentHealth += 1;
                }
                else
                {
                    currentEnergy = mid;
                }
            }

            EnergySetEvent?.Invoke(CurrentEnergy, maxEnergy);
        }
    }


    [Header("Byte: Abilities")]
    [Header("Jumping")]
    public float jumpVelocity;
    public float maxExtraJumps;
    [SerializeField]
    private float currentExtraJumps;
    private bool isJumping = false;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    public float wallJumpRayLength;
    public bool canWallJump;

    [Header("Dash")]
    public float dashSpeed;
    public float dashTime;
    public AnimationCurve dashCurve;
    public float maxDashCharge;
    public float currentDashCharge;
    public GameObject dashParticles;
    public bool isDashing;



    [Header("Phase")]
    public GameObject phaseParticles;
    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    public Color normalColor;
    public Color phaseColor;
    public bool isPhasing;

    public HealthUI healthUI;


    /*
    [Header("Possess")]
    public float possessFireVel;
    public float possessFireCooldownDuration;
    private float possessFireCooldownCounter;
    public bool canFirePossess;
    public bool isPossessing;
    public GameObject possessProjectile;
    private GameObject currentPossessingObject;
    */


    [Header("Byte: Attacks")]
    public LayerMask playerAttackLayerMask;

    [Header("Slash")]
    public float slashDamage;
    public Vector2 slashKnockback;
    public Vector2 slashAttackSize;
    public float slashCooldownDuration;
    private float slashCooldownCounter;
    public bool canSlash;
    public bool didDownSlash;
    public Transform slashSideTrans;
    public Transform slashUpTrans;
    public Transform slashDownTrans;
    private Transform currSlashTrans;

    [Header("Blast")]
    public float blastDamage;
    public Vector2 blastKnockback;
    public float blastFireVel;
    public bool canFireBlast;
    public float blastCooldownDuration;
    private float blastCooldownCounter;
    public GameObject blastProjectile;
    private GameObject spawnedBlastProjectile;

    private GameController gameController;

    [Header("MISC:")]
    public Collider2D hurtBox;
    public Vector3 respawnPoint;





    protected override void Awake()
    {
        base.Awake();
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        damageKnockReact += KnockbackAbilityCancel;

        respawnPoint = transform.position;
    }


    protected override void Start()
    {
        base.Start();

        gameController = GameController.instance;
        currentExtraJumps = maxExtraJumps;
        currentDashCharge = maxDashCharge;
        currentEnergy = 0;

    }


    protected override void Update()
    {
        base.Update();

        //Get Input Axis [Horizontal Movement]
        if (currControlType == ControlType.CanControl)
        {
            baseInputSpeed = (Input.GetAxis("Horizontal") * InputMultiplier);
        }
        else
        {
            baseInputSpeed = 0;
        }

        if (currControlType == ControlType.CanControl)
        {
            IsGrounded();
            //Jumping
            PJump();
            PJumpCancel();

            //Attacks
            PSlashAttack();
            PBlastAttack();

            //Abilities
            PWallJumpSensor();
            PWallJump();

            PDashInitiate();
            //PPossessInitiate();

            EnterPhase();
            ExitPhase();
            PhaseSlowDown();

        }

        if (currControlType == ControlType.OtherControl)
        {
            if (isDashing)
            {
                EnterPhase();
                ExitPhase();
                PhaseSlowDown();
            }
            /*
            PPossessMovement();
            if (Input.GetButtonDown("Jump") && isPossessing)
            {
                PPossessCancel();
            }
            */
        }

        PTimekeeping();

        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnByte();
        }

    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        GroundedSets();
        TerminalVelocitySets();
    }

    //----------------------------------------------------------------------------------

    // METHODS

    public void RespawnByte()
    {
        Vector2 velocity = entityRB2D.velocity;
        transform.position = respawnPoint;
        entityRB2D.velocity = velocity;
    }

    protected void KnockbackAbilityCancel()
    {
        //Ability Cancels
        PAbilityCancel();

        EntityControlTypeSet(ControlType.CannotControl, true, 0.35f);
    }

    private void PAbilityCancel()
    {
        if (isDashing == true)
        {
            PDashCancel();
        }

        /*
        if (isPossessing == true)
        {
            PPossessCancel();
        }
        */
    }

    public void PJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !canWallJump)
        {
            
            Debug.Log("Jumping");
            this.entityRB2D.velocity = new Vector2(this.entityRB2D.velocity.x, jumpVelocity);
            isJumping = true;
        }
        // Extra Jumps
        else if (Input.GetButtonDown("Jump") && currentExtraJumps > 0 && !canWallJump)
        {
            Debug.Log("Double Jumping");
            this.entityRB2D.velocity = new Vector2(this.entityRB2D.velocity.x, jumpVelocity/1.25f);
            isJumping = true;

            currentExtraJumps -= 1;
        }
    }

    public void PJumpCancel()
    {
        if (((Input.GetButtonUp("Jump") && isJumping) || (Input.GetKeyUp(KeyCode.X) && didDownSlash)) && this.entityRB2D.velocity.y > 0)
        {
            if (didDownSlash)
            {
                didDownSlash = false;
            }
            this.entityRB2D.velocity = new Vector2(this.entityRB2D.velocity.x, 0);
        }
    }

    private void PSlashAttack()
    {

        if (Input.GetKeyDown(KeyCode.X) && canSlash)
        {
            Transform currSlashTrans = slashSideTrans;

            float verticalAxis = Input.GetAxis("Vertical");
            if(verticalAxis != 0)
            {
                if (verticalAxis > 0)
                {
                    currSlashTrans = slashUpTrans;
                }
                else if (verticalAxis < 0 && !isGrounded)
                {
                    currSlashTrans = slashDownTrans;
                }
            }

            Instantiate(dashParticles, currSlashTrans.position, Quaternion.identity);

            Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(new Vector2(currSlashTrans.position.x, currSlashTrans.position.y), slashAttackSize, 0f, playerAttackLayerMask);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                Attack slashAttack = new Attack(slashDamage, true, this.gameObject, slashKnockback);
                if(currSlashTrans == slashUpTrans)
                {
                    slashAttack.knockback.x = 0;
                }
                if(currSlashTrans == slashDownTrans)
                {
                    slashAttack.knockback.x = 0;
                    slashAttack.knockback.y *= -1;
                }
                enemiesToDamage[i].GetComponent<LivingEntity>().EntityHit(slashAttack);

                CurrentEnergy += 1;
            }
            if(enemiesToDamage.Length > 0 && currSlashTrans == slashDownTrans)
            {
                entityRB2D.velocity = new Vector2(entityRB2D.velocity.x, jumpVelocity);
                didDownSlash = true;
            }

            slashCooldownCounter = slashCooldownDuration;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (currSlashTrans != null)
        {
            Gizmos.DrawWireCube(
                new Vector3(currSlashTrans.transform.position.x, currSlashTrans.transform.position.y, 0),
                new Vector3(slashAttackSize.x, slashAttackSize.y, 0));
        }
    }

    public void GroundedSets()
    {
        if (isGrounded && entityRB2D.velocity.y <= 0)
        {
            isJumping = false;
            didDownSlash = false;
            currentExtraJumps = maxExtraJumps;
        }

        if (isGrounded && currentDashCharge != maxDashCharge)
        {
            currentDashCharge = maxDashCharge;
        }
    }

    private void PTimekeeping()
    {
        // Slash Cooldown
        {
            if (slashCooldownCounter > 0)
            {
                slashCooldownCounter -= Time.deltaTime;

                canSlash = false;
            }
            else
            {
                canSlash = true;
            }
        }

        //Blast Cooldown
        {
            if (blastCooldownCounter > 0)
            {
                blastCooldownCounter -= Time.deltaTime;

                canFireBlast = false;
            }
            else
            {
                canFireBlast = true;
            }
        }

        // Possess Fire Cooldown

        /*
        {
            if (possessFireCooldownCounter > 0)
            {
                possessFireCooldownCounter -= Time.deltaTime;

                canFirePossess = false;
            }
            else
            {
                canFirePossess = true;
            }
        }
        */
    }

    private void PWallJumpSensor()
    {
        Vector2 wallJumpRayDir = Vector2.right;

        if (!isLookingRight)
        {
            wallJumpRayDir = Vector2.left;
        }
        if((Physics2D.Raycast(transform.position, wallJumpRayDir, wallJumpRayLength, groundMask) && !isGrounded && currControlType == 0))
        {
            canWallJump = true;
        }
        else
        {
            canWallJump = false;
        }
        Debug.DrawRay(transform.position, wallJumpRayDir * wallJumpRayLength, Color.cyan);
    }

    private void PWallJump()
    {
        if (Input.GetButtonDown("Jump") && canWallJump)
        { 
            isJumping = false;

            if (isLookingRight)
            {
                StartKnockback(new Vector2(-wallJumpForce.x, wallJumpForce.y), 0.1f, 0.6f);
            }
            else
            {
                StartKnockback(wallJumpForce, 0.1f, 0.6f);
            }

            //Flip Sets
            flipControlLockCounter = 0.4f;
            EntityFlip();

            //Other Ability Interactions
            currentExtraJumps = maxExtraJumps;
            currentDashCharge = maxDashCharge;
        }
    }

    private void PDashInitiate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && currentDashCharge > 0)
        {
            Vector2 dashVec = Vector2.right;

            if (Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Horizontal") != 0)
            {
                dashVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                if(Input.GetAxisRaw("Horizontal") > 0 && !isLookingRight)
                {
                    EntityFlip();
                    flipControlLockCounter = 0;
                }
                if (Input.GetAxisRaw("Horizontal") < 0 && isLookingRight)
                {
                    EntityFlip();
                    flipControlLockCounter = 0;
                }
            }
            else
            {
                if (isLookingRight)
                {
                    dashVec.x = 1;
                }
                else
                {
                    dashVec.x = -1;
                }
            }

            if(dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }
            dashCoroutine = StartCoroutine(Dash(dashVec.normalized));           
        }
    }

    Coroutine dashCoroutine;
    float lastGrav;

    private IEnumerator Dash(Vector2 dashVector)
    {
        EntityControlTypeSet(ControlType.OtherControl, true);
        lastGrav = entityRB2D.gravityScale;
        entityRB2D.gravityScale = 0;
        isDashing = true;

        Instantiate(dashParticles, gameObject.transform.position, Quaternion.identity);


        float currentTime = 0;

        while(currentTime <= dashTime)
        {
            entityRB2D.velocity = dashVector * dashCurve.Evaluate(currentTime/dashTime) * dashSpeed;
            currentTime += Time.fixedDeltaTime;
            //Debug.Log(dashVector * dashDistance);
            yield return waitForFix;
        }

        EntityControlTypeSet(ControlType.CanControl, true);
        entityRB2D.gravityScale = lastGrav;
        isDashing = false;
        currentDashCharge -= 1;

    }

    private void PDashCancel()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        EntityControlTypeSet(ControlType.CanControl, true);
        entityRB2D.gravityScale = lastGrav;
        isDashing = false;
    }

    private void TerminalVelocitySets()
    {
        if (this.currControlType == 0 && this.canWallJump == true)
        {
            terminalVelocity = -5f;
        }

        else
        {
            terminalVelocity = -20f;
        }
        
    }
   
    /*
    private void PPossessInitiate()
    {
        if (Input.GetMouseButtonDown(1) && this.canFirePossess == true)
        {
            spawnedBlastProjectile = Instantiate(possessProjectile, this.transform.position, this.transform.rotation);
            Rigidbody2D possessProjectileRB2D = spawnedBlastProjectile.GetComponent<Rigidbody2D>();
            PossessProjectile possessProjectileScript = spawnedBlastProjectile.GetComponent<PossessProjectile>();

            possessProjectileScript.Player = this.gameObject;

            Vector2 fireVel = new Vector2(possessFireVel, 0);
            if (!isLookingRight)
            {
                fireVel.x *= -1;
            }

            possessProjectileRB2D.velocity = fireVel;

            possessFireCooldownCounter = possessFireCooldownDuration;
        }
    }

    
    public void PPossess(GameObject possessingObject)
    {
        isPossessing = true;
        EntityControlTypeSet(ControlType.OtherControl, true);
        entityRB2D.gravityScale = 0;
        entityBC2D.enabled = false;
        currentPossessingObject = possessingObject;
    }

    public void PPossessMovement()
    {
        if (isPossessing)
        {
            transform.Translate(currentPossessingObject.transform.position.normalized * 12f * Time.deltaTime);
        }
    }


    public void PPossessCancel()
    {
        currentPossessingObject.GetComponent<PossessableObject>().currentlyPossessed = false;
        currentPossessingObject = null;

        EntityControlTypeSet(ControlType.CanControl, true);
        entityRB2D.gravityScale = 2;
        entityBC2D.enabled = true;
        entityRB2D.velocity = new Vector2(entityRB2D.velocity.x, 7f);
        isPossessing = false;
    }
    */


    private void PBlastAttack()
    {
        if (Input.GetMouseButtonDown(0) && canFireBlast == true)
        {
            spawnedBlastProjectile = Instantiate(blastProjectile, this.transform.position, this.transform.rotation);
            Rigidbody2D blastProjectileRB2D = spawnedBlastProjectile.GetComponent<Rigidbody2D>();
            BlastProjectile blastProjectileScript = spawnedBlastProjectile.GetComponent<BlastProjectile>();

            Attack blastAttack = new Attack(blastDamage, true, this.gameObject, blastKnockback);
            blastProjectileScript.attackObject = blastAttack;

            Vector2 fireVel = new Vector2(blastFireVel, 0);
            if (!isLookingRight)
            {
                fireVel.x *= -1;
            }
             
            blastProjectileRB2D.velocity = fireVel;

            blastCooldownCounter = blastCooldownDuration;
        }
    }

    private void EnterPhase()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            gameObject.layer = LayerMask.NameToLayer("Player - Phase");
            hurtBox.gameObject.layer = LayerMask.NameToLayer("Player - Phase");
            groundMask = (1 << LayerMask.NameToLayer("Ground - Hard"));
            phaseParticles.SetActive(true);
            foreach(SpriteRenderer rend in sprites)
            {
                rend.color = phaseColor;
            }

            isPhasing = true;
        }
    }
    private void ExitPhase()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            hurtBox.gameObject.layer = LayerMask.NameToLayer("Player");
            groundMask = (1 << LayerMask.NameToLayer("Ground - Soft")) | (1 << LayerMask.NameToLayer("Ground - Hard"));
            phaseParticles.SetActive(false);
            foreach (SpriteRenderer rend in sprites)
            {
                rend.color = normalColor;
            }

            isPhasing = false;
        }
    }

    private void PhaseSlowDown()
    {
        if (isPhasing && nextCheck < Time.time)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, entityRB2D.velocity, 0.2f, (1 << LayerMask.NameToLayer("Ground - Soft")));
            if (hit)
            {
                nextCheck = Time.time + 0.2f;
                Debug.Log(hit.collider.gameObject.name);
                GameController.instance.TimeScaleSlowDown(0.5f, 0.5f);
            }

            Debug.DrawRay(transform.position, entityRB2D.velocity.normalized * 0.25f, Color.blue);
        }
    }
    float nextCheck = 0;
}
