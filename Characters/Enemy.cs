using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class Enemy : Character {

    /*
        BASIC ENEMY SCRIPT. This script provides very simple basic AI for enemies. Can be extended to provide more variety in AI
        */

    // dependent gameobjects / components assigned in start function
    Animator anm;
    GameObject player;
    Rigidbody rb;

    // The Weapon (or damaging tool) of this enemy.
    public EnemyWeapon ew;

    // the target loc
    public Transform targetloc;
    
    /// <summary>
    /// ANIMATION EVENT: makes the enemy's weapon lethal
    /// </summary>
    /// <param name="i">Which Damage Number To Use</param>
    void StartWeaponHit(int i)
    {
        ew.DamageDoing = AttackDamages[i].Damages;
        ew.isLethal = true;
    }

    /// <summary>
    /// ANIMATION EVENT: makes the enemy's weapon non lethal
    /// </summary>
    /// <param name="i"></param>
    void EndWeaponHit(int i)
    {
        ew.DamageDoing = new int[7];
        ew.isLethal = false;
    }

    public MultiDimensionalInt[] AttackDamages;

    [System.Serializable]
    public class MultiDimensionalInt
    {
        public int[] Damages;
    }

    // UI VARIABLES!
    GameObject HBContainer;
    RectTransform HB;
    public GameObject UICanvas;
    public Transform HBPOS;

	// Use this for initialization
	void Start () {
        anm = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        ChillRES = 10;
        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();
        //CURRENTACTION = StartCoroutine(RunTowardsPlayer());

        // ADD ALL POSSIBLE ACTIONS UNDER UI HERE
        Priorities.Add("Attack1", 0f);
        Priorities.Add("Attack2", 0f);
        Priorities.Add("Attack3", 0f);
        Priorities.Add("NoAction", 0f);


        HBContainer = Instantiate(Resources.Load("UI/HB")) as GameObject;
        HBContainer.transform.SetParent(UICanvas.transform);
        HBContainer.transform.position = Camera.main.WorldToScreenPoint(HBPOS.position);
        HB = HBContainer.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
	}
    
    // UI FUNCTION CALLED IN UPDATE
    GameObject damagetxt = null;
    void UpdateGUI()
    {
        if (DamageFloatText)
        {
            DamageFloatText = false;
            if (!crit)
            {
                damagetxt = Instantiate(Resources.Load("UI/Damage")) as GameObject;
                damagetxt.GetComponent<Text>().text = DamageTaken + "";
                damagetxt.transform.SetParent(UICanvas.transform);
                damagetxt.transform.position = Camera.main.WorldToScreenPoint(sourcex);
            }
            else
            {
                damagetxt = Instantiate(Resources.Load("UI/DamageCrit")) as GameObject;
                damagetxt.GetComponent<Text>().text = DamageTaken + "";
                damagetxt.transform.SetParent(UICanvas.transform);
                damagetxt.transform.position = Camera.main.WorldToScreenPoint(sourcex);
            }

        }
        float dot = Vector3.Dot((HBPOS.position + Vector3.up * 0.5f - Camera.main.transform.position).normalized, Camera.main.transform.forward);
        if (HBContainer != null)
        {
            if (dot > 0)
            {
                HBContainer.SetActive(true);
                HBContainer.transform.position = Camera.main.WorldToScreenPoint(HBPOS.position);
                HB.offsetMax = new Vector2(-(1 - (float)(CurrentHealth) / (float)(MaximumHealth)) * 200f, 2);

            }
            else
            {
                HBContainer.SetActive(false);
            }
            if (!isAggroed)
            {
                HBContainer.SetActive(false);

            }
            else
            {
                HBContainer.SetActive(true);
            }
        }
    }

    // COROUTINES OF ALL AVAILIBLE AI ACTIONS

    IEnumerator NoAction()
    {
        CANCAST = false;
        isPerformingAction = false;
        yield return new WaitForSeconds(3f);
        isPerformingAction = false;
        CANCAST = true;
    }

    IEnumerator Attack1()
    {
        CANCAST = false;
        isPerformingAction = true;
        anm.SetTrigger("AttackTrigger1");
        yield return new WaitForSeconds(3f);
        isPerformingAction = false;
        CANCAST = true;
    }

    IEnumerator Attack2()
    {
        CANCAST = false;
        isPerformingAction = true;
        anm.SetTrigger("AttackTrigger2");
        yield return new WaitForSeconds(3f);
        isPerformingAction = false;
        CANCAST = true;
    }

    IEnumerator Attack3()
    {
        CANCAST = false;
        isPerformingAction = true;
        anm.SetTrigger("AttackTrigger3");
        yield return new WaitForSeconds(3f);
        isPerformingAction = false;
        CANCAST = true;
    }

    bool customVelocity = false;
    Vector3 cveldir = Vector3.zero;

    /*
        Basic AI for all enemies:
        the base class for enemies have all these functions and can be called in their update class:
        - A function to stop the current action (coroutine)
        - A function to check the player's distance
        - A bool to check if aggroed
        - A function that runs the enemy to a specific destination
        - A function tthat strafes the enemy to a location
        - A function that walks the enemy to a specific destination

        1. Each update check for player's position and the distance away he is
        2. if the player is too far away, deagro and venture to original location / patrol path
        3. if player is close enough, aggro
        4. if took damage, aggro
        5. if far away from the player, and aggroed, venture to the player


        PRIORITY TREE Of the Following Actions:
        - Run towards player
        - Walk towards player
        - Rotate towards player
        - Strafe Left or Right
        - Dodge / roll in a direction
        - Attack
        - Block for a certain amount of time
        - Stop running / walking / strafing / performing action
    */

    // for fairly tall things prefeerred distance is 9. Represents the area in which the enemy will attack in
    public float PreferredDistance = 13f;
    public float AggroDistance = 13f;
    public float WalkFDistance = 12f;
    bool isPerformingAction = false;
    
    NavMeshAgent nma;
    Coroutine CURRENTACTION;
    
    void StopMoving()
    {
        anm.SetBool("Running", false);
        anm.SetBool("Walking", false);
        isRunning = false;
        isWalking = false;
        nma.Stop();
    }
    
    /// <summary>
    /// Gets ditsance from the player
    /// </summary>
    /// <returns></returns>
    float DistanceFromPlayer()
    {
        return (player.transform.position - transform.position).magnitude;
    }

    public bool isAggroed = false;

    // FUNCTIONS CALLED IN BETWEEN AI EVENTS. Move this enemy to various locations and update animators

    bool isRunning = false;
    bool isWalking = false;
    bool CANCAST = true;
    void RunTowardsPlayer()
    {
        nma.Resume();
            anm.SetBool("Running", true);
            anm.SetBool("Walking", false);
            nma.SetDestination(player.transform.position);
            nma.speed = RunSpeed;
            isRunning = true;
    }

    void WalkTowardsPlayer()
    {
        nma.Resume();
            anm.SetBool("Running", false);
            anm.SetBool("Walking", true);
        anm.SetFloat("WalkDirX", Mathf.Lerp(anm.GetFloat("WalkDirX"), 0, Time.deltaTime * 2f));
        anm.SetFloat("WalkDirY", Mathf.Lerp(anm.GetFloat("WalkDirY"), 1f, Time.deltaTime * 2f));
        nma.SetDestination(player.transform.position);
            nma.speed = RunSpeed * 0.5f;
            isRunning = false;
            isWalking = true;
    }

    void StrafeLeft()
    {
        anm.SetBool("Running", false);
        anm.SetBool("Walking", true);
        anm.SetFloat("WalkDirX", Mathf.Lerp(anm.GetFloat("WalkDirX"), -1f, Time.deltaTime * 2f));
        anm.SetFloat("WalkDirY", Mathf.Lerp(anm.GetFloat("WalkDirY"), 0, Time.deltaTime * 2f));
        rb.AddForce(transform.right * -1 * RunSpeed * 0.3f, ForceMode.VelocityChange);
    }

    void StrafeRight()
    {
        anm.SetBool("Running", false);
        anm.SetBool("Walking", true);
        anm.SetFloat("WalkDirX", Mathf.Lerp(anm.GetFloat("WalkDirX"), 1f, Time.deltaTime * 2f));
        anm.SetFloat("WalkDirY", Mathf.Lerp(anm.GetFloat("WalkDirY"), 0, Time.deltaTime * 2f));
        rb.AddForce(transform.right * RunSpeed * 0.3f, ForceMode.VelocityChange);
    }

    void Backup()
    {
        anm.SetBool("Running", false);
        anm.SetBool("Walking", true);
        anm.SetFloat("WalkDirX", Mathf.Lerp(anm.GetFloat("WalkDirX"), 0, Time.deltaTime * 2f));
        anm.SetFloat("WalkDirY", Mathf.Lerp(anm.GetFloat("WalkDirY"), -1f, Time.deltaTime * 2f));
        rb.AddForce(transform.forward * -1 * RunSpeed * 0.4f, ForceMode.VelocityChange);
    }

    void StrafeNowhere()
    {
        anm.SetBool("Running", false);
        anm.SetBool("Walking", true);
        anm.SetFloat("WalkDirX", Mathf.Lerp(anm.GetFloat("WalkDirX"), 0, Time.deltaTime * 2f));
        anm.SetFloat("WalkDirY", Mathf.Lerp(anm.GetFloat("WalkDirY"), 0f, Time.deltaTime * 2f));
    }

    float DesiredDirectionTime = 2f;
    bool ChosenOneDirection = false;
    int dirchosen = 0;
    void Movement()
    {
        if(DesiredDirectionTime < 0)
        {
            ChosenOneDirection = false;
        }
        if (!ChosenOneDirection)
        {
            ChosenOneDirection = true;
            DesiredDirectionTime = 1.4f;
            dirchosen = Random.Range(0, 8);
        } else
        {
            DesiredDirectionTime -= Time.deltaTime;
            if (dirchosen == 0 || dirchosen == 1)
            {
                StrafeLeft();
            }
            else if (dirchosen == 2 || dirchosen == 3)
            {
                StrafeRight();
            }
            else if (dirchosen == 4 || dirchosen == 5)
            {
                WalkTowardsPlayer();
            }
            else if (dirchosen == 6)
            {
                Backup();
            } else
            {
                StrafeNowhere();
            }
        }
    }

    public float TurnSpeed = 1.5f;

    void FacePlayer()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), Time.deltaTime * TurnSpeed);
    }

    Dictionary<string, float> Priorities = new Dictionary<string, float>();
    public bool isActioning = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        tencur += Time.deltaTime * 0.5f;
        if(tencur > Tenacity)
        {
            tencur = Tenacity;
        }
        UpdateGUI();
        if (customVelocity)
        {
            rb.AddForce(cveldir, ForceMode.Impulse);
        }
        if (!isStunnedCurrently && !isDying)
        {
            if (DistanceFromPlayer() <= AggroDistance)
            {
                isAggroed = true;
                
            } else
            {
                return;
            }

            float dfp = DistanceFromPlayer();

            // calculate intended move direction
            if (isAggroed && !isPerformingAction)
            {
                if (dfp <= PreferredDistance)
                {
                    nma.Stop();
                    //StopMoving();
                    FacePlayer();
                    Movement();
                    isActioning = true;
                    //    StopMoving();
                }
                else if ((dfp <= PreferredDistance + 5) && (dfp > PreferredDistance))
                {
                    nma.Stop();
                    FacePlayer();
                    WalkTowardsPlayer();
                }
                else
                {
                    isActioning = false;
                    RunTowardsPlayer();
                }
            }

            // calculate priorities
            if (dfp < 3)
            {
                Priorities["Attack1"] += 2f;
                Priorities["Attack2"] += 2f;
                Priorities["Attack3"] += 2f;
            }
            else
            {
                Priorities["NoAction"] += 2.3f;
                Priorities["Attack1"] += 1f;
                Priorities["Attack2"] += 1f;
                Priorities["Attack3"] += 1f;
            }


            if (CANCAST && isAggroed && isActioning)
            {
                // execute priorities
                List<KeyValuePair<string, float>> l = new List<KeyValuePair<string, float>>();
                l = Priorities.ToList();
                l.Sort((x, y) => x.Value.CompareTo(y.Value));
                int rng = 1;//Random.Range(l.Count - 3, l.Count - 1);
                nma.Stop();
                CURRENTACTION = StartCoroutine(l[rng].Key);
                Priorities[l[rng].Key] = 0f;
            }
        }
    }



    IEnumerator deathslam()
    {
        StunnedE(3.5f);
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }


    public override void Die()
    {
        if(targetloc!= null)
        {
            Destroy(targetloc.gameObject);
        }
        Destroy(HBContainer);
        if(CURRENTACTION!= null)
        {
            StopCoroutine(CURRENTACTION);
        }
        isDying = true;
        isPerformingAction = true;
        anm.SetTrigger("DeathTrigger");
        StartCoroutine(deathslam());
    }

    bool isDying = false;
    bool isStunnedCurrently = false;
    Coroutine stuncrt;
    public IEnumerator Stunned(float f)
    {
        isStunnedCurrently = true;
        isPerformingAction = true;
        yield return new WaitForSeconds(f);
        isPerformingAction = false;
        isStunnedCurrently = false;
    }

    public void StunnedE(float secs)
    {
        if(stuncrt != null)
        {
            StopCoroutine(stuncrt);
        }
        stuncrt = StartCoroutine(Stunned(secs));
        
    }

    Coroutine pushcrt;
    IEnumerator PushedBack(Vector3 dir)
    {
        customVelocity = true;
        cveldir = dir;
        yield return new WaitForSeconds(0.2f);
        cveldir = Vector3.zero;
        customVelocity = false;
    }

    public void PushBackInDir(Vector3 dir)
    {
        if(pushcrt!= null)
        {
            StopCoroutine(pushcrt);
        }
        pushcrt = StartCoroutine(PushedBack(dir));
    }

    int DamageTaken = 0;
    bool crit = false;
    bool DamageFloatText = false;
    public float Tenacity = 2f;
    float tencur = 1f;
    Vector3 sourcex = Vector3.zero;
    public override void TakeAHit(int DAMAGE_Te, int DAMAGE_Lu, int DAMAGE_So, int DAMAGE_In, int DAMAGE_Ch, int DAMAGE_Li, int DAMAGE_De, Vector3 source, bool ovr = false)
    {
        tencur -= 1.2f;
        // IF STAGGER
        if (tencur <= 0)
        {
            EndWeaponHit(0);
        }
        if (isDying)
        {
            return;
        }
        bool behind = CheckForBackHits(source);
        Vector3 s = (source - transform.position).normalized * 2;
        float xs = Vector3.Dot(s, transform.forward) * 2f;
        // if staggered
        if (tencur <= 0)
        {
            anm.SetFloat("HitForward", xs);
            anm.SetFloat("HitSideways", Vector3.Dot(s, transform.right) * 2f);
            anm.SetTrigger("HitTrigger");
        }
        int totaldamage = Mathf.CeilToInt(DAMAGE_Te * TerrestrialResistance + DAMAGE_Lu * LunarResistance + DAMAGE_So * SolarResistance + DAMAGE_In * InfernalResistance + DAMAGE_Ch * ChillResistance + DAMAGE_Li * LifeResistance + DAMAGE_De * DeathResistance);

        DamageFloatText = true;
        crit = behind;
        sourcex = source;
        if (crit)
        {
            DamageTaken = Mathf.CeilToInt(totaldamage * 1.5f);
        } else
        {
            DamageTaken = totaldamage;
        }
        if (behind)
        {
            LoseHealth(Mathf.CeilToInt(totaldamage * 1.5f));
            StunnedE(1f);
        }
        else
        {
            LoseHealth(totaldamage);
            StunnedE(0.6f);
        }
        if(tencur <= 0)
        {
            PushBackInDir(-(player.transform.position - transform.position).normalized * 555f);

            tencur = Tenacity;
        }
        //PushBackInDir(new Vector3(xs * 555f, 0, Vector3.Dot(s, transform.right) * 555f));
    }
    

}
