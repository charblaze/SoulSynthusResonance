using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class PlayerMovement : MonoBehaviour
{
    /*
        This fairly long compnent handles all physics based player movement and combat and various other things. Most RPG elements are handled by Player.cs
    */


    // these are modifiable by the RPG script
    public float moveSpeed = 10f;
    public float jumpPower = 1f;
    public float jumpDistance = 70000f;

    // these are animations that are needed to tell time
    public AnimationClip RollAnim, LandAnim;

    // Camera / components
    Vector3 forward_CAM;
    Vector3 MoveDIR;
    public Animator anm;
    Rigidbody cc;
    CapsuleCollider col;
    ConstantForce cf;
    float DistanceToGround;
    public GameObject sword;
    Player p;
    BipedIK ik;

    // INITIALIZOR
    void Start()
    {
        cc = GetComponent<Rigidbody>();
        anm = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        cf = GetComponent<ConstantForce>();
        p = GetComponent<Player>();
        ik = GetComponent<BipedIK>();
        // put sword sheathed
        WeaponChanged();
        
        //Cursor.visible = false;
    }

    // function called when the equipped weapon of the player changes
    public void WeaponChanged()
    {
        isWeaponSheathed = true;
        instantiateSheathedSword();
        // change all animations bumbo
        // vivi is a bum
        RuntimeAnimatorController cn = anm.runtimeAnimatorController;
        AnimatorOverrideController ov = new AnimatorOverrideController();
        ov.runtimeAnimatorController = cn;
        WeaponScript ws = sword.GetComponent<WeaponScript>();
        ov["ReadySword"] = ws.WeaponHoldTarget;
        ov["HoldSword"] = ws.WeaponHold;
        ov["Swing1"] = ws.LightATK;
        ov["Swing2"] = ws.LightATK2;
        ov["Swing3"] = ws.LightATK3;
        ov["Attack1"] = ws.HeavyATK;
        ov["Attack2"] = ws.HeavyATK2;
        ov["ChargeAttack"] = ws.ChargeAttack;
	    ov["RollAttack"] = ws.RollAttack;
        //ov["SpecialAttack"] = ws.SpecialAttack;
        ov["Sheathe"] = ws.Sheathe;
        ov["Unsheathe"] = ws.Unsheathe;
        anm.runtimeAnimatorController = ov;
    }

    // SHEATHING IN RPG NEEDS TO GET LOCAL POS / ROT DEPENDING ON THE WEAPON TYPE
    public void instantiateSheathedSword()
    {

        AudioSource.PlayClipAtPoint(sheathesound, transform.position, 0.6f);
        if (sword != null)
        {
            Destroy(sword);
        }
        // Instantiate the equipped weapon
        sword = Instantiate(Resources.Load("Weapons/" + p.EquippedWeapon + "/base")) as GameObject;

        // get the weaponscript from it
        WeaponScript ws = sword.GetComponent<WeaponScript>();
        SetCorrectParrent(ws.WeaponParentSheathed, sword);
        sword.GetComponent<Collider>().enabled = false;
        sword.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().enableEmission = false;
        sword.transform.localPosition = ws.WeaponLocationSheathed;
        sword.transform.localRotation = ws.WeaponRotationSheathed;
        weaponloc = ws.WeaponLocation;
        weaponlocsh = ws.WeaponLocationSheathed;
        weaponrot = ws.WeaponRotation;
        weaponrotsh = ws.WeaponRotationSheathed;
        //sword.transform.localPosition = new Vector3(-.032f, -.143f, -.064f);
        // sword.transform.localRotation = new Quaternion(.0003608f, .599695f, -.000592649f, .854846f);
    }

    public Transform spine;
    public Transform hand;
    public Transform hips;
    public Transform chest;

    /// <summary>
    /// sets gameobject specified to correct string based parent
    /// </summary>
    /// <param name="p"></param>
    /// <param name="g"></param>
    void SetCorrectParrent(string p, GameObject g)
    {
        if(p == "Spine")
        {
            g.transform.SetParent(spine);
        } else if (p == "Hand")
        {
            g.transform.SetParent(hand);
        } else if (p == "Hips")
        {
            g.transform.SetParent(hips);
        } else if (p == "Chest")
        {
            g.transform.SetParent(chest);
        }
    }

    /// <summary>
    /// spawns the player's weapon unsheathed
    /// </summary>
    void instantiateUnsheathedSword()
    {

        AudioSource.PlayClipAtPoint(sheathesound, transform.position, 0.6f);
        if (sword != null)
        {
            Destroy(sword);
        }
        // TO DO: Instantiate the correct sword based on what the player has equipped
        sword = Instantiate(Resources.Load("Weapons/" + p.EquippedWeapon + "/base")) as GameObject;

        // get the weaponscript from it
        WeaponScript ws = sword.GetComponent<WeaponScript>();
        SetCorrectParrent(ws.WeaponParent, sword);
        sword.GetComponent<Collider>().enabled = false;
        sword.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().enableEmission = false;
        sword.transform.localPosition = ws.WeaponLocation;
        sword.transform.localRotation = ws.WeaponRotation;
        weaponloc = ws.WeaponLocation;
        weaponlocsh = ws.WeaponLocationSheathed;
        weaponrot = ws.WeaponRotation;
        weaponrotsh = ws.WeaponRotationSheathed;
        //sword.transform.localPosition = new Vector3(-.068f, .489f, -.022f);
        //sword.transform.localRotation = new Quaternion(.0882259f, .7074087f, .7003437f, -.0355345f);
    }

    /// <summary>
    /// determines if the Player is grounded
    /// </summary>
    /// <returns></returns>
    bool isGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f,Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 0.7f , 1);
        Debug.DrawLine(transform.position + Vector3.up * 0.5f, transform.position - Vector3.up * 0.5f, Color.black, 0.4f);
        for (int c = 0; c < hits.Length; ++c)
        {
                if (hits[c].transform.tag == "Floor")
                {
                    return true;
                }
        }
        return false;
    }
    bool isJumping = false;
    // JUMP METHOD, this is static and not affected by RPG.
    IEnumerator Jump()
    {
        p.LoseStamina(25f);
        isPerformingAction = true;
        isJumping = true;
        anm.SetTrigger("JumpTrigger");
        //cc.AddForce(Vector3.up * 300f);
        yield return new WaitForSeconds(0.7f);
        isJumping = false;
        isPerformingAction = false;
    }

    // when landed, character has 0 velocity
    bool landstun = false;
    // Land lag, time affected by RPG stat (TO BE IMPLEMENTED)
    // needs PUBLIC ANIMATION CLIP TO FIND OUT WAITFORSECONDS TIME
    IEnumerator Land()
    {
        anm.ResetTrigger("EndHurt");
        landstun = true;
        isPerformingAction = true;

        attackmoving = false;
        anm.SetTrigger("LandingTrigger");
        cc.velocity = new Vector3(0, cc.velocity.y, 0);

        // 1.6 seconds is the default 1 tenacity duration of the animation.
        // modify wait time
        yield return new WaitForSeconds(1.6f * p.Tenacity);

        anm.SetTrigger("EndHurt");
        isPerformingAction = false;
        willLand = false;
        landstun = false;
        gettingHURT = false;
        isPerformingAction = false;
        isPerformingQuickAction = false;
        willLand = false;
        NOHANDS = false;
        isSpellCasting = false;
        moveSpeed = runspeed;
    }

    bool isRolling = false;
    public AudioClip RollSound;
    void PlayRollSound()
    {
        AudioSource.PlayClipAtPoint(RollSound, transform.position);
    }

    IEnumerator Roll()
    {
        PlayRollSound();
        p.LoseStamina(15f);
        print("ROLLING");
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        isRolling = true;
        anm.SetTrigger("Roll");
        customvelocity = true;
        float rolltime = 0.6f * p.Swiftness;
        // IMMUNITY
        p.GrantImmunity(0.4f);
        customveloc = new Vector3(transform.forward.x * runspeed * 2, cc.velocity.y, transform.forward.z * runspeed * 2);
        // .583 of total roll time
        yield return new WaitForSeconds(rolltime * .583f);
        customveloc = new Vector3(transform.forward.x * runspeed / 2, cc.velocity.y, transform.forward.z * runspeed / 2);
        // .283 of total roll time
        yield return new WaitForSeconds(rolltime * .283f);
        customveloc = new Vector3(0, cc.velocity.y, 0);
        // .133 of total roll time
        yield return new WaitForSeconds(rolltime * .133f);
        customvelocity = false;
        isPerformingAction = false;
        isRolling = false;
    }

    IEnumerator StrafeRoll(float v, float h)
    {
        PlayRollSound();
        p.LoseStamina(15f);
        print("STRAFEROLLING");
        isRolling = true;
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        anm.SetTrigger("StrafeRoll");
        customvelocity = true;

        float rolltime = 0.6f * p.Swiftness;
        forward_CAM = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        MoveDIR = v * forward_CAM + h * Camera.main.transform.right;
        if (h == 0 && v == 0)
        {
            MoveDIR = forward_CAM;
        }
        // IMMUNITY
        p.GrantImmunity(0.4f);
        customveloc = new Vector3(MoveDIR.x * runspeed, cc.velocity.y, MoveDIR.z * runspeed);
        yield return new WaitForSeconds(rolltime * .583f);
        customveloc = new Vector3(MoveDIR.x * runspeed / 2, cc.velocity.y, MoveDIR.z * runspeed / 2);
        yield return new WaitForSeconds(rolltime * .283f);
        customveloc = new Vector3(0, cc.velocity.y, 0);
        yield return new WaitForSeconds(rolltime * .133f);
        customvelocity = false;
        isPerformingAction = false;
        isRolling = false;
    }

    public AudioClip sheathesound;


    // needs PUBLIC ANIMATION CLIP TO FIND OUT WAITFORSECONDS TIME
    IEnumerator SheatheWeapon()
    {

        isPerformingQuickAction = true;
        anm.SetTrigger("Sheath");
        yield return new WaitForSeconds(0.25f);

        yield return new WaitForSeconds(0.25f);
        // put sword in hand
        Destroy(sword);
        instantiateSheathedSword();
        // do whoosh sound effect and spark
        GameObject s = Instantiate(Resources.Load("Effects/WeaponSparks"), sword.transform.position, transform.rotation) as GameObject;
        s.GetComponent<ParticleSystem>().startColor = p.CharacterColor;
        s.transform.SetParent(gameObject.transform);
        isWeaponSheathed = true;
        yield return new WaitForSeconds(0.5f);
        isPerformingQuickAction = false;
    }

    IEnumerator UnsheatheWeapon()
    {
        isPerformingQuickAction = true;
        anm.SetTrigger("Unsheath");
        yield return new WaitForSeconds(0.25f);

        yield return new WaitForSeconds(0.25f);
        // put sword on back
        Destroy(sword);
        instantiateUnsheathedSword();
        GameObject s = Instantiate(Resources.Load("Effects/WeaponSparks"), sword.transform.position, transform.rotation) as GameObject;
        s.GetComponent<ParticleSystem>().startColor = p.CharacterColor;
        s.transform.SetParent(gameObject.transform);
        isWeaponSheathed = false;
        yield return new WaitForSeconds(0.5f);
        isPerformingQuickAction = false;
    }

    /// <summary>
    /// ANIMATION EVENT: Called at the end of any attack that is not a heavy attack.
    /// </summary>
    void EndOfAttack()
    {
        if (AttackIsQueued || attack1held)
        {
            AttackIsQueued = true;
            Attack();
            AttackIsQueued = false;
            return;
        }
        customvelocity = false;
        isAttacking = false;
        isPerformingAction = false;
        AttackIsQueued = false;
        hasOverwritingLegAnimation = false;
        p.ParryBonus = false;
    }

    public bool isAttacking = false;

    /// <summary>
    /// Function called when the player wishes to light attack
    /// </summary>
    void Attack()
    {

        p.LoseStamina(sword.GetComponent<WeaponScript>().STAMCOST_Light);
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        customvelocity = true;
        isAttacking = true;
        anm.SetBool("Attacking", isAttacking);
        if (!AttackIsQueued)
        {
            anm.SetTrigger("AttackTrigger");
        } else
        {
            anm.SetTrigger("AttackContinueTrigger");
        }
        customveloc = Vector3.zero;
    }

    /// <summary>
    /// Called when a charge attack is performed
    /// </summary>
    void ChargeAttack()
    {

        p.LoseStamina(sword.GetComponent<WeaponScript>().STAMCOST_Charge, 2.1f);
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        customvelocity = true;
        isAttacking = true;
        anm.SetBool("Attacking", isAttacking);
            anm.SetTrigger("ChargeAttackTrigger");
        customveloc = Vector3.zero;
    }

    // ANIMATION EVENT that sends the player forward
    void CustomVelocityBegin(float f)
    {
        customvelocity = true;
        customveloc = new Vector3(transform.forward.x * f, cc.velocity.y, transform.forward.z * f);
    }

    // ANIMATION EVENT that stops the player
    void CustomVelocityEnd(float f)
    {
        customveloc = Vector3.zero;
    }

    public void RollAttack()
    {

        p.LoseStamina(sword.GetComponent<WeaponScript>().STAMCOST_Roll);
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        customvelocity = true;
        isAttacking = true;
        // first face the target if targetting
            if (isTargeting && TARGET != null)
            {
                var lookpos = -transform.position + TARGET.transform.position;
                lookpos.y = 0;
                var tarrot = Quaternion.LookRotation(lookpos);
            transform.rotation = tarrot;
            } else
        {
            // otherwise just face the input direction
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (Camera.main.transform != null)
            {
                // only do this if NOT TARGETTING or if you are DASHING

                
                
                    forward_CAM = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                    MoveDIR = v * forward_CAM + h * Camera.main.transform.right;

                    transform.rotation = Quaternion.LookRotation(MoveDIR);
                
            }
        }
        anm.SetTrigger("OutofrollAttack");

    }

    bool hasOverwritingRUNLegAnimation = false;

    // ANIMATION EVENT that occurs when the player can move a little between attacks
    void AllowPlayerMovement(float f)
    {
        attackmoving = true;
        attackmovespeed = f;
        EnableLegMovement(f, 0);
    }

    void AllowPlayerRotation(float f)
    {
        attackmoving = true;
        attackmovespeed = 0;
    }

    void EndAllowPlayerRotation(float f)
    {
        attackmoving = false;
    }

    void AllowPlayerMovementRUN(float f)
    {
        attackmoving = true;
        attackmovespeed = f;
        EnableLegMovement(f, 1);
    }

    // ANIMATION EVENT where the player can no longer move between attacks
    void EndAllowPlayerMovement()
    {
        attackmoving = false;
        DisableLegMovement();
    }

    // ANIMATION EVENT that turns on strafing while performing other animations
    void EnableLegMovement(float f, int b)
    {
        if (b == 1)
        {
            hasOverwritingRUNLegAnimation = true;

            hasOverwritingLegAnimation = true;
        } else
        {

            hasOverwritingLegAnimation = true;
        }
    }

    // ANIMATION EVENT that ends strafing while performing other actions
    void DisableLegMovement()
    {
        hasOverwritingRUNLegAnimation = false;
        hasOverwritingLegAnimation = false;
    }

    // animation event that signals that we are done performing a heavy attack
    void EndOfHeavyAttack()
    {
        if (HeavyAttackisQueued || attack2held)
        {
            HeavyAttackisQueued = true;
            StartOfHeavyAttack();
            HeavyAttackisQueued = false;
            return;
        }
        customvelocity = false;
        isHeavyAttacking = false;
        isPerformingAction = false;
        HeavyAttackisQueued = false;
        hasOverwritingLegAnimation = false;
        isAttacking = false;
        p.ParryBonus = false;
    }

    // animation event that signals that we are about to perform a heavy attack
    void StartOfHeavyAttack()
    {
        p.LoseStamina(sword.GetComponent<WeaponScript>().STAMCOST_Heavy);
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        customvelocity = true;
        isHeavyAttacking = true;
        isAttacking = true;
        if (!HeavyAttackisQueued)
        {
            anm.SetTrigger("HeavyAttackTrigger");
        }
        else
        {
            anm.SetTrigger("HeavyAttackContinueTrigger");
        }
        customveloc = Vector3.zero;
    }

    void BeginParry()
    {
        p.LoseStamina(sword.GetComponent<WeaponScript>().STAMCOST_Special);
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        customvelocity = true;
        isHeavyAttacking = true;
        isAttacking = true;
            anm.SetTrigger("ParryTrigger");
        customveloc = Vector3.zero;
    }
    public bool isParrying = false;
    void ParryStart()
    {
        isParrying = true;
        sword.GetComponent<Collider>().enabled = true;
    }

    public void ParryEnd()
    {
        isParrying = false;
        sword.GetComponent<Collider>().enabled = false;
    }

    public void ParryAnimEnd()
    {
        customvelocity = false;
        isHeavyAttacking = false;
        isPerformingAction = false;
        HeavyAttackisQueued = false;
        hasOverwritingLegAnimation = false;
        isAttacking = false;
    }


    void EndOfSpellCast(int i)
    {

        // this is where you would instantiate the selected spell
        Instantiate(Resources.Load("Spells/0"), transform.position + transform.forward * 1.4f + transform.up * 1f, transform.rotation);
        customvelocity = false;
        isSpellCasting = false;
        isPerformingAction = false;
        isPerformingQuickAction = false;
        hasOverwritingLegAnimation = false;
        isAttacking = false;
    }


    bool isSpellCasting = false;
    
    void BeginCastSpell()
    {
        p.LoseStamina(30f);
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        isPerformingQuickAction = true;
        customvelocity = true;
        isSpellCasting = true;
        isAttacking = true;
            anm.SetTrigger("SpellCastTrigger");
        customveloc = Vector3.zero;
    }

    void BeginCastSelfSpell()
    {
        p.LoseStamina(30f);
        cc.constraints = RigidbodyConstraints.FreezeRotation;
        isPerformingAction = true;
        isPerformingQuickAction = true;
        customvelocity = true;
        isSpellCasting = true;
        isAttacking = true;
        anm.SetTrigger("SelfSpellTrigger");
        customveloc = Vector3.zero;
    }

    // current coroutine being performed
    Coroutine CURRENTACTION;

    // RPG stats (run and dash speeds)
    public float runspeed = 3f;
    public float dashspeed = 6f;
    public float walkspeed = 1.5f;
    bool isRunning = true;
    public float Walking = 0f;
    bool isDashing = false;
    bool isPerformingAction = false;
    bool isPerformingQuickAction = false;
    bool willLand = false;
    bool hasOverwritingLegAnimation = false;

    // CUSTOM VELOCITIES for actions
    bool customvelocity = false;
    Vector3 customveloc;

    // WEAPON SHEATH
    bool isWeaponSheathed = true;

    // TARGETING
    public Transform TARGET;
    public bool isTargeting = false;
    public float maxTargetDistance = 10f;
    bool AttackIsQueued = false;
    bool OutofrollAttackIsQueued = false;
    public bool isHeavyAttacking = false;
    bool HeavyAttackisQueued = false;
    bool attackmoving = false;
    float attackmovespeed = 0f;

    // QUEUEING
    bool RollIsQueued = false;
    bool QUEUEHeavyAttack = false;
    bool QUEUELightAttack = false;
    bool attack2held = false;
    bool attack1held = false;

    public bool WeaponIsHITTING = false;

    /// <summary>
    /// ANIMATION EVENT: Signals when the player's weapon is lethal
    /// </summary>
    void StartWeaponHit()
    {
        sword.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().startColor = p.CharacterColor;
        sword.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().enableEmission = true;
        sword.GetComponent<Collider>().enabled = true;
        WeaponIsHITTING = true;
    }

    /// <summary>
    /// ANIMATION EVENT: Signals when the player's weapon stops being lethal
    /// </summary>
    void EndWeaponHit()
    {
        sword.GetComponent<Collider>().enabled = false;
        sword.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().enableEmission = false;
        WeaponIsHITTING = false;
    }

    /// <summary>
    /// Coroutine that always runs. Checks for out of place bools and animations. Prevents Random input bugs.
    /// </summary>
    /// <returns></returns>
    IEnumerator checktriggers()
    {
        yield return new WaitForSeconds(0.05f);
        if (anm.GetBool("AttackContinueTrigger"))
        {
            customvelocity = false;
            isAttacking = false;
            isPerformingAction = false;
            AttackIsQueued = false;
            hasOverwritingLegAnimation = false;
            anm.ResetTrigger("AttackContinueTrigger");
            EndWeaponHit();
        }
        if (anm.GetBool("HeavyAttackContinueTrigger"))
        {
            customvelocity = false;
            isAttacking = false;
            isPerformingAction = false;
            AttackIsQueued = false;
            HeavyAttackisQueued = false;
            isHeavyAttacking = false;
            hasOverwritingLegAnimation = false;
            anm.ResetTrigger("HeavyAttackContinueTrigger");
            EndWeaponHit();
        }
    }

    public bool isBlocking = false;

    bool gettingHURT = false;

    /// <summary>
    /// Coroutine that plays the player's hurting animation, which stuns the player based on their tenacity
    /// </summary>
    /// <returns></returns>
    IEnumerator GetHurt()
    {
        gettingHURT = true;
        isPerformingAction = true;
        isPerformingQuickAction = true;
        moveSpeed = 0;
        EndWeaponHit();
        if (isAttacking)
        {
            EndOfAttack();
        }
        if (isHeavyAttacking)
        {
            EndOfHeavyAttack();
        }
        DisableLegMovement();
        isAttacking = false;
        isHeavyAttacking = false;
        anm.SetTrigger("HurtTrigger");
        cc.velocity = new Vector3(0, cc.velocity.y, 0);
        // LAG TIME affected by tenacity
        yield return new WaitForSeconds(0.9f * p.Tenacity);
        anm.SetTrigger("EndHurt");
        gettingHURT = false;
        isPerformingAction = false;
        isPerformingQuickAction = false;
        willLand = false;

        isSpellCasting = false;
        moveSpeed = runspeed;
        NOHANDS = false;
    }

    bool NOHANDS = false;

    IEnumerator GetHurtBlocking()
    {
        gettingHURT = true;
        NOHANDS = true;
        isPerformingAction = true;
        isPerformingQuickAction = true;
        isBlocking = true;
        moveSpeed = 0;
        EndWeaponHit();
        if (isAttacking)
        {
            EndOfAttack();
        }
        if (isHeavyAttacking)
        {
            EndOfHeavyAttack();
        }
        DisableLegMovement();
        isAttacking = false;
        isHeavyAttacking = false;
        anm.SetTrigger("HurtTrigger");
        cc.velocity = new Vector3(0, cc.velocity.y, 0);

        yield return new WaitForSeconds(0.6f * p.Tenacity);

        anm.SetTrigger("EndHurt");
        gettingHURT = false;
        isPerformingAction = false;
        isPerformingQuickAction = false;
        willLand = false;
        NOHANDS = false;
        moveSpeed = runspeed;
    }

    IEnumerator KnockedDown()
    {
        gettingHURT = true;
        NOHANDS = false;
        isPerformingAction = true;
        isPerformingQuickAction = true;
        isBlocking = true;
        moveSpeed = 0;
        EndWeaponHit();
        if (isAttacking)
        {
            EndOfAttack();
        }
        if (isHeavyAttacking)
        {
            EndOfHeavyAttack();
        }
        DisableLegMovement();
        isAttacking = false;
        isHeavyAttacking = false;
        anm.SetTrigger("KnockTrigger");
        cc.velocity = new Vector3(0, cc.velocity.y, 0);

        yield return new WaitForSeconds(2.5f * p.Tenacity);

        anm.SetTrigger("EndHurt");
        gettingHURT = false;
        isPerformingAction = false;
        isPerformingQuickAction = false;
        willLand = false;
        NOHANDS = false;
        moveSpeed = runspeed;
    }

    IEnumerator DieAnimation()
    {
        gettingHURT = true;
        isPerformingAction = true;
        isPerformingQuickAction = true;
        isBlocking = true;
        moveSpeed = 0;
        EndWeaponHit();
        if (isAttacking)
        {
            EndOfAttack();
        }
        if (isHeavyAttacking)
        {
            EndOfHeavyAttack();
        }
        DisableLegMovement();
        isAttacking = false;
        isHeavyAttacking = false;
        anm.SetTrigger("Death");
        cc.velocity = new Vector3(0, cc.velocity.y, 0);

        yield return new WaitForSeconds(10f);
        gettingHURT = false;
        isPerformingAction = false;
        isPerformingQuickAction = false;
        willLand = false;
        NOHANDS = false;
        isSpellCasting = false;
        moveSpeed = runspeed;
        ///Application.LoadLevel(0);
    }

    public void fallingdown()
    {
        isPerformingAction = true;
        isPerformingQuickAction = true;
        isBlocking = true;
        EndWeaponHit();
        if (isAttacking)
        {
            EndOfAttack();
        }
        if (isHeavyAttacking)
        {
            EndOfHeavyAttack();
        }
        DisableLegMovement();
        isAttacking = false;
        isHeavyAttacking = false;
        cc.velocity = new Vector3(0, cc.velocity.y * 5f, 0);
    }

    Coroutine hurtblock;

    public enum TakeDamageAnimationType { NORMAL, BLOCKING, FALLDOWN, DEATH};

    public bool GetHurtAnimation(TakeDamageAnimationType type, Vector3 from)
    {
        if (hurtblock != null)
        {
            StopCoroutine(hurtblock);
        }
        // update the animator based on where you got hit from
        Vector3 s = (from - transform.position).normalized * 2;
        float xs = Vector3.Dot(s, transform.forward) * 2f;
        anm.SetFloat("HitForward", xs );
        anm.SetFloat("HitSideways", Vector3.Dot(s, transform.right) * 2f);
        anm.ResetTrigger("EndHurt");

        if (type== TakeDamageAnimationType.BLOCKING)
        {
            hurtblock = StartCoroutine(GetHurtBlocking());
        }else if(type == TakeDamageAnimationType.FALLDOWN)
        {
            hurtblock = StartCoroutine(KnockedDown());
        } else if (type == TakeDamageAnimationType.DEATH)
        {
            hurtblock = StartCoroutine(DieAnimation());
        } else
        {
            hurtblock = StartCoroutine(GetHurt());
        }
        // return true if got hit from behind
        if(xs < 0.1)
        {
            return true;
        } else
        {
            return false;
        }
    }

    Vector3 weaponloc, weaponlocsh;
    Quaternion weaponrot, weaponrotsh;

    void KeepSwordPos()
    {
        if (!isWeaponSheathed)
        {
            sword.transform.localPosition = weaponloc;
            sword.transform.localRotation = weaponrot;
        }
        else
        {
            sword.transform.localPosition = weaponlocsh;
            sword.transform.localRotation = weaponrotsh;
        }
    }

    float tenacity = 1f;
    float swiftness = 1f;
    GameObject tarreticle;
    Vector3 lookpos = Vector3.zero;
    public GameObject uicanvas;

    void FixedUpdate()
    {

        // check if we are grounded each frame
        bool grounded = isGrounded();
        if (isAttacking && !grounded)
        {
            customvelocity = false;
            isHeavyAttacking = false;
            isPerformingAction = false;
            HeavyAttackisQueued = false;
            hasOverwritingLegAnimation = false;
            isAttacking = false;
        }
        if (isHeavyAttacking && !grounded)
        {
            customvelocity = false;
            isHeavyAttacking = false;
            isPerformingAction = false;
            HeavyAttackisQueued = false;
            hasOverwritingLegAnimation = false;
            isAttacking = false;
        }

        // GET AND UPDATE ALL RPG STATS
        runspeed = p.RunSpeed;
        dashspeed = p.RunSpeed * 2f;
        tenacity = p.Tenacity;
        swiftness = p.Swiftness;


        // DEATH ANIMATIONS

        /*if (Input.GetButtonDown("TEST"))
        {
            if (hurtblock != null)
            {
                StopCoroutine(hurtblock);
            }
            StartCoroutine(DieAnimation());
        }*/


        if (anm.GetBool("AttackContinueTrigger"))
        {
            StartCoroutine(checktriggers());
        }
        if (anm.GetBool("HeavyAttackContinueTrigger"))
        {
            StartCoroutine(checktriggers());
        }

        // get horizontal and vertical input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // first perform custom velocities if enabled
        if (customvelocity)
        {
          
            cc.velocity = customveloc;
        }



        // check if in landing lag and set velocity to ZERO
        if (landstun)
        {
            cc.velocity = Vector3.zero;
        }

        // if attack moving is on and we are grounded
        if (grounded)
        {
            if (attackmoving)
            {
                //rotate to desired location and move like normal but slowed
                // if targetting, look the right way, unless you are dashing
                if (isTargeting && TARGET != null)
                {
                    var lookpos = -transform.position + TARGET.transform.position;
                    lookpos.y = 0;
                    var tarrot = Quaternion.LookRotation(lookpos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, Time.deltaTime * 35f);
                }

                // Move
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                {

                    cc.constraints = RigidbodyConstraints.FreezeRotation;
                    // ROTATE in desired direction
                    if (Camera.main.transform != null)
                    {
                        // only do this if NOT TARGETTING or if you are DASHING

                        if (!isTargeting)
                        {
                            forward_CAM = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                            MoveDIR = v * forward_CAM + h * Camera.main.transform.right;

                            transform.rotation = Quaternion.LookRotation(MoveDIR);
                        }
                    }
                    // move FORWARD simply if you're not targetting
                    if (!isTargeting)
                    {
                        isRunning = true;
                        cc.velocity = new Vector3(transform.forward.x * attackmovespeed, cc.velocity.y, transform.forward.z * attackmovespeed);
                    }
                    else
                    {
                        // if you are, you must travel in the correct direction
                        isRunning = true;
                        // velocity would be... wherever the axis is pointing times movespeed ^_^
                        forward_CAM = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                        MoveDIR = v * forward_CAM + h * Camera.main.transform.right;
                        cc.velocity = new Vector3(MoveDIR.x * attackmovespeed, cc.velocity.y, MoveDIR.z * attackmovespeed);
                    }
                }
                else
                {
                    isRunning = false;
                    if (!isPerformingAction)
                    {
                        cc.velocity = new Vector3(0, 0, 0);
                        cc.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                    }
                }
            }
        }

        // check if landing lag is applicable. TO BE IMPLEMENTED : fall damage
        if (cc.velocity.y < -10)
        {
            willLand = true;
        }
        if (runHELD && p.isAbleToDashAgain)
        {

            isDashing = true;
            // these lines are necessary. Only dash if you're actually dashing..
            if (anm.GetCurrentAnimatorStateInfo(0).IsName("Dashing") && p.isAbleToAttack)
            {

                moveSpeed = dashspeed;
            } else
            {
                moveSpeed = runspeed;
            }
        }
        else
        {
            isDashing = false;
            moveSpeed = runspeed;
        }

        if (moveSpeed == dashspeed)
        {
            p.LoseGradualStamina(p.StaminaRegenPerSecond + 10f);
        }

        // check if landing lag is necessary
        if (willLand)
        {
            if (grounded)
            {
                if (willLand)
                {
                    if (CURRENTACTION != null)
                    {
                        StopCoroutine(CURRENTACTION);
                    }
                    CURRENTACTION = StartCoroutine(Land());
                }
                willLand = false;
            }
        }

        // BLOCKING
        if (Input.GetButton("Block") && p.isAbleToAttack)
        {
            if(grounded && !isPerformingAction && !isPerformingQuickAction && !isWeaponSheathed)
            {
                isBlocking = true;
                p.LoseGradualStamina(0, true);
            } else
            {
                isBlocking = false;
            }
        } else
        {
            isBlocking = false;
        }
        if (NOHANDS)
        {
            isBlocking = true;
            p.LoseGradualStamina(0, true);
        }
        // Update ANIMATOR
        anm.SetBool("Running", isRunning);
        anm.SetBool("Grounded", grounded);
        anm.SetBool("Dashing", isDashing);
        anm.SetBool("SwordisSheathed", isWeaponSheathed);
        anm.SetBool("Targetting", isTargeting);
        anm.SetBool("Attacking", isAttacking);
        anm.SetFloat("VectorForward", v);
        anm.SetFloat("VectorSideways", h);
        anm.SetBool("AttackIsQueued", AttackIsQueued);
        anm.SetBool("LegAnimation", hasOverwritingLegAnimation);
        anm.SetBool("HeavyAttackIsQueued", HeavyAttackisQueued);
        anm.SetBool("LegRunAnimation", hasOverwritingRUNLegAnimation);
        anm.SetBool("Blocking", isBlocking);
        anm.SetBool("NOHANDS", NOHANDS);
        anm.SetFloat("Swiftness", 2 - p.Swiftness);
        anm.SetFloat("Tenacity", p.Tenacity);
        anm.SetFloat("Walking", Walking);

        // walking
        Vector2 dirchecker = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        if(dirchecker.magnitude < 0.3 || Input.GetButton("Walk"))
        {
            Walking = 0f;
            moveSpeed = walkspeed;
        } else
        {
            Walking = p.RunSpeed / 4.5f;
        }

        // Player can change target whenever
        if (Input.GetButtonUp("Target"))
        {
            if (isTargeting)
            {
                isTargeting = false;
                TARGET = null;
                Destroy(tarreticle);
            }
            else
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyTarget");
                if (enemies.Length > 0)
                {
                    GameObject closestEnemy = enemies[0];
                    for (int c = 0; c < enemies.Length; ++c)
                    {
                        if ((transform.position - enemies[c].transform.position).magnitude <= (transform.position - closestEnemy.transform.position).magnitude)
                        {
                            closestEnemy = enemies[c];
                        }
                    }
                    if ((transform.position - closestEnemy.transform.position).magnitude < maxTargetDistance)
                    {
                        isTargeting = true;
                        TARGET = closestEnemy.transform;
                        // spawn ui reticle
                        tarreticle = Instantiate(Resources.Load("UI/Target")) as GameObject;
                        tarreticle.transform.SetParent(uicanvas.transform);
                    }
                }
            }
        }



        // and at any time if the target exceeds max distance or is blocked by wall or is simply null then lose the target
        if (TARGET == null || (transform.position - TARGET.transform.position).magnitude > maxTargetDistance)
        {
            isTargeting = false;
            TARGET = null;
            Destroy(tarreticle);
        }


        // HANDLE HEAD LOOKING IK/*
            GameObject[] faces = GameObject.FindGameObjectsWithTag("Face");
            if (faces.Length != 0 && (!isAttacking && !isHeavyAttacking))
            {
                GameObject closestface = faces[0];
                for (int c = 0; c < faces.Length; ++c)
                {
                    if ((faces[c].transform.position - transform.position).magnitude < (closestface.transform.position - transform.position).magnitude)
                    {
                        closestface = faces[c];
                    }
                }
                if ((closestface.transform.position - transform.position).magnitude < 10)
                {

                    lookpos = Vector3.Lerp(lookpos, closestface.transform.position, Time.deltaTime * 4f);
                    //ik.solvers.lookAt.target = closestface.transform;
                    ik.SetLookAtPosition(lookpos);
                    ik.solvers.lookAt.IKPositionWeight = Mathf.Lerp(ik.solvers.lookAt.IKPositionWeight, 1f, Time.deltaTime * 5f);
                }
                else
                {
                    ik.solvers.lookAt.IKPositionWeight = Mathf.Lerp(ik.solvers.lookAt.IKPositionWeight, 0f, Time.deltaTime * 5f);
                }
            }
            else
            {
                ik.solvers.lookAt.IKPositionWeight = Mathf.Lerp(ik.solvers.lookAt.IKPositionWeight, 0f, Time.deltaTime * 5f);
            }
        if (!isRunning && grounded)
        {
            GetComponent<GrounderBipedIK>().enabled = true;
        } else
        {

            GetComponent<GrounderBipedIK>().enabled = true;
        }


        if (runTAPPED)
        {
            if (AttackIsQueued)
            {
                AttackIsQueued = false;
            }
            if (HeavyAttackisQueued)
            {
                HeavyAttackisQueued = false;
                
            }
            if (isRolling || isAttacking || isHeavyAttacking)
            {
                RollIsQueued = true;
            }
        }
        // spell casting

        if(grounded && isWeaponSheathed && p.isAbleToAttack && !isPerformingAction && !isPerformingQuickAction && !isSpellCasting)
        {
            if(Input.GetButton("Attack") )
            {
                BeginCastSpell();
            }
            if (Input.GetButton("Attack2"))
            {
                BeginCastSelfSpell();
            }
        }


        if (grounded && !isWeaponSheathed && p.isAbleToAttack)
        {
            if (!isPerformingAction)
            {
                // check for out of roll attack
                if (OutofrollAttackIsQueued)
                {
                    RollAttack();
                    OutofrollAttackIsQueued = false;
                }
            }

            if (Input.GetButton("Special"))
            {
                if(!isPerformingAction && !isPerformingQuickAction)
                {
                    BeginParry();
                }
            }

            // ATTACK
            if (Input.GetButton("Attack") || QUEUELightAttack)
            {
                // if already attacking, queue the next attack
                bool gotem = false;
                if (isAttacking && !isHeavyAttacking)
                {
                    AttackIsQueued = true;
                    gotem = true;
                }
                if (isHeavyAttacking)
                {
                    HeavyAttackisQueued = false;
                    QUEUELightAttack = true;

                    gotem = true;
                }
                if (isRolling)
                {
                    OutofrollAttackIsQueued = true;

                    gotem = true;
                }
                if (!isPerformingAction && !isPerformingQuickAction)
                {

                    if (CURRENTACTION != null)
                    {
                        StopCoroutine(CURRENTACTION);
                    }
                    if (isDashing)
                    {
                        ChargeAttack();
                    } else
                    {
                        Attack();
                    }

                    QUEUELightAttack = false;

                    gotem = true;
                }
                if (!gotem)
                {
                    QUEUELightAttack = true;
                }
            }
            // HEAVY ATTACK
            if (Input.GetButton("Attack2") || QUEUEHeavyAttack)
            {
                bool gotem = false;
                // if already heavy attacking queue the next heavy attack
                if (isHeavyAttacking)
                {
                    HeavyAttackisQueued = true;
                    gotem = true;
                }
                if(isAttacking && !isHeavyAttacking)
                {
                    HeavyAttackisQueued = false;
                    QUEUEHeavyAttack = true;
                    gotem = true;
                }
                if (isRolling)
                {
                    OutofrollAttackIsQueued = true;
                    gotem = true;
                }
                if(!isPerformingAction && !isPerformingQuickAction)
                {
                    if(CURRENTACTION != null)
                    {
                        StopCoroutine(CURRENTACTION);
                    }
                    if (isDashing)
                    {
                        ChargeAttack();
                    } else
                    {
                        StartOfHeavyAttack();
                    }

                    QUEUEHeavyAttack = false;
                    gotem = true;
                }
                if (!gotem)
                {
                    QUEUEHeavyAttack = true;
                }
            }
        } 
        // only works if gorundedf
        if (grounded && !isPerformingAction)
        {



            // SHEATHE or UNSHEATHE
            if (Input.GetButton("Sheathe"))
            {
                if (!isPerformingQuickAction)
                {
                    if (CURRENTACTION != null)
                    {
                        StopCoroutine(CURRENTACTION);
                    }
                    if (isWeaponSheathed)
                    {
                        StartCoroutine(UnsheatheWeapon());
                    }
                    else
                    {
                        StartCoroutine(SheatheWeapon());
                    }
                }
            }

            // DODGE
            if ((runTAPPED || RollIsQueued))
            {
                if (!p.isAbleToAttack)
                {
                    RollIsQueued = false;
                }
                if (!isPerformingAction && p.isAbleToAttack)
                {
                    // ROLL if not targeting
                    if (!isTargeting)
                    {
                        if (CURRENTACTION != null)
                        {
                            StopCoroutine(CURRENTACTION);
                        }
                        CURRENTACTION = StartCoroutine(Roll());
                        RollIsQueued = false;
                    }
                    else
                    {
                        if (CURRENTACTION != null)
                        {
                            StopCoroutine(CURRENTACTION);
                        }
                        CURRENTACTION = StartCoroutine(StrafeRoll(v, h));
                        RollIsQueued = false;
                    }
                }
            }

            // if targetting, look the right way, unless you are dashing
            if (isTargeting && TARGET != null && !isDashing)
            {
                var lookpos = -transform.position + TARGET.transform.position;
                lookpos.y = 0;
                var tarrot = Quaternion.LookRotation(lookpos);
                transform.rotation = Quaternion.Slerp(transform.rotation, tarrot, Time.deltaTime * 5f);
            }

            // Move
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {

                cc.constraints = RigidbodyConstraints.FreezeRotation;
                // ROTATE in desired direction
                if (Camera.main.transform != null)
                {
                    // only do this if NOT TARGETTING or if you are DASHING

                    if (!isTargeting || isDashing)
                    {
                        forward_CAM = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                        MoveDIR = v * forward_CAM + h * Camera.main.transform.right;

                        transform.rotation = Quaternion.LookRotation(MoveDIR);
                    }
                }
                // move FORWARD simply if you're not targetting
                if (!isTargeting)
                {
                    isRunning = true;
                    cc.velocity = new Vector3(transform.forward.x * moveSpeed , cc.velocity.y, transform.forward.z * moveSpeed);
                }
                else
                {
                    // if you are, you must travel in the correct direction
                    isRunning = true;
                    // velocity would be... wherever the axis is pointing times movespeed ^_^
                    forward_CAM = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                    MoveDIR = v * forward_CAM + h * Camera.main.transform.right;
                    cc.velocity = new Vector3(MoveDIR.x * moveSpeed, cc.velocity.y, MoveDIR.z * moveSpeed);
                }

                // JUMP
                if (isDashing && p.isAbleToAttack)
                {
                    if (Input.GetButton("Jump"))
                    {
                        if (CURRENTACTION != null)
                        {
                            StopCoroutine(CURRENTACTION);
                        }
                        CURRENTACTION = StartCoroutine(Jump());
                    }
                }
            }
            else
            {
                isRunning = false;
                if (!isPerformingAction)
                {
                    cc.velocity = new Vector3(0, 0, 0);
                    cc.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                }
            }
        }
        
        if(isAttacking || isHeavyAttacking)
        {
            if(customveloc == Vector3.zero && !hasOverwritingLegAnimation && !hasOverwritingRUNLegAnimation)
            {
                cc.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

                cc.velocity = new Vector3(0, 0, 0);
            }
        }


        if (gettingHURT)
        {
            isPerformingAction = true;
            isPerformingQuickAction = false;
            CustomVelocityEnd(0);
            EndWeaponHit();
            DisableLegMovement();

            isAttacking = false;
            isHeavyAttacking = false;
            moveSpeed = 0;
            cc.velocity = Vector3.zero;
        }


        if (!grounded)
        {
            gettingHURT = true;
            isPerformingAction = true;
            isPerformingQuickAction = true;
            moveSpeed = 0;
            EndWeaponHit();
            if (isAttacking)
            {
                EndOfAttack();
            }
            if (isHeavyAttacking)
            {
                EndOfHeavyAttack();
            }
            DisableLegMovement();
            isAttacking = false;
            isHeavyAttacking = false;
            anm.SetBool("Grounded", false);
            cc.constraints = RigidbodyConstraints.FreezeRotation;
            cc.AddForce(Vector3.down * 500f, ForceMode.Acceleration);
            //cc.velocity = new Vector3(cc.velocity.x, cc.velocity.y, cc.velocity.z);
            //cc.AddForce(Vector3.down * 1000f);
            gettingHURT = false;
            isPerformingAction = false;
            isPerformingQuickAction = false;
            willLand = false;

            isSpellCasting = false;
            moveSpeed = runspeed;
            NOHANDS = false;
        }
        if (isJumping)
        {
            cc.AddForce(Vector3.up * 2000f + transform.forward * 1900f, ForceMode.Force);
            //cc.AddForce(Vector3.up * 7f, ForceMode.VelocityChange);
            //cc.AddForce(transform.forward * jumpDistance, ForceMode.Impulse);
        }
        // lastly keep the sword in place bro

        KeepSwordPos();

        // IMAGE EFFECTS

    }

    void LateUpdate()
    {
        if (isTargeting && TARGET != null && tarreticle != null)
        {
            tarreticle.transform.position = Camera.main.WorldToScreenPoint(TARGET.position);
        }
    }

    public float testScalar = 5f;
    bool BLURRING = false;
    float BLURAMOUNT = 3f;

    /*
        From here on, this portion of this component handles INPUT
    */
    // DIFFERENTIATE RUN TAP AND RUN HELD

    bool runTAPPED = false;
    bool runHELD = false;
    float timer = 0.2f;

    IEnumerator runtapped()
    {
        runTAPPED = true;
        yield return new WaitForFixedUpdate();
        runTAPPED = false;
    }

    public GameObject menuCanvas;
    public bool MenuIsUp = false;
    
    void Update()
    {

        if (Input.GetButton("Attack"))
        {
            attack1held = true;
        } else
        {
            attack1held = false;
        }

        if (Input.GetButton("Attack2"))
        {
            attack2held = true;
        } else
        {
            attack2held = false;
        }

        // GET BUTTON PRESSES
        if (Input.GetButtonDown("Run"))
        {
            timer = 0.2f;
        }
        if (Input.GetButton("Run"))
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                runHELD = true;
            }
        }
        if (Input.GetButtonUp("Run"))
        {
            runHELD = false;
            if (timer > 0)
            {
                StartCoroutine(runtapped());
            }
        }

        if (Input.GetButtonUp("Menu"))
        {
            if (MenuIsUp)
            {
                MenuIsUp = false;
                menuCanvas.SetActive(false);

                isPerformingAction = false;
                isPerformingQuickAction = false;
            } else
            {
                MenuIsUp = true;
                menuCanvas.SetActive(true);
                AttackIsQueued = false;
                HeavyAttackisQueued = false;
                isPerformingAction = true;
                isPerformingQuickAction = true;
                menuCanvas.GetComponent<StartMenuScript>().StartUpMenu();
            }
        }
    }
}
